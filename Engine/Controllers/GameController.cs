namespace Engine.Controllers
{
    [Authorize]
    public class GameController : ApiCoreController
    {
        [HttpGet]
        public ActionResult<GameInfoDto> GetGameInfo()
        {
            var gameName = Unity.GameRepo.GetGameName(User.GetPlayerName());
            var gameInfo = Unity.GameRepo.GetGameInfo(gameName, User.GetPlayerName());

            return gameInfo == null ? NotFound() : gameInfo;
        }

        [AllowAnonymous]
        [HttpGet("homepage-info")]
        public ActionResult<HomePageInfoDto> GetHomeInfo()
        {
            var activePlayersCount = Unity.PlayerRepo.Count(p => p.LobbyName != null || p.GameName != null || p.RoomName != null);
            var gamesCount = Unity.GameRepo.Count();
            return Ok(new HomePageInfoDto(activePlayersCount, gamesCount));
        }
    }
}
