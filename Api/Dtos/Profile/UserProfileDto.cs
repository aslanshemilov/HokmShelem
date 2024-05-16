namespace Api.Dtos.Profile
{
    public class UserProfileDto
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
        public string PlayerName { get; set; }
        public DateTime  AccountCreated { get; set; }
        public string BadgeColor { get; set; }
        public string CountryName { get; set; }
    }
}
