namespace Api.Controllers
{
    public class GuestController : ApiCoreController
    {
        private readonly IJWTService _jwtService;
        private ApiResult _response;

        public GuestController(IJWTService jwtService)
        {
            _jwtService = jwtService;
            _response = new ApiResult();
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUserDto>> RegisterAsGuest()
        {
            var guestToAdd = new Guest() { PlayerName = UniqueRandomName() };
            Context.Guest.Add(guestToAdd);
            await Context.SaveChangesAsync();

            return CreateGuestUserDto(guestToAdd);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApplicationUserDto>> RefreshGuestUser()
        {
            var playerName = User.GetPlayerName();
            var guest = await Context.Guest
                .Where(x => x.PlayerName == playerName)
                .FirstOrDefaultAsync();

            if (guest == null) return Unauthorized(new ApiResponse(401));
            return CreateGuestUserDto(guest);
        }

        [Authorize]
        [HttpGet("info")]
        public async Task<ActionResult<ApiResult>> GetPlayerInfo()
        {
            var playerName = User.GetPlayerName();
            var guest = await Context.Guest
                .Where(g => g.PlayerName == playerName)
                .FirstOrDefaultAsync();

            if (guest == null)
            {
                _response.IsSuccess = false;
                _response.Message = "guest was not found";
            }
            else
            {
                var result = new PlayerDto
                {
                    PlayerName = playerName,
                    Badge = SD.Pink,
                    Rate = 0,
                    HokmScore = 0,
                    ShelemScore = 0,
                    GamesWon = 0,
                    GamesLost = 0,
                    GamesLeft = 0,
                    PhotoUrl = Configuration["AzureBlobUrl"] + "site/user.png",
                    Country = "World",
                };

                _response.Result = result;
            }

            return _response;
        }

        #region Private Methods
        private ApplicationUserDto CreateGuestUserDto(Guest user)
        {
            return new ApplicationUserDto()
            {
                PlayerName = user.PlayerName,
                PhotoUrl = Configuration["AzureBlobUrl"] + "site/user.png",
                JWT = _jwtService.CreateGuestJWT(user)
            };
        }
        private string UniqueRandomName()
        {
            var randomeName = SD.GetRandomName();

            while (Context.Guest.Any(x => x.PlayerName.ToLower() == randomeName.ToLower()))
            {
                randomeName = SD.GetRandomName();
            }

            return randomeName;
        }
        #endregion
    }
}
