namespace Engine.Entities
{
    // Chart of the Game
    // -----------------
    //
    //              3 (Blue2)
    // 4 (Red2)                       2 (Red1)
    //              1 (Blue1)          
    public class Room : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string GameType { get; set; }
        public int TargetScore { get; set; }
        public string HostName { get; set; }
        public string Blue1 { get; set; }
        public string Red1 { get; set; }
        public string Blue2 { get; set; }
        public string Red2 { get; set; }
        public bool Blue1Ready { get; set; }
        public bool Red1Ready { get; set; }
        public bool Blue2Ready { get; set; }
        public bool Red2Ready { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
