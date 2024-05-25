namespace Api.Services
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SymmetricSecurityKey _jwtKey;

        public JWTService(IConfiguration config,
            UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
        }

        public async Task<string> CreateJWTAsync(AppUserToGenerateJWTDto userDto)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.ApplicationUserId.ToString()),
                new Claim(ClaimTypes.Name, userDto.UserName),
                new Claim(ClaimTypes.GivenName, userDto.PlayerName),
                new Claim(ClaimTypes.Email, userDto.Email ?? ""),
                new Claim(ClaimTypes.Sid, userDto.UserProfileId.ToString()),
            };

            var user = await _userManager.FindByNameAsync(userDto.UserName);
            var roles = await _userManager.GetRolesAsync(user);
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return GenerateJWT(userClaims);
        }
        public string CreateGuestJWT(Guest guest)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, guest.PlayerName),
            };

            return GenerateJWT(userClaims);
        }

        private string GenerateJWT(List<Claim> userClaims)
        {
            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["JWT:ExpiresInMinutes"])),
                SigningCredentials = credentials,
                Issuer = _config["JWT:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwt);
        }
    }
}
