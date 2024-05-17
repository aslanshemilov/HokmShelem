namespace Engine.SignalR
{
    [Authorize]
    public class HokmHub : Hub
    {
        private readonly IUnityRepo _unity;
        private readonly IMapper _mapper;

        public HokmHub(IUnityRepo unity,
           IMapper mapper)
        {
            _mapper = mapper;
            _unity = unity;
        }
        public override async Task OnConnectedAsync()
        {
            var player = _unity.PlayerRepo.FindByName(PlayerName());
            player.ConnectionId = ConnectionId();
            var oldConnectionId = _unity.PlayerRepo.AddUpdatePlayer(player);
            // handle duplicate browser connection ....

            var game = _unity.GameRepo.FindByName(player.GameName, includeProperties: "Players");
            if (game != null)
            {
                UpdatePlayerStatusOnConnection(player.Name, game);
                _unity.Complete();
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Name);

                if (AllPlayersAreConnected(game) && game.GS == SD.GS.GameHasNotStarted)
                {
                    PauseGame(1);
                    await GameAsync(game, SD.GS.DetermineHakem);
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task GameAsync(Game game, SD.GS gs)
        {
            game.GS = gs;
            _unity.Complete();

            if (gs == SD.GS.DetermineHakem)
            {
                await SelectHakemAsync(game);
            }
            else if (gs == SD.GS.HakemChooseHokm)
            {
                await HakemChoosingHokmAsync(game);
            }
            else if (gs == SD.GS.InTheMiddleOfGame)
            {
                await DisplayPlayerCardsAsync(game);
            }
        }
        public async Task SelectHakemAsync(Game game)
        {
            var cards = SD.CardsToDetermineHakem();
            await Clients.Group(game.Name).SendAsync("SpecifyHakem", cards);
            PauseGame(2);

            var hakemIndex = SD.SpecifyHakemIndex(cards);
            game.HakemIndex = hakemIndex;
            game.WhosTurnIndex = hakemIndex;
            game.GS = SD.GS.HakemChooseHokm;
            await ShowHakemAsync(game.Name, hakemIndex);
            PauseGame(2);

            await RemoveThrownCardsAsync(game.Name);
            await GameAsync(game, SD.GS.HakemChooseHokm);
        }
        public async Task ShowHakemAsync(string gameName, int hakemIndex)
        {
            await Clients.Group(gameName).SendAsync("ShowHakem", hakemIndex);
        }
        public async Task RemoveThrownCardsAsync(string gameName)
        {
            await Clients.Group(gameName).SendAsync("RemoveThrownCards");
        }
        public async Task HakemChoosingHokmAsync(Game game)
        {
            _unity.GameRepo.AssignPlayersCards(game);
            _unity.Complete();
            var hakemHokmInfo = GetHakemHokmInfo(game);
            await Clients.Client(hakemHokmInfo.HakemConnectionId).SendAsync("HakemChooseHokm", hakemHokmInfo.Cards);
            foreach (var connectionId in hakemHokmInfo.OtherPlayersConnectionId)
            {
                await Clients.Client(connectionId).SendAsync("HakemChoosingHokm");
            }
        }
        public async Task DisplayPlayerCardsAsync(Game game)
        {
            var blue1ConnectionId = game.Players.Where(p => p.Name == game.Blue1).Select(c => c.ConnectionId).FirstOrDefault();
            var red1ConnectionId = game.Players.Where(p => p.Name == game.Red1).Select(c => c.ConnectionId).FirstOrDefault();
            var blue2ConnectionId = game.Players.Where(p => p.Name == game.Blue2).Select(c => c.ConnectionId).FirstOrDefault();
            var red2ConnectionId = game.Players.Where(p => p.Name == game.Red2).Select(c => c.ConnectionId).FirstOrDefault();

            // Blue1
            await Clients.Client(blue1ConnectionId).SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Blue1Cards));

            // Red1
            await Clients.Client(red1ConnectionId).SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Red1Cards));

            // Blue2
            await Clients.Client(blue2ConnectionId).SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Blue2Cards));

            // Red2
            await Clients.Client(red2ConnectionId).SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Red2Cards));
        }
        public async Task DisplayWhosTurnAsync(Game game)
        {
            await Clients.Group(game.Name).SendAsync("WhosTurnIndex", game.WhosTurnIndex);
        }
        public async Task UpdateGameResultAsync(Game game)
        {
            await Clients.Group(game.Name).SendAsync("UpdateGameResult", game);
        }

        #region Receiving commands from client
        public async Task MessageReceived(string message, string gameName)
        {
            if (!string.IsNullOrEmpty(message.Trim()) && message.Length <= 1000)
            {
                var messageThread = new MessageThread
                {
                    From = Context.User.GetPlayerName(),
                    Message = message
                };

                await Clients.Group(gameName).SendAsync("NewMessageReceived", messageThread);
            }
        }
        public async Task HakemChoseHokmSuit(string gameName, string suit)
        {
            var game = _unity.GameRepo.FindByName(gameName, includeProperties: "Players,Blue1Cards,Red1Cards,Blue2Cards,Red2Cards");
            game.HokmSuit = suit;
            await Clients.Group(gameName).SendAsync("DisplayHokmSuitAndWhosTurnIndex", suit, game.WhosTurnIndex);
            await GameAsync(game, SD.GS.InTheMiddleOfGame);
        }
        public async Task PlayerPlayedTheCard(string gameName, string card)
        {
            var playerName = PlayerName();
            var game = _unity.GameRepo.FindByName(gameName, includeProperties: "Players,Blue1Cards,Red1Cards,Blue2Cards,Red2Cards");
            var playerIndex = GetPlayerIndex(game, playerName);

            if (playerIndex == game.WhosTurnIndex)
            {
                if (HandlePlayerPlayedTheCard(game, card, playerName, playerIndex))
                {
                    await Clients.Group(gameName).SendAsync("DisplayPlayedCard", card, playerIndex);
                    await Clients.Caller.SendAsync("RemovePlayerCardFromHand", card);
                    _unity.Complete();

                    if (playerIndex == FourthPLayerIndex(game.RoundStartsByIndex))
                    {
                        RoundCalculation(game);
                        await UpdateGameResultAsync(game);
                        _unity.Complete();

                        PauseGame(2);

                        if (EndOfRoundGameCheck(game))
                        {

                        }
                        else
                        {
                            await RemoveThrownCardsAsync(gameName);
                            await DisplayWhosTurnAsync(game);
                        }
                    }
                    else
                    {
                        await DisplayWhosTurnAsync(game);
                    }
                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ErrorCard", "Invalid card!");
                }
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorCard", "It is not your turn yet!");
            }
        }
        #endregion

        #region Private Helper Methods
        private string PlayerName()
        {
            return Context.User.GetPlayerName();
        }
        private string ConnectionId()
        {
            return Context.ConnectionId;
        }
        private void PauseGame(double sec)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(sec));
                return 42;
            });
            t.Wait();
        }
        private void UpdatePlayerStatusOnConnection(string playerName, Game game)
        {
            if (game.Blue1.Equals(playerName))
            {
                game.Blue1Status = SD.PlayerInGameStatus.Connected;
            }
            else if (game.Red1.Equals(playerName))
            {
                game.Red1Status = SD.PlayerInGameStatus.Connected;
            }
            else if (game.Blue2.Equals(playerName))
            {
                game.Blue2Status = SD.PlayerInGameStatus.Connected;
            }
            else if (game.Red2.Equals(playerName))
            {
                game.Red2Status = SD.PlayerInGameStatus.Connected;
            }
        }
        private bool AllPlayersAreConnected(Game game)
        {
            return (game.Blue1Status == SD.PlayerInGameStatus.Connected &&
                game.Red1Status == SD.PlayerInGameStatus.Connected &&
                game.Blue2Status == SD.PlayerInGameStatus.Connected &&
                game.Red2Status == SD.PlayerInGameStatus.Connected) ? true : false;
        }
        private HakemHokmInfoDto GetHakemHokmInfo(Game game)
        {
            Card cards;
            string hakemConnectionId = "";
            List<string> otherPlayersConnectionIds = new List<string>();

            if (game.HakemIndex == 1)
            {
                cards = game.Blue1Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Blue1).Select(x => x.ConnectionId).FirstOrDefault();
                otherPlayersConnectionIds.AddRange(game.Players.Where(p => p.Name != game.Blue1).Select(x => x.ConnectionId).ToList());
            }
            else if (game.HakemIndex == 2)
            {
                cards = game.Red1Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Red1).Select(x => x.ConnectionId).FirstOrDefault();
                otherPlayersConnectionIds.AddRange(game.Players.Where(p => p.Name != game.Red1).Select(x => x.ConnectionId).ToList());
            }
            else if (game.HakemIndex == 3)
            {
                cards = game.Blue2Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Blue2).Select(x => x.ConnectionId).FirstOrDefault();
                otherPlayersConnectionIds.AddRange(game.Players.Where(p => p.Name != game.Blue2).Select(x => x.ConnectionId).ToList());
            }
            else
            {
                cards = game.Red2Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Red2).Select(x => x.ConnectionId).FirstOrDefault();
                otherPlayersConnectionIds.AddRange(game.Players.Where(p => p.Name != game.Red2).Select(x => x.ConnectionId).ToList());
            }

            var firstFiveCards = new string[]
            {
                cards.Card1,
                cards.Card2,
                cards.Card3,
                cards.Card4,
                cards.Card5
            };

            return new HakemHokmInfoDto(hakemConnectionId, otherPlayersConnectionIds, firstFiveCards);
        }
        private int GetPlayerIndex(Game game, string playerName)
        {
            if (game != null)
            {
                if (game.Blue1 == playerName)
                {
                    return 1;
                }
                else if (game.Red1 == playerName)
                {
                    return 2;
                }
                else if (game.Blue2 == playerName)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }

            return 0;
        }
        public bool HandlePlayerPlayedTheCard(Game game, string card, string playerName, int playerIndex)
        {
            // if the played card by the player is the first round card
            if (game.RoundSuit == null)
            {
                game.RoundSuit = SD.GetSuitOfCard(card);
                SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(game, card, playerIndex);
                return true;
            }
            else
            {
                var playerHasRoundSuitCard = false;
                var playerCards = _unity.CardRepo.GetPlayerCardsAsList(game, playerName);
                foreach (var c in playerCards)
                {
                    if (SD.GetSuitOfCard(c).Equals(game.RoundSuit))
                    {
                        playerHasRoundSuitCard = true;
                    }
                }

                //  check if player has card of current round suit
                if (playerHasRoundSuitCard)
                {
                    // check if the thrown card is not the same kind as round suit
                    if (SD.GetSuitOfCard(card) != game.RoundSuit)
                    {
                        return false;
                    }
                }

                SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(game, card, playerIndex);

                return true;
            }
        }
        private void SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(Game game, string card, int playerIndex)
        {
            if (playerIndex == 1)
            {
                game.Blue1Card = card;
                game.WhosTurnIndex = 2;
                _unity.CardRepo.RemoveCardFromPlayerHand(game.Blue1Cards, card);
            }
            else if (playerIndex == 2)
            {
                game.Red1Card = card;
                game.WhosTurnIndex = 3;
                _unity.CardRepo.RemoveCardFromPlayerHand(game.Red1Cards, card);
            }
            else if (playerIndex == 3)
            {
                game.Blue2Card = card;
                game.WhosTurnIndex = 4;
                _unity.CardRepo.RemoveCardFromPlayerHand(game.Blue2Cards, card);
            }
            else
            {
                game.Red2Card = card;
                game.WhosTurnIndex = 1;
                _unity.CardRepo.RemoveCardFromPlayerHand(game.Red2Cards, card);
            }
        }
        private int FourthPLayerIndex(int roundStartIndex)
        {
            if (roundStartIndex == 1)
                return 4;
            else if (roundStartIndex == 2)
                return 1;
            else if (roundStartIndex == 3)
                return 2;
            else if (roundStartIndex == 4)
                return 3;

            return -1;
        }
        private void RoundCalculation(Game game)
        {
            int blueTotal = 0;
            int redTotal = 0;

            int blue1Value = GetValueOfPlayedCard(game.Blue1Card, game.RoundSuit, game.HokmSuit);
            int blue2Value = GetValueOfPlayedCard(game.Blue2Card, game.RoundSuit, game.HokmSuit);
            int red1Value = GetValueOfPlayedCard(game.Red1Card, game.RoundSuit, game.HokmSuit);
            int red2Value = GetValueOfPlayedCard(game.Red2Card, game.RoundSuit, game.HokmSuit);

            if (blue1Value > blue2Value)
            {
                blueTotal = blue1Value;
            }
            else
            {
                blueTotal = blue2Value;
            }

            if (red1Value > red2Value)
            {
                redTotal = red1Value;
            }
            else
            {
                redTotal = red2Value;
            }

            if (blueTotal > redTotal)
            {
                if (blue1Value > blue2Value)
                {
                    game.WhosTurnIndex = 1;
                    game.RoundStartsByIndex = 1;
                }
                else
                {
                    game.WhosTurnIndex = 3;
                    game.RoundStartsByIndex = 3;
                }
                game.BlueRoundScore++;
            }
            else
            {
                if (red1Value > red2Value)
                {
                    game.WhosTurnIndex = 2;
                    game.RoundStartsByIndex = 2;
                }
                else
                {
                    game.WhosTurnIndex = 4;
                    game.RoundStartsByIndex = 4;
                }
                game.RedRoundScore++;
            }

            game.NthCardIsPlaying++;

            // empty the cards
            game.Blue1Card = null;
            game.Red1Card = null;
            game.Blue2Card = null;
            game.Red2Card = null;
            game.RoundSuit = null;
        }

        private int GetValueOfPlayedCard(string card, string roundSuit, string hokmSuit)
        {
            int value = 0;

            if (SD.GetSuitOfCard(card).Equals(roundSuit))
            {
                value = SD.GetValueOfCard(card);
            }

            if (SD.GetSuitOfCard(card).Equals(hokmSuit))
            {
                value = SD.GetValueOfCard(card) + 13;
            }

            return value;
        }
        private bool EndOfRoundGameCheck(Game game)
        {
            if (game != null)
            {
                // Blue won the round game
                if (game.BlueRoundScore == 7)
                {
                    return true;
                }

                // Red won the round game
                if (game.RedRoundScore == 7)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
