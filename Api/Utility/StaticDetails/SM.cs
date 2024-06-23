using Mailjet.Client.Resources;

namespace StaticDetail
{
    public static class SM
    {
        // Messages
        #region Success
        public const string AccountCreatedConfirmEmail = "Your account has been created, and a confirmation email has been sent. Please look into you email inbox or spam folder to confirm your email address. ";
        public const string ConfirmationEmailLinkSent = "A confirmation link has been sent. Please log in to your email and confirm your address.";
        public const string PasswordHasBeenReset = "Your password has been reset";
        public const string EmailConfirmed = "Your email address has been confirmed, and your account has been activated. You can now log in and enjoy your stay.";
        public const string PasswordResetEmailLinkSent = "The password reset link has been sent. Please log in to your email and reset your password.";
        #endregion

        #region Error
        public const string UnregisteredEmailMessage = "This email address has not been registered yet.";
        public const string InvalidTokenMessage = "Invalid token. Please try again.";
        public const string EmailSentFailureMessage = "Failed to send email. Please try again or contact the admin of the site.";
        public const string AccountCreationFailureMessage = "Failed to create an account. Please try again or contact the admin of the site.";
        public const string InvalidUsernameOrPassword = "Invalid username or password. Please try again.";
        public const string ConfirmYourEmail = "Please login to your email and confirm your email address first.";
        public const string AccountNotFound = "Unable to find your account. Please try to register.";
        public const string UnauthorizedByFacebook = "You are not authorized by Facebook.";
        public const string UnauthorizedByGoogle = "You are not authorized by Google.";
        public const string InvalidProvider = "Invalid Provider. Please contact the admin of the site.";
        public const string AnErrorOccuredContactAdmin = "An error occured. Please contact the admin of the site.";
        public const string EmailConfirmerBefore = "Your email has already been confirmed. Please log in to your account.";
        public const string InternalServerError = "Something unexpected went wrong. Please contact the admin of the site.";

        public static string DuplicateRegistrationWithExternal(string provider)
        {
            return $"You already have an account. Please log in with your {provider} account.";
        }
        public static string PlayerNameTaken(string playerName)
        {
            return $"The player name of '{playerName}' is taken. Please try another name.";
        }
        public static string EmailTaken(string email)
        {
            return $"An account has been registered with {email}. Please try using another email address";
        }
        public static string AccountLockedMessage(DateTime endDate)
        {
            var timeDiff = SD.GetTimeDifferenceFromToday(endDate);
            return $"Your account is temporary locked.<br>You should wait {timeDiff.Days} day(s), {timeDiff.Hours} hour(s) and {timeDiff.Minutes} minute(s)";
        }
        #endregion

        // Title
        public const string ConfirmationEmailSent = "Confirmation Email Sent";
        public const string EmailSent = "Email Sent";
        public const string AccountLockedTitle = "Account LOCKED";
        public const string NotRegistered = "Not Registered";
        public const string AccountActivationFailure = "Account Activation Failure";
        public const string AccountActivationSuccess = "Account Activated";
    }
}
