namespace Api.Dtos.GameHistory
{
    public class GameHistoryDto
    {
        [Required] public string GameType { get; set; }
        [Required] public int TargetScore { get; set; }
        [Required] public string Blue1 { get; set; }
        [Required] public string Red1 { get; set; }
        [Required] public string Blue2 { get; set; }
        [Required] public string Red2 { get; set; }
        [Required] public string Winner { get; set; }
        [Required] public string Status { get; set; }
    }
}
