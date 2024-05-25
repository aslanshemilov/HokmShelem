namespace Api.Models
{
    public class Guest
    {
        public int Id { get; set; }
        [Required]
        public string PlayerName { get; set; }
        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;
    }
}
