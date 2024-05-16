namespace Api.Dtos.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Player name is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Player name must be at least {2}, and maximum {1} charachters")]
        [RegularExpression("^[a-zA-Z0-9_.-]*$", ErrorMessage = "Player name must contain only a-z A-Z 0-9 charachters")]
        public string PlayerName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$", ErrorMessage = "Password must be at least 6 characters, no more than 15 characters, and must include at least one upper case letter, one lower case letter, and one numeric digit")]
        public string Password { get; set; }
        [Required]
        public int CountryId { get; set; }
    }
}
