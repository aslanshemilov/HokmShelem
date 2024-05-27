namespace Api.Models
{
    public class GameHistory
    {
        public int Id { get; set; }
        public string GameType { get; set; }
        public int TargetScore { get; set; }
        public string Blue1 { get; set; }
        public string Red1 { get; set; }
        public string Blue2 { get; set; }
        public string Red2 { get; set; }
        public string Winner { get; set; }
        public string Status { get; set; }
        public string LeftBy { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
