namespace Api.Dtos.Admin
{
    public class A_MemberViewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PlayerName { get; set; }
        public string Email { get; set; }
        public string Provider { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
        public string UserProfileStatusName { get; set; }
        public DateTime AccountCreated { get; set; }
        public DateTime? LastActive { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
