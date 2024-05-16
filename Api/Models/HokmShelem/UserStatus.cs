namespace Api.Models.HokmShelem
{
    [Table("UserStatus", Schema = "Setting")]
    public class UserStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserProfile> UserProfiles { get; set; }
    }
}