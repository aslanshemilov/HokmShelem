namespace Engine.Entities
{
    // Chart of the Game
    // -----------------
    //
    //              3 (Blue2)
    // 4 (Red2)                       2 (Red1)
    //              1 (Blue1)          
    public class Game : BaseEntity
    {
        [Required]
        [StringLength(10)]
        public string GameType { get; set; }
        public int TargetScore { get; set; }
        public string Blue1 { get; set; }
        public string Red1 { get; set; }
        public string Blue2 { get; set; }
        public string Red2 { get; set; }
        public SD.GS GS { get; set; } = SD.GS.GameHasNotStarted;
        public int HakemIndex { get; set; }
        public string HokmSuit { get; set; }
        public string RoundSuit { get; set; }
        public string Blue1Card { get; set; }
        public string Red1Card { get; set; }
        public string Blue2Card { get; set; }
        public string Red2Card { get; set; }
        public int RedRoundScore { get; set; }
        public int BlueRoundScore { get; set; }
        public int RedTotalScore { get; set; }
        public int BlueTotalScore { get; set; }
        public int WhosTurnIndex { get; set; }
        public int RoundStartsByIndex { get; set; }
        public int RoundTargetScore { get; set; }
        public int ClaimStartsByIndex { get; set; }
        public int Blue1Claimed { get; set; }
        public int Red1Claimed { get; set; }
        public int Blue2Claimed { get; set; }
        public int Red2Claimed { get; set; }

        public SD.PlayerInGameStatus Blue1Status { get; set; }
        public SD.PlayerInGameStatus Red1Status { get; set; }
        public SD.PlayerInGameStatus Blue2Status { get; set; }
        public SD.PlayerInGameStatus Red2Status { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();

        // Cards
        [ForeignKey("Blue1Cards")]
        public string Blue1CardsName { get; set; }
        public Card Blue1Cards { get; set; }

        [ForeignKey("Red1Cards")]
        public string Red1CardsName { get; set; }
        public Card Red1Cards { get; set; }

        [ForeignKey("Blue2Cards")]
        public string Blue2CardsName { get; set; }
        public Card Blue2Cards { get; set; }

        [ForeignKey("Red2Cards")]
        public string Red2CardsName { get; set; }
        public Card Red2Cards { get; set; }

        [ForeignKey("HakemCards")]
        public string HakemCardsName { get; set; }
        public Card HakemCards { get; set; }
    }
}
