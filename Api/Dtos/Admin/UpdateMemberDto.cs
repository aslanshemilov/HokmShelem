namespace Api.Dtos.Admin
{
    public class UpdateMemberDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PlayerName { get; set; }
        public string Provider { get; set; }

        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} charachters")]
        public string NewPassword { get; set; }
        public bool EmailConfirme { get; set; }
    }
}
