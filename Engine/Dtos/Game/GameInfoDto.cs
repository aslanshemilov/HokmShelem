﻿namespace Engine.Dtos.Game
{
    public class GameInfoDto
    {
        public string GameName { get; set; }
        public string GameType { get; set; }
        public int TargetScore { get; set; }
        public string Blue1 { get; set; }
        public string Red1 { get; set; }
        public string Blue2 { get; set; }
        public string Red2 { get; set; }
        public SD.GS GS { get; set; }
        public int? HakemIndex { get; set; }
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

        public string MyPlayerName { get; set; }
        public int MyIndex { get; set; }
        public string[] MyCards { get; set; }

        public PlayersIndexDto PlayersIndex { get; set; }
    }
}
