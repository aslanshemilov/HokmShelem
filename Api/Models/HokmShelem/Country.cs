namespace Api.Models.HokmShelem
{
    [Table("Country", Schema = "Setting")]
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserProfile> UserProfiles { get; set; }
    }
}
