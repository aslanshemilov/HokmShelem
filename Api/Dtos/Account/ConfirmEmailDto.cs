namespace Api.Dtos.Account
{
    public class ConfirmEmailDto
    {
        [Required]
        public string Token { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^.+@[^\\.].*\\.[a-z]{2,}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
