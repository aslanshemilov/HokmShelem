namespace Api.Dtos.Profile
{
    public class ChangePasswordDto
    {
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} charachters")]
        public string NewPassword { get; set; }
    }
}
