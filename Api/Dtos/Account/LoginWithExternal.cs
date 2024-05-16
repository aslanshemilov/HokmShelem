namespace Api.Dtos.Account
{
    public class LoginWithExternal
    {
        [Required(ErrorMessage = "External User ID is required")]
        public string UserId
        {
            get => _userId;
            set => _userId = value.ToLower();
        }
        private string _userId;
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string Provider
        {
            get => _provider;
            set => _provider = value.ToLower();
        }
        private string _provider;
    }
}
