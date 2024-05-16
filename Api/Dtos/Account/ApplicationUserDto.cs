namespace Api.Dtos.Account
{
    public class ApplicationUserDto
    {
        public string PlayerName { get; set; }
        public string UserName { get; set; }
        public string PhotoUrl { get; set; }
        public string JWT { get; set; }
    }
    public class AppUserToGenerateJWTDto : ApplicationUserDto
    {
        public int ApplicationUserId { get; set; }
        public int UserProfileId { get; set; }
        public string Email { get; set; }
    }
}
