using Engine.Entities;

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
            var oldConnectionId = _unity.PlayerRepo.AddUpdatePlayer(player, ConnectionId());
            // handle duplicate browser connection ....
            if (!string.IsNullOrEmpty(oldConnectionId))
            {
                await Groups.RemoveFromGroupAsync(oldConnectionId, player.GameName);
                var notification = new NotificationMessage(SM.MultipleDeviceTitle, SM.MultipleDeviceMessage, false, false);
                await NotificationAsync(oldConnectionId, notification);
            }

            var game = _unity.GameRepo.FindByName(player.GameName, includeProperties: "Players");
            if (game != null)
            {
                _unity.GameRepo.UpdatePlayerStatusOfTheGame(game, player.Name, SD.PlayerInGameStatus.Connected);
                _unity.Complete();
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Name);

                if (AllPlayersAreConnected(game) && game.GS == SD.GS.GameHasNotStarted)
                {
                    PauseGame(1);
                    await GameAsync(game, SD.GS.DetermineTheFirstHakem);
                }
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = _unity.PlayerRepo.FindByName(Context.User.GetPlayerName(), includeProperties: "ConnectionTracker,Game");

            if (Context.ConnectionId.Equals(player.ConnectionTracker.OldId))
            {
                await Groups.RemoveFromGroupAsync(player.ConnectionTracker.OldId, player.GameName);
                player.ConnectionTracker.OldId = null;
                _unity.Complete();
            }
            else
            {
                if (!string.IsNullOrEmpty(player.GameName))
                {
                    _unity.GameRepo.UpdatePlayerStatusOfTheGame(player.Game, player.Name, SD.PlayerInGameStatus.Disconnected);
                    _unity.Complete();
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task GameAsync(Game game, SD.GS gs)
        {
            if (gs == SD.GS.DetermineTheFirstHakem)
            {
                await DetermineTheFirstHakemAsync(game);
            }
            else if (gs == SD.GS.HakemChooseHokm)
            {
                await HakemChoosingHokmAsync(game);
            }
            else if (gs == SD.GS.GameHasStarted)
            {
                await DisplayPlayerCardsAsync(game);
            }
        }
        public async Task DetermineTheFirstHakemAsync(Game game)
        {
            game.GS = SD.GS.DetermineTheFirstHakem;
            var cards = SD.CardsToDetermineTheFirstHakem();
            await Clients.Group(game.Name).SendAsync("DetermineTheFirstHakem", cards);

            var hakemIndex = SD.GetTheFirstHakemIndex(cards);
            _unity.GameRepo.UpdateGame(game, new GameUpdateDto(hakemIndex, hakemIndex, hakemIndex));
            _unity.Complete();
            PauseGame(3);

            await ShowHakemAsync(game.Name, hakemIndex);
            await GameAsync(game, SD.GS.HakemChooseHokm);
        }
        public async Task ShowHakemAsync(string gameName, int hakemIndex, bool roundGameEnded = false)
        {
            await Clients.Group(gameName).SendAsync("ShowHakem", SD.GS.HakemChooseHokm, hakemIndex, roundGameEnded);
        }
        public async Task HakemChoosingHokmAsync(Game game)
        {
            game.GS = SD.GS.HakemChooseHokm;
            if (game.Blue1Cards != null) _unity.CardRepo.RemovePlayerCards(game.Blue1);
            if (game.Red1Cards != null) _unity.CardRepo.RemovePlayerCards(game.Red1);
            if (game.Blue2Cards != null) _unity.CardRepo.RemovePlayerCards(game.Blue2);
            if (game.Red2Cards != null) _unity.CardRepo.RemovePlayerCards(game.Red2);
            _unity.Complete();

            _unity.GameRepo.AssignPlayersCards(game);
            _unity.Complete();
            var hakemCardsToHokm = GetHakemCardsToHokm(game);
            await Clients.Client(hakemCardsToHokm.HakemConnectionId).SendAsync("HakemChooseHokm", hakemCardsToHokm.Cards);
        }
        public async Task DisplayPlayerCardsAsync(Game game)
        {
            await Clients.Client(_unity.PlayerRepo.GetPlayerConnectionId(game.Blue1))
                .SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Blue1Cards));
            await Clients.Client(_unity.PlayerRepo.GetPlayerConnectionId(game.Red1))
                .SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Red1Cards));
            await Clients.Client(_unity.PlayerRepo.GetPlayerConnectionId(game.Blue2))
                .SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Blue2Cards));
            await Clients.Client(_unity.PlayerRepo.GetPlayerConnectionId(game.Red2))
                .SendAsync("DisplayMyCards", _unity.CardRepo.GetCardsAsList(game.Red2Cards));
        }
        public async Task DisplayWhosTurnAsync(string gameName, int whosTurnIndex)
        {
            await Clients.Group(gameName).SendAsync("WhosTurnIndex", whosTurnIndex);
        }
        public async Task NotificationAsync(string connectionId, NotificationMessage notification)
        {
            await Clients.Client(connectionId).SendAsync("NotificationMessage", notification);
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
            game.GS = SD.GS.GameHasStarted;
            _unity.GameRepo.UpdateGame(game, new GameUpdateDto(hs: suit));
            _unity.Complete();
            await Clients.Group(gameName).SendAsync("DisplayHokmSuit", SD.GS.GameHasStarted, suit, game.WhosTurnIndex);
            await GameAsync(game, SD.GS.GameHasStarted);
        }
        public async Task PlayerPlayedTheCard(string gameName, string card)
        {
            var playerName = PlayerName();
            var game = _unity.GameRepo.FindByName(gameName, includeProperties: "Players");
            var playerIndex = GetPlayerIndex(game, playerName);

            if (playerIndex == game.WhosTurnIndex)
            {
                if (HandlePlayerPlayedTheCard(game, card, playerName, playerIndex))
                {
                    await Clients.Group(gameName).SendAsync("DisplayPlayedCard", card, playerIndex);
                    await Clients.Caller.SendAsync("RemovePlayerPlayedCardFromHand", card);
                    _unity.Complete();

                    if (playerIndex == FourthPLayerIndex(game.RoundStartsByIndex))
                    {
                        RoundCalculation(game);
                        _unity.Complete();
                        PauseGame(2);
                        await Clients.Group(game.Name).SendAsync("UpdateRoundResult", game.BlueRoundScore, game.RedRoundScore);

                        if (_unity.GameRepo.EndOfRoundGame(game))
                        {
                            _unity.GameRepo.EmptyRoundCardsAndSuit(game);
                            _unity.Complete();
                            await Clients.Group(game.Name).SendAsync("UpdateTotalResult", game.BlueTotalScore, game.RedTotalScore);

                            if (_unity.GameRepo.EndOfTheGame(game))
                            {
                                // close the game
                            }
                            else
                            {
                                await ShowHakemAsync(game.Name, game.HakemIndex, true);
                                await GameAsync(game, SD.GS.HakemChooseHokm);
                            }
                        }
                        else
                        {
                            await DisplayWhosTurnAsync(game.Name, game.WhosTurnIndex);
                        }
                    }
                    else
                    {
                        await DisplayWhosTurnAsync(game.Name, game.WhosTurnIndex);
                    }
                }
                else
                {
                    await NotificationAsync(Context.ConnectionId, new NotificationMessage("Invalid card", isSuccess: false));
                }
            }
            else
            {
                await NotificationAsync(Context.ConnectionId, new NotificationMessage("Not your turn", isSuccess: false));
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
        private bool AllPlayersAreConnected(Game game)
        {
            return (game.Blue1Status == SD.PlayerInGameStatus.Connected &&
                game.Red1Status == SD.PlayerInGameStatus.Connected &&
                game.Blue2Status == SD.PlayerInGameStatus.Connected &&
                game.Red2Status == SD.PlayerInGameStatus.Connected) ? true : false;
        }
        private HakemCardsToHokm GetHakemCardsToHokm(Game game)
        {
            Card cards;
            string hakemConnectionId = "";

            if (game.HakemIndex == 1)
            {
                cards = game.Blue1Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Blue1);
            }
            else if (game.HakemIndex == 2)
            {
                cards = game.Red1Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Red1);
            }
            else if (game.HakemIndex == 3)
            {
                cards = game.Blue2Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Blue2);
            }
            else
            {
                cards = game.Red2Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Red2);
            }

            var firstFiveCards = new string[]
            {
                cards.Card1,
                cards.Card2,
                cards.Card3,
                cards.Card4,
                cards.Card5
            };

            return new HakemCardsToHokm(hakemConnectionId, firstFiveCards);
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
            var playerCards = _unity.CardRepo.FindByName(playerName);
            // if the played card by the player is the first round card
            if (game.RoundSuit == null)
            {
                game.RoundSuit = SD.GetSuitOfCard(card);
                SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(game, playerCards, card, playerIndex);
                return true;
            }
            else
            {
                var playerHasRoundSuitCard = false;
                var playerCardsAsList = _unity.CardRepo.GetCardsAsList(playerCards);

                for (int i = 0; i < playerCardsAsList.Count; i++)
                {
                    if (SD.GetSuitOfCard(playerCardsAsList[i]).Equals(game.RoundSuit))
                    {
                        playerHasRoundSuitCard = true;
                        i = playerCardsAsList.Count;
                    }
                }

                //  check if player has card of current round suit
                if (playerHasRoundSuitCard)
                {
                    // check if the played card is not the same kind as round suit
                    if (SD.GetSuitOfCard(card) != game.RoundSuit)
                    {
                        return false;
                    }
                }

                SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(game, playerCards, card, playerIndex);

                return true;
            }
        }
        private void SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(Game game, Card playerCards, string card, int playerIndex)
        {
            if (playerIndex == 1)
            {
                game.Blue1Card = card;
                game.WhosTurnIndex = 2;
            }
            else if (playerIndex == 2)
            {
                game.Red1Card = card;
                game.WhosTurnIndex = 3;
            }
            else if (playerIndex == 3)
            {
                game.Blue2Card = card;
                game.WhosTurnIndex = 4;
            }
            else
            {
                game.Red2Card = card;
                game.WhosTurnIndex = 1;
            }

            _unity.CardRepo.RemoveCardFromPlayerHand(playerCards, card);
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

            int blue1Value = GetValueOfPlayedCard(game, game.Blue1Card, game.RoundSuit, game.HokmSuit);
            int blue2Value = GetValueOfPlayedCard(game, game.Blue2Card, game.RoundSuit, game.HokmSuit);
            int red1Value = GetValueOfPlayedCard(game, game.Red1Card, game.RoundSuit, game.HokmSuit);
            int red2Value = GetValueOfPlayedCard(game,game.Red2Card, game.RoundSuit, game.HokmSuit);

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

            // empty the cards
            game.Blue1Card = null;
            game.Red1Card = null;
            game.Blue2Card = null;
            game.Red2Card = null;
            game.RoundSuit = null;
        }

        private int GetValueOfPlayedCard(Game game, string card, string roundSuit, string hokmSuit)
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
        #endregion
    }
}
