namespace Api.Models.HokmShelem
{
    [Table("Badge", Schema = "Setting")]
    public class Badge
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public ICollection<UserProfile> UserProfiles { get; set; }
    }
}
