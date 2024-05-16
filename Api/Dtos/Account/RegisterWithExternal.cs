namespace Api.Dtos.Account
{
    public class RegisterWithExternal
    {
        [Required(ErrorMessage = "Player name is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Player name must be at least {2}, and maximum {1} charachters")]
        [RegularExpression("^[a-zA-Z0-9_.-]*$", ErrorMessage = "Player name must contain only a-z A-Z 0-9 charachters")]
        public string PlayerName { get; set; }
        [Required(ErrorMessage = "External User ID is required")]
        public string UserId
        {
            get => _userId;
            set => _userId = value.ToLower();
        }
        private string _userId;
        [Required]
        public string AccessToken { get; set; }

        private string _provider;
        [Required]
        public string Provider
        {
            get => _provider;
            set => _provider = value.ToLower();
        }

        [Required]
        public int CountryId { get; set; }
    }
}
