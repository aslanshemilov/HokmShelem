namespace Api.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string AboutMe { get; set; }
        public int Rate { get; set; } = 0;
        public int HokmScore { get; set; } = 0;
        public int ShelemScore { get; set; } = 0;
        public int GamesWon { get; set; } = 0;
        public int GamesLost { get; set; } = 0;
        public int GamesLeft { get; set; } = 0;
        public int TournomenstWon { get; set; } = 0;

        [ForeignKey("ApplicationUser")]
        public int UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int BadgeId { get; set; }
        public Badge Badge { get; set; }

        [ForeignKey("Country")]
        public int? CountryId { get; set; }
        public Country Country { get; set; }

        [ForeignKey("UserStatus")]
        public int StatusId { get; set; }
        public UserStatus Status { get; set; }

        public Photo Photo { get; set; }
    }

}

