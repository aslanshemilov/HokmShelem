namespace Engine.Controllers
{
    [Authorize]
    public class LobbyController : ApiCoreController
    {
        //[AllowAnonymous]
        //[HttpGet("join-as-guest")]
        //public ActionResult<string> JoinAsGuest()
        //{

           
        //}

        [HttpGet("roomname-taken")]
        public ActionResult<bool> RoomNameTaken([FromQuery] string roomName)
        {
            if (string.IsNullOrEmpty(roomName)) return BadRequest(new ApiResponse(400));
            return Unity.RoomRepo.AnyByName(roomName) || Unity.GameRepo.AnyByName(roomName);
        }
    }
}
