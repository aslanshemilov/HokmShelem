namespace Api.Dtos.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [RegularExpression("^.+@[^\\.].*\\.[a-z]{2,}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^.{6,15}$", ErrorMessage = "Password must be between 6-15 characters in length.")]
        public string NewPassword { get; set; }
    }
}
