namespace Api.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        public string PlayerName { get; set; }
        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;
        public DateTime? LastActive { get; set; }
        public string Provider { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
