namespace Api.Dtos.Profile
{
    public class PlayerDto
    {
        public string PlayerName { get; set; }
        public string Badge { get; set; }
        public int Rate { get; set; }
        public int HokmScore { get; set; }
        public int ShelemScore { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int GamesLeft { get; set; }
        public string PhotoUrl { get; set; }
        public string Country { get; set; }
    }
}
