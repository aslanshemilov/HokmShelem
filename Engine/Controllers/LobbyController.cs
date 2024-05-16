namespace Engine.Controllers
{
    [Authorize]
    public class LobbyController : ApiCoreController
    {
        [HttpGet("roomname-taken")]
        public ActionResult<bool> RoomNameTaken([FromQuery] string roomName)
        {
            if (string.IsNullOrEmpty(roomName)) return BadRequest(new ApiResponse(400));
            return Unity.RoomRepo.AnyByName(roomName) || Unity.GameRepo.AnyByName(roomName);
        }
    }
}
