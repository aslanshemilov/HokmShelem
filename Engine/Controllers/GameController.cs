namespace Engine.Controllers
{
    [Authorize]
    public class GameController : ApiCoreController
    {
        private readonly IHubContext<HokmHub> _hokmHub;
        private readonly IGameTrackerService _tracker;
        private readonly IApiService _apiService;

        public GameController(IHubContext<HokmHub> hokmHub,
             IGameTrackerService tracker,
             IApiService apiService)
        {
            _hokmHub = hokmHub;
            _tracker = tracker;
            _apiService = apiService;
        }

        [HttpGet]
        public ActionResult<GameInfoDto> GetGameInfo()
        {
            var playerName = User.GetPlayerName();
            var currentGame = Unity.GameRepo.GetCurrentGame(playerName);
            var gameInfo = Unity.GameRepo.GetGameInfo(currentGame.Name, playerName);

            return gameInfo == null ? BadRequest(new ApiResponse(400, "Game not found", "The requested game was not found")) : gameInfo;
        }

        [AllowAnonymous]
        [HttpGet("homepage-info")]
        public ActionResult<HomePageInfoDto> GetHomeInfo()
        {
            var activePlayersCount = Unity.PlayerRepo.Count(p => p.LobbyName != null || p.GameName != null || p.RoomName != null);
            var gamesCount = Unity.GameRepo.Count();
            return Ok(new HomePageInfoDto(activePlayersCount, gamesCount));
        }

        [HttpGet("current-game")]
        public ActionResult<CurrentGameDto> GetCurrentGame()
        {
            var currentGame = Unity.GameRepo.GetCurrentGame(User.GetPlayerName());
            return Ok(currentGame);
        }

        [HttpGet("leave-the-game-api/{gameName}")]
        public async Task<IActionResult> LeaveTheGameApi(string gameName)
        {
            var playerName = User.GetPlayerName();

            var game = Unity.GameRepo.FindByName(gameName, includeProperties: "Players");
            if (game == null) return BadRequest(new ApiResponse(400, message: "Game was not found", displayByDefault: true));
            await _apiService.CreateGameHistoryAsync(SD.GetGameHistory(game, SD.Left, playerName));
            _tracker.RemoveGameTracker(gameName);
            Unity.GameRepo.CloseTheGame(game);
            Unity.Complete();

            foreach (var player in game.Players)
            {
                if (!player.Name.Equals(playerName) && !string.IsNullOrEmpty(player.ConnectionId))
                {
                    await _hokmHub.Clients.Client(player.ConnectionId).SendAsync("PlayerLeftTheGame", playerName);
                }
            }

            return Ok();
        }
    }
}
