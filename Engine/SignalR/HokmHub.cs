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
        }
        public async Task SelectHakemAsync(Game game)
        {
            var cards = SD.CardsToDetermineHakem();
            await Clients.Group(game.Name).SendAsync("SpecifyHakem", cards);
            PauseGame(2);

            var hakemIndex = SD.SpecifyHakemIndex(cards);
            game.HakemIndex = hakemIndex;
            game.GS = SD.GS.HakemChooseHokm;
            await ShowHakemAsync(game.Name, hakemIndex);
            PauseGame(2);

            await RemoveThrownCardsAsync(game.Name);
            await GameAsync(game,SD.GS.HakemChooseHokm);
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
            var hakemCardsToChooseHokm = GetHakemCardsToChooseHokm(game);
            await Clients.Client(hakemCardsToChooseHokm.ConnectionId).SendAsync("HakemChooseHokm", hakemCardsToChooseHokm.Cards);
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
        //public async Task HakemChoseHokmSuit(string gameName, string suit)
        //{
        //    var game = await _hokmEngine.GetGameModelAsync(gameName);
        //    await _hokmEngine.SetHokmAsync(gameName, suit);
        //    await Clients.Group(gameName).SendAsync("DisplayHokmSuitAndWhosTurnIndex", suit, game.WhosTurnIndex);
        //    await SetGameStage(gameName, GameStage.InTheMiddleOfGame);
        //    await Game(gameName, GameAction.DisplayPlayerCards);
        //}
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
        private HakemToChooseHokmDto GetHakemCardsToChooseHokm(Game game)
        {
            Card cards;
            string hakemConnectionId = "";

            if (game.HakemIndex == 1)
            {
                cards = game.Blue1Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Blue1).Select(x => x.ConnectionId).FirstOrDefault();
            }
            else if (game.HakemIndex == 2)
            {
                cards = game.Red1Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Red1).Select(x => x.ConnectionId).FirstOrDefault();
            }
            else if (game.HakemIndex == 3)
            {
                cards = game.Blue2Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Blue2).Select(x => x.ConnectionId).FirstOrDefault();
            }
            else
            {
                cards = game.Red2Cards;
                hakemConnectionId = game.Players.Where(p => p.Name == game.Red2).Select(x => x.ConnectionId).FirstOrDefault();
            }

            var firstFiveCards = new string[]
            {
                cards.Card1,
                cards.Card2,
                cards.Card3,
                cards.Card4,
                cards.Card5
            };

            return new HakemToChooseHokmDto(hakemConnectionId, firstFiveCards);
        }
        #endregion
    }
}
