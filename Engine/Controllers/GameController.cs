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
    }
}
