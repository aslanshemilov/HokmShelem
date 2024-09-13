namespace Api.Dtos.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Player name is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Player name must be at least {2}, and maximum {1} charachters")]
        [RegularExpression("^[a-zA-Z0-9_.-]*$", ErrorMessage = "Player name must contain only a-z A-Z 0-9 charachters")]
        public string PlayerName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^.+@[^\\.].*\\.[a-z]{2,}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        public bool AgeConsent { get; set; }
        [Required]
        public bool TermsOfService { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^.{6,15}$", ErrorMessage = "Password must be between 6-15 characters in length.")]
        public string Password { get; set; }
        [Required]
        public int CountryId { get; set; }
    }
}
