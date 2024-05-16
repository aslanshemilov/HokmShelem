namespace Engine.Dtos
{
    public class RoomToJoinDto
    {
        public string RoomName { get; set; }
        public string GameType { get; set; }
        public int TargetScore { get; set; }
        public string HostName { get; set; }
        public DateTime Created { get; set; }
        public List<string> Players { get; set; }
    }
    public class RoomDto
    {
        public string RoomName { get; set; }
        public string HostName { get; set; }
        public string GameType { get; set; }
        public int TargetScore { get; set; }
        public string Blue1 { get; set; }
        public string Red1 { get; set; }
        public string Blue2 { get; set; }
        public string Red2 { get; set; }
        public bool Blue1Ready { get; set; }
        public bool Red1Ready { get; set; }
        public bool Blue2Ready { get; set; }
        public bool Red2Ready { get; set; }
        public bool ReadyButtonEnabled { get; set; } = false;
        public DateTime Created { get; set; }
        public List<PlayerDto> Players { get; set; }
    }
}
