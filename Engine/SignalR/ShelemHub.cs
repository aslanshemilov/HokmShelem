using Engine.Dtos;
using Engine.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Numerics;

namespace Engine.SignalR
{
    [Authorize]
    public class ShelemHub : Hub
    {
        private readonly IUnityRepo _unity;
        private readonly IGameTrackerService _tracker;
        private readonly IApiService _apiService;

        public ShelemHub(IUnityRepo unity,
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

                if (!string.IsNullOrEmpty(oldConnectionId))
                {
                    await Groups.RemoveFromGroupAsync(oldConnectionId, player.GameName);
                    var notification = new NotificationMessage(SM.MultipleDeviceTitle, SM.MultipleDeviceMessage, false, false);
                    await NotificationAsync(notification, oldConnectionId);
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
                            game.GS = SD.GS.DetermineTheInitiator;
                            var cards = SD.CardsToDetermineTheFirstHakem();
                            int claimStartsByIndex = SD.GetTheFirstInitiatorIndex(cards);
                            _unity.GameRepo.UpdateGame(game, new GameUpdateDto(claimStartsByIndex: claimStartsByIndex, whosTurnIndex: claimStartsByIndex));
                            _unity.Complete();
                            await Clients.Group(game.Name).SendAsync("DetermineTheInitiator", SD.GS.DetermineTheInitiator, cards);
                            PauseGame(2);
                            await ShelemStartsAsync(game);
                        }
                    }
                }
            }
            else
            {
                await NotificationAsync(new NotificationMessage("Unexpected Error", "Please try again.", false, false), ConnectionId());
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = _unity.PlayerRepo.FindByName(PlayerName(), includeProperties: "ConnectionTracker,Game");
            if (player != null && !string.IsNullOrEmpty(player.GameName))
            {
                if (Context.ConnectionId.Equals(player.ConnectionTracker.OldId))
                {
                    await Groups.RemoveFromGroupAsync(player.ConnectionTracker.OldId, player.GameName);
                    player.ConnectionTracker.OldId = null;
                    _unity.Complete();
                }
                else
                {
                    if (_tracker.GameTrackerHasPlayer(player.GameName, player.Name))
                    {
                        _tracker.RemovePlayerFromGameTracker(player.GameName, player.Name);
                        _unity.GameRepo.UpdatePlayerStatusOfTheGame(player.Game, player.Name, SD.PlayerInGameStatus.Disconnected);
                        _unity.Complete();
                        await NotificationAsync(new NotificationMessage($"{player.Name} has disconnected. Please wait for him or her to reconnect.", null, false, true),
                            gameName: player.GameName);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task ShelemStartsAsync(Game game)
        {
            await Clients.Group(game.Name).SendAsync("ShelemStarts", SD.GS.DetermineTheInitiator);
            _unity.GameRepo.AssignPlayersCards(game);
            _unity.Complete();
            await DisplayPlayerCardsAsync(game);
            await PlayerClaimsPointAsync(game, game.ClaimStartsByIndex, 105);
        }
        public async Task ShowHakemAsync(string gameName, int hakemIndex, bool roundGameEnded = false)
        {
            await Clients.Group(gameName).SendAsync("ShowHakem", SD.GS.HakemChooseHokm, hakemIndex, roundGameEnded);
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
        public async Task PlayerClaimsPointAsync(Game game, int whosTurnIndex, int lastClaimedPoint)
        {
            await Clients.Group(game.Name).SendAsync("PlayerClaimsPoint", whosTurnIndex, lastClaimedPoint);
        }
        public async Task DisplayWhosTurnAsync(string gameName, int whosTurnIndex)
        {
            await Clients.Group(gameName).SendAsync("WhosTurnIndex", whosTurnIndex);
        }
        public async Task NotificationAsync(NotificationMessage notification, string connectionId = null, string gameName = null)
        {
            if (!string.IsNullOrEmpty(connectionId))
            {
                await Clients.Client(connectionId).SendAsync("NotificationMessage", notification);
            }
            else
            {
                await Clients.Group(gameName).SendAsync("NotificationMessage", notification);
            }
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

        public async Task PlayerClaimedPoint(string gameName, int playerIndex, int point)
        {
            await Clients.Group(gameName).SendAsync("DisplayClaimedPoint", playerIndex, point);
            var game = _unity.GameRepo.FindByName(gameName);
            var result = _unity.GameRepo.PlayerClaimsPoint(game, playerIndex, point);
            _unity.Complete();
            if (result == 0) // Claim continues
            {
                int nextAvailablePoint = SD.LastMaxClaimPoint(game.Blue1Claimed, game.Red1Claimed, game.Blue2Claimed, game.Red2Claimed);
                await PlayerClaimsPointAsync(game, game.WhosTurnIndex, nextAvailablePoint == 0 ? 105 : nextAvailablePoint + 5);
            }
            else if (result == -1) // All players passed
            {
                _unity.GameRepo.ResetShelem(game);
                _unity.Complete();
                await ShelemStartsAsync(game);
            }
            else
            {
                game.GS = SD.GS.HakemChooseHokm;
                var hakemIndex = result;
                _unity.GameRepo.UpdateGame(game, new GameUpdateDto(hakemIndex: hakemIndex, whosTurnIndex: hakemIndex, roundStartsByIndex: hakemIndex));
                _unity.Complete();
                await ShowHakemAsync(game.Name, hakemIndex);
                var hakemAdditionalCards = _unity.CardRepo.GetFirstOrDefault(x => x.Name.Equals(game.Name + "_hakem"));
                var hakemAdditionalCardsAsList = _unity.CardRepo.GetCardsAsList(hakemAdditionalCards);
                var hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(_unity.GameRepo.GetPlayerNameByIndex(game, hakemIndex));
                await Clients.Client(hakemConnectionId).SendAsync("HakemGetsHakemCards", hakemAdditionalCardsAsList);
            }
        }
        public async Task HakemChoseHokmSuit(string gameName, string suit)
        {
            var game = _unity.GameRepo.FindByName(gameName);
            _unity.GameRepo.UpdateGame(game, new GameUpdateDto(hokmSuit: suit));
            _unity.Complete();
            await Clients.Group(gameName).SendAsync("DisplayHokmSuitShelem", suit);
        }
        public async Task HakemPutDownCards(string gameName, List<string> selectedCards)
        {
            var game = _unity.GameRepo.FindByName(gameName);
            int s = _unity.GameRepo.ShelemUpdateHakemCards(game, selectedCards);
            _unity.Complete();
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
                                _tracker.RemoveGameTracker(gameName);
                                _unity.Complete();
                                await Clients.Group(game.Name).SendAsync("EndOfTheGame", winnerTeam);
                                await _apiService.CreateGameHistoryAsync(SD.GetGameHistory(game));
                            }
                            else
                            {
                                await ShowHakemAsync(game.Name, game.HakemIndex, true);
                                //await HakemGetsCardsToChooseHokmAsync(game);
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
                    await NotificationAsync(new NotificationMessage("Invalid card", isSuccess: false), Context.ConnectionId);
                }
            }
            else
            {
                await NotificationAsync(new NotificationMessage("Not your turn", isSuccess: false), Context.ConnectionId);
            }
        }
        public async Task LeaveTheGame(string gameName, string playerName)
        {
            var game = _unity.GameRepo.FindByName(gameName, includeProperties: "Players");
            await _apiService.CreateGameHistoryAsync(SD.GetGameHistory(game, SD.Left, playerName));
            _tracker.RemoveGameTracker(gameName);
            _unity.GameRepo.CloseTheGame(game);
            _unity.Complete();

            foreach (var player in game.Players)
            {
                if (!player.Name.Equals(playerName))
                {
                    await Clients.Client(player.ConnectionId).SendAsync("PlayerLeftTheGame", playerName);
                }
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
        #endregion
    }
}
