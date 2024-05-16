namespace Api.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Required]
        public string PhotoUrl { get; set; }

        [ForeignKey("UserProfile")]
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
