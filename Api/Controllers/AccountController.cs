namespace Api.Controllers
{
    public class AccountController : ApiCoreController
    {
        private readonly IJWTService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        private readonly HttpClient _facebookHttpClient;

        public AccountController(
            IJWTService jwtService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager
            )
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _facebookHttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com")
            };
        }

        [Authorize]
        [HttpGet("refresh-applicationUser")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshApplicationUser()
        {
            var user = await _userManager.FindByNameAsync(User.GetUserName());
            if (user == null) return Unauthorized(new ApiResponse(401));

            if (_userManager.IsLockedOutAsync(user).GetAwaiter().GetResult())
            {
                return Unauthorized(new ApiResponse(401, title: SM.AccountLockedTitle, message: SM.AccountLockedMessage(user.LockoutEnd.Value.DateTime), isHtmlEnabled: true));
            }

            return await CreateApplicationUserDtoAsync(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.UserName);
            }

            if (user == null) return Unauthorized(new ApiResponse(401, message: SM.InvalidUsernameOrPassword));

            if (user.EmailConfirmed == false) return Unauthorized(new ApiResponse(401, message: SM.ConfirmYourEmail));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsLockedOut)
            {
                return Unauthorized(new ApiResponse(401, title: SM.AccountLockedTitle, message: SM.AccountLockedMessage(user.LockoutEnd.Value.DateTime), isHtmlEnabled: true));
            }

            if (!result.Succeeded)
            {
                if (user.LockoutEnabled)
                {
                    await _userManager.AccessFailedAsync(user);
                }
                
                return Unauthorized(new ApiResponse(401, message: SM.InvalidUsernameOrPassword));
            }

            await ResetUserLockoutAsync(user);
            return await CreateApplicationUserDtoAsync(user);
        }

        [HttpPost("login-with-third-party")]
        public async Task<ActionResult<ApplicationUserDto>> LoginWithThirdParty(LoginWithExternal model)
        {
            if (model.Provider.Equals(SD.Facebook))
            {
                try
                {
                    if (!FacebookValidatedAsync(model.AccessToken, model.UserId).GetAwaiter().GetResult())
                    {
                        return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByFacebook));
                    }
                }
                catch (Exception)
                {
                    return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByFacebook));
                }
            }
            else if (model.Provider.Equals(SD.Google))
            {
                try
                {
                    if (string.IsNullOrEmpty(GoogleValidatedAsync(model.AccessToken, model.UserId).GetAwaiter().GetResult()))
                    {
                        return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByGoogle));
                    }
                }
                catch (Exception)
                {
                    return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByGoogle));
                }
            }
            else
            {
                return BadRequest(new ApiResponse(400, message: SM.InvalidProvider));
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x=> x.UserName == model.UserId && x.Provider == model.Provider);
            if (user == null) return Unauthorized(new ApiResponse(401, message: SM.AccountNotFound));

            if (_userManager.IsLockedOutAsync(user).GetAwaiter().GetResult())
            {
                return Unauthorized(new ApiResponse(401, title: SM.AccountLockedTitle, message: SM.AccountLockedMessage(user.LockoutEnd.Value.DateTime), isHtmlEnabled: true));
            }

            await ResetUserLockoutAsync(user);
            return await CreateApplicationUserDtoAsync(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (model.AgeConsent == false)
            {
                return BadRequest(new ApiResponse(400, message: "We're sorry, but you must be at least 16 years old to sign up for this website."));
            }

            if (model.TermsOfService == false)
            {
                return BadRequest(new ApiResponse(400, message: "In order to create an account, you must agree to our terms of service."));
            }

            if (await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest(new ApiResponse(400, message: SM.EmailTaken(model.Email)));
            }

            if (await CheckUsernameExistsAsync(model.PlayerName))
            {
                return BadRequest(new ApiResponse(400, message: SM.PlayerNameTaken(model.PlayerName)));
            }

            var userToAdd = await CreateApplicationUserAsync(Mapper.Map<ApplicationUserAddDto>(model));
            if (userToAdd == null) return BadRequest(new ApiResponse(400, message: SM.AccountCreationFailureMessage));

            if (await SendConfirmEmailAsync(userToAdd))
            {
                return Ok(new ApiResponse(200, title: SM.ConfirmationEmailSent, message: SM.AccountCreatedConfirmEmail));
            }
            else
            {
                return BadRequest(new ApiResponse(400, message: SM.EmailSentFailureMessage));
            }
        }

        [HttpPost("register-with-third-party")]
        public async Task<ActionResult<ApplicationUserDto>> RegisterWithThirdParty(RegisterWithExternal model)
        {
            string email = null;

            if (model.Provider.Equals(SD.Facebook))
            {
                try
                {
                    if (!FacebookValidatedAsync(model.AccessToken, model.UserId).GetAwaiter().GetResult())
                    {
                        return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByFacebook));
                    }
                }
                catch (Exception)
                {
                    return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByFacebook));
                }
            }
            else if (model.Provider.Equals(SD.Google))
            {
                try
                {
                    email = GoogleValidatedAsync(model.AccessToken, model.UserId).GetAwaiter().GetResult();
                    if (string.IsNullOrEmpty(email))
                    {
                        return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByGoogle));
                    }
                }
                catch (Exception)
                {
                    return Unauthorized(new ApiResponse(401, message: SM.UnauthorizedByGoogle));
                }
            }
            else
            {
                return BadRequest(new ApiResponse(400, SM.InvalidProvider));
            }

            if (await CheckPlayernameExistsAsync(model.PlayerName))
            {
                return BadRequest(new ApiResponse(400, message: SM.PlayerNameTaken(model.PlayerName)));
            }

            if (await CheckUsernameExistsAsync(model.UserId))
            {
                return BadRequest(new ApiResponse(400, message: SM.DuplicateRegistrationWithExternal(model.Provider)));
            }

            var applicationUserAdd = Mapper.Map<ApplicationUserAddDto>(model);
            applicationUserAdd.Email = email;

            var userToAdd = await CreateApplicationUserAsync(applicationUserAdd);
            if (userToAdd == null) return BadRequest(new ApiResponse(400, message: SM.AccountCreationFailureMessage));

            return await CreateApplicationUserDtoAsync(userToAdd);
        }

        [HttpGet("playername-taken")]
        public async Task<ActionResult<bool>> PlayerNameTaken([FromQuery] string playerName)
        {
            return await CheckPlayernameExistsAsync(playerName);
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ApiResponse(401, message: SM.UnregisteredEmailMessage));

            if (user.EmailConfirmed == true) return BadRequest(new ApiResponse(400, message: SM.EmailConfirmerBefore));

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    return Ok(new ApiResponse(200, title: SM.AccountActivationSuccess, message: SM.EmailConfirmed));
                }

                return BadRequest(new ApiResponse(401, message: SM.InvalidTokenMessage));
            }
            catch (Exception)
            {
                return BadRequest(new ApiResponse(401, message: SM.InvalidTokenMessage));
            }
        }

        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfimrationLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized(new ApiResponse(401, title: SM.NotRegistered, message: SM.UnregisteredEmailMessage));

            if (user.EmailConfirmed == true) return BadRequest(new ApiResponse(400, message: SM.EmailConfirmerBefore));

            if (await SendConfirmEmailAsync(user))
            {
                return Ok(new ApiResponse(200, title: SM.EmailSent, message: SM.ConfirmationEmailLinkSent));
            }
            else
            {
                return BadRequest(new ApiResponse(400, message: SM.EmailSentFailureMessage));
            }
        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized(new ApiResponse(401, message: SM.UnregisteredEmailMessage));

            if (!string.IsNullOrEmpty(user.Provider)) return BadRequest(new ApiResponse(400, title: $"Use your {user.Provider}", message: $"You have an account with us. Please try logging in using your {user.Provider} account." ));

            if (user.EmailConfirmed == false) return BadRequest(new ApiResponse(400, message: SM.ConfirmYourEmail));

            if (await SendForgotUsernameOrPasswordEmail(user))
            {
                return Ok(new ApiResponse(200, title: SM.EmailSent, message: SM.PasswordResetEmailLinkSent));
            }

            return BadRequest(new ApiResponse(400, message: SM.EmailSentFailureMessage));
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ApiResponse(401, message: SM.UnregisteredEmailMessage));

            if (user.EmailConfirmed == false) return BadRequest(new ApiResponse(400, message: SM.ConfirmYourEmail));

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new ApiResponse(200, message: SM.PasswordHasBeenReset));
                }

                return BadRequest(new ApiResponse(400, message: SM.InvalidTokenMessage));
            }
            catch (Exception)
            {
                return BadRequest(new ApiResponse(400, message: SM.InvalidTokenMessage));
            }
        }

        #region Private Helper Methods
        private async Task<ApplicationUser> CreateApplicationUserAsync(ApplicationUserAddDto model)
        {
            var userToAdd = new ApplicationUser
            {
                UserName = model.PlayerName.ToLower(),
                PlayerName = model.PlayerName,
                Email = model.Email
            };

            IdentityResult result;

            if (string.IsNullOrEmpty(model.UserId))
            {
                userToAdd.LockoutEnabled = true;
                result = await _userManager.CreateAsync(userToAdd, model.Password);
            }
            else
            {
                userToAdd.UserName = model.UserId;
                userToAdd.Provider = model.Provider;
                userToAdd.LockoutEnabled = true;
                userToAdd.EmailConfirmed = true;
                result = await _userManager.CreateAsync(userToAdd);
            }
            
            if (!result.Succeeded) return null;
            result = await _userManager.AddToRoleAsync(userToAdd, SD.Player);
            if (!result.Succeeded) return null;

            model.ApplicationUserId = userToAdd.Id;
            await UnitOfWork.UserProfileRepository.CreateUserProfileAsync(model);
            await UnitOfWork.CompleteAsync();

            return userToAdd;
        }
        private async Task<ApplicationUserDto> CreateApplicationUserDtoAsync(ApplicationUser user)
        {
            var userDto = await _userManager.Users
              .Where(x => x.Id == user.Id)
              .ProjectTo<AppUserToGenerateJWTDto>(Mapper.ConfigurationProvider)
              .SingleOrDefaultAsync();

      

            return new ApplicationUserDto
            {
                PlayerName = userDto.PlayerName,
                PhotoUrl = userDto.PhotoUrl,
                JWT = await _jwtService.CreateJWTAsync(userDto)
            };
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
        private async Task<bool> CheckPlayernameExistsAsync(string playerName)
        {
            return await _userManager.Users.AnyAsync(x => x.PlayerName.ToLower() == playerName.ToLower());
        }
        private async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
        private async Task<bool> FacebookValidatedAsync(string accessToken, string userId)
        {
            var facebookKeys = Configuration["Facebook:AppId"] + "|" + Configuration["Facebook:AppSecret"];
            var fbResult = await _facebookHttpClient.GetFromJsonAsync<FacebookResultDto>($"debug_token?input_token={accessToken}&access_token={facebookKeys}");

            if (fbResult == null || fbResult.Data.Is_Valid == false || !fbResult.Data.User_Id.Equals(userId))
            {
                return false;
            }

            return true;
        }
        private async Task<string> GoogleValidatedAsync(string accessToken, string userId)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);

            if (!payload.Audience.Equals(Configuration["Google:ClientId"]))
            {
                return null;
            }

            if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
            {
                return null;
            }

            if (payload.ExpirationTimeSeconds == null)
            {
                return null;
            }

            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
            if (now > expiration)
            {
                return null;
            }

            if (!payload.Subject.Equals(userId))
            {
                return null;
            }

            if (string.IsNullOrEmpty(payload.Email)) return null;

            return payload.Email;
        }

        private async Task<bool> SendConfirmEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{Configuration["JWT:ClientUrl"]}/{Configuration["Email:ConfrimEmailPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.PlayerName}</p>" +
                $"<p>Please confirm your email address by clicking on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                $"<p>{Configuration["Email:ApplicationName"]}</p>";

            var emailSend = new EmailSendDto(user.Email, "Confirm your email", body);

            return await EmailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{Configuration["JWT:ClientUrl"]}/{Configuration["Email:ResetPasswordPath"]}?token={token}&email={user.Email}&username={user.UserName}";

            var body = $"<p>Hello: {user.PlayerName}</p>" +
                $"<p>Username: {user.UserName}</p>" +
                $"<p>In order to reset your password, please click on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                $"<p>{Configuration["Email:ApplicationName"]}</p>";

            var emailSend = new EmailSendDto(user.Email, "Forgot username or password", body);

            return await EmailService.SendEmailAsync(emailSend);
        }

        private async Task ResetUserLockoutAsync(ApplicationUser user)
        {
            if (user.LockoutEnd != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }

            if (user.AccessFailedCount > 0)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }
        }
        #endregion
    }
}
