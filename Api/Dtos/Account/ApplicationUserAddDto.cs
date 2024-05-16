namespace Api.Dtos.Account
{
    public class ApplicationUserAddDto
    {
        public string PlayerName { get; set; }
        public string Email
        {
            get => _email;
            set => _email = value != null ? value.ToLower() : null;
        }
        private string _email;
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public string UserId { get; set; }
        public string Provider { get; set; }
        public int ApplicationUserId { get; set; }
        public int CountryId { get; set; }
    }
}
