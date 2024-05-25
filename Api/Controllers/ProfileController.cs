namespace Api.Controllers
{
    [Authorize(Roles = "admin,player,moderator")]
    public class ProfileController : ApiCoreController
    {
        private ApiResult _response;

        public ProfileController()
        {
            _response = new ApiResult();
        }

        [HttpGet("my-profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            return await UnitOfWork.UserProfileRepository.GetUserProfileAsync(User.GetProfileId());
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateDto model)
        {
            var userProfile = await UnitOfWork.UserProfileRepository.GetUserProfileToUpdateAsync(User.GetProfileId());
            if (userProfile == null) return NotFound();

            Mapper.Map(model, userProfile);

            if (await UnitOfWork.CompleteAsync()) return NoContent();

            return BadRequest("Failed to update user profile");
        }

        [HttpGet("player-info")]
        public async Task<ActionResult<ApiResult>> GetPlayerInfo()
        {
            var player = await UnitOfWork.UserProfileRepository.GetPlayerInfoAsync(User.GetProfileId());
            if (player == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Player was not found";
            }
            else
            {
                _response.Result = player;
            }

            return _response;
        }
    }
}
