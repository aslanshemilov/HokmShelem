namespace Api.Dtos.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,15}$", ErrorMessage = "Password must contain at least one letter, at least one number, and be between 6-15 characters in length with no special characters.")]
        public string NewPassword { get; set; }
    }
}
