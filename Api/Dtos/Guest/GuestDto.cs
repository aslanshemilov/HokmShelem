namespace Api.Dtos.Guest
{
    public class GuestDto
    {
        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Guest name must be at least {2}, and maximum {1} charachters")]
        public string GuestName { get; set; }
    }
}
