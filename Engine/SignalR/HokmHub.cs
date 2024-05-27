namespace Engine.SignalR
{
    [Authorize]
    public class HokmHub : Hub
    {
        private readonly IUnityRepo _unity;
        private readonly IGameTrackerService _tracker;
        private readonly IApiService _apiService;

        public HokmHub(IUnityRepo unity,
            IGameTrackerService tracker,
            IApiService apiService)
        {
            _unity = unity;
            _tracker = tracker;
            _apiService = apiService;
        }
        public override async Task OnConnectedAsync()
        {
            var player = _unity.PlayerRepo.FindByName(PlayerName());
            if (player != null)
            {
                var oldConnectionId = _unity.PlayerRepo.AddUpdatePlayer(player, ConnectionId());
                // handle duplicate browser connection ....
                if (!string.IsNullOrEmpty(oldConnectionId))
                {
                    await Groups.RemoveFromGroupAsync(oldConnectionId, player.GameName);
                    var notification = new NotificationMessage(SM.MultipleDeviceTitle, SM.MultipleDeviceMessage, false, false);
                    await NotificationAsync(oldConnectionId, notification);
                }

                var game = _unity.GameRepo.FindByName(player.GameName, includeProperties: "Players");
                if (game != null && _tracker.PlayerConnectedToGameTracker(game.Name, player.Name))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, game.Name);
                    _unity.GameRepo.UpdatePlayerStatusOfTheGame(game, player.Name, SD.PlayerInGameStatus.Connected);
                    _unity.Complete();
                    var connectedPlayers = _tracker.GetGameTrackerConnectedPlayers(game.Name);
                    if (connectedPlayers.Count == 4 && game.GS == SD.GS.GameHasNotStarted)
                    {
                        if (PlayerName().Equals(connectedPlayers.LastOrDefault()))
                        {
                            await AllPlayersConnected(game);
                        }
                    }
                }
            }
            else
            {
                await NotificationAsync(ConnectionId(), new NotificationMessage("Unexpected Error", "Please try again.", false, false));
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = _unity.PlayerRepo.FindByName(PlayerName(), includeProperties: "ConnectionTracker,Game");
            if (player != null && !string.IsNullOrEmpty(player.GameName))
            {
                _tracker.RemovePlayerFromGameTracker(player.GameName, player.Name);

                if (Context.ConnectionId.Equals(player.ConnectionTracker.OldId))
                {
                    await Groups.RemoveFromGroupAsync(player.ConnectionTracker.OldId, player.GameName);
                    player.ConnectionTracker.OldId = null;
                    _unity.Complete();
                }
                else
                {
                    _unity.GameRepo.UpdatePlayerStatusOfTheGame(player.Game, player.Name, SD.PlayerInGameStatus.Disconnected);
                    _unity.Complete();
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task ShowHakemAsync(string gameName, int hakemIndex, bool roundGameEnded = false)
        {
            await Clients.Group(gameName).SendAsync("ShowHakem", SD.GS.HakemChooseHokm, hakemIndex, roundGameEnded);
        }
        public async Task HakemGetsCardsToChooseHokmAsync(Game game)
        {
            game.GS = SD.GS.HakemChooseHokm;
            _unity.CardRepo.RemoveAllPlayersCardsFromTheGame(game);
            _unity.Complete();

            _unity.GameRepo.AssignPlayersCards(game);
            _unity.Complete();
            var hakemCardsToHokm = _unity.GameRepo.GetHakemCardsToHokm(game);
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
        public async Task AllPlayersConnected(Game game)
        {
            game.GS = SD.GS.DetermineTheFirstHakem;
            var cards = SD.CardsToDetermineTheFirstHakem();
            var hakemIndex = SD.GetTheFirstHakemIndex(cards);
            _unity.GameRepo.UpdateGame(game, new GameUpdateDto(hakemIndex, hakemIndex, hakemIndex));
            game.GS = SD.GS.HakemChooseHokm;
            _unity.Complete();

            await Clients.Group(game.Name).SendAsync("DetermineTheFirstHakem", SD.GS.DetermineTheFirstHakem, cards);
            PauseGame(3);
            await ShowHakemAsync(game.Name, hakemIndex);
            await HakemGetsCardsToChooseHokmAsync(game);
        }
        public async Task HakemChoseHokmSuit(string gameName, string suit)
        {
            var game = _unity.GameRepo.FindByName(gameName, includeProperties: "Players,Blue1Cards,Red1Cards,Blue2Cards,Red2Cards");
            game.GS = SD.GS.RoundGameStarted;
            _unity.GameRepo.UpdateGame(game, new GameUpdateDto(hs: suit));
            _unity.Complete();
            await Clients.Group(gameName).SendAsync("DisplayHokmSuit", SD.GS.RoundGameStarted, suit, game.WhosTurnIndex);
            await DisplayPlayerCardsAsync(game);
        }
        public async Task PlayerPlayedTheCard(string gameName, string card)
        {
            var playerName = PlayerName();
            var game = _unity.GameRepo.FindByName(gameName, includeProperties: "Players");
            var playerIndex = _unity.GameRepo.GetPlayerIndex(game, playerName);

            if (playerIndex == game.WhosTurnIndex)
            {
                if (_unity.GameRepo.HandlePlayerPlayedTheCard(game, card, playerName, playerIndex))
                {
                    await Clients.Group(gameName).SendAsync("DisplayPlayedCard", card, playerIndex);
                    await Clients.Caller.SendAsync("RemovePlayerPlayedCardFromHand", card);
                    _unity.Complete();

                    if (playerIndex == SD.FourthPLayerIndex(game.RoundStartsByIndex))
                    {
                        _unity.GameRepo.RoundCalculation(game);
                        _unity.Complete();
                        PauseGame(2);
                        await Clients.Group(game.Name).SendAsync("UpdateRoundResult", game.BlueRoundScore, game.RedRoundScore);
                        var newHakemIndex = _unity.GameRepo.GetNewHakemIndex(game);

                        if (newHakemIndex > 0)
                        {
                            _unity.GameRepo.ResetRoundGame(game, newHakemIndex);
                            _unity.Complete();
                            await Clients.Group(game.Name).SendAsync("UpdateTotalResult", game.BlueTotalScore, game.RedTotalScore);

                            var winnerTeam = _unity.GameRepo.EndOfTheGame(game);
                            if (!string.IsNullOrEmpty(winnerTeam))
                            {
                                _unity.Complete();
                                await Clients.Group(game.Name).SendAsync("EndOfTheGame", winnerTeam);
                                await _apiService.CreateGameHistoryAsync(GetGameHistory(game));
                            }
                            else
                            {
                                await ShowHakemAsync(game.Name, game.HakemIndex, true);
                                await HakemGetsCardsToChooseHokmAsync(game);
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
        public async Task LeaveTheGame(string gameName, string playerName)
        {
            var game = _unity.GameRepo.FindByName(gameName, includeProperties: "Players");
            await _apiService.CreateGameHistoryAsync(GetGameHistory(game, SD.Left, playerName));
            _unity.GameRepo.CloseTheGame(game);
            _unity.Complete();
            await Clients.Group(gameName).SendAsync("PlayerLeftTheGame", playerName);
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
        private GameHistory GetGameHistory(Game game, string status = null, string leftBy = null)
        {
            return new GameHistory()
            {
                GameType = game.GameType,
                TargetScore = game.TargetScore,
                Blue1 = game.Blue1,
                Red1 = game.Red1,
                Blue2 = game.Blue2,
                Red2 = game.Red2,
                Status = string.IsNullOrEmpty(status) ? SD.Completed : status,
                Winner = string.IsNullOrEmpty(status) ? (game.RedTotalScore >= game.TargetScore ? SD.Red : SD.Blue) : null,
                LeftBy = leftBy
            };
        }
        #endregion
    }
}
