namespace Engine.Utility
{
    public static class SD
    {
        public const string HSLobby = "hs";
        public const string Hokm = "hokm";
        public const string Shelem = "shelem";
        public const int HokmMinScore = 3;
        public const int HokmMaxScore = 7;
        public const int ShelemMinScore = 300;
        public const int ShelemMaxScore = 1200;
        public const int HokmEndOfRoundScore = 7;
        public const int ShelemMinRoundClaim = 100;
        public const int ShelemMaxRoundClaim = 165;

        // Game history status
        public const string Completed = "completed";
        public const string Left = "left";
        public const string VotedToEnd = "voted to end";
        public const string Blue = "blue";
        public const string Red = "red";

        public enum GS // GS -> GameStage
        {
            GameHasNotStarted,        // 0
            DetermineTheInitiator,    // 1
            HakemChooseHokm,          // 2
            RoundGameStarted          // 3
        }
        public enum PlayerInGameStatus
        {
            NotYetConnected,
            Connected,
            Disconnected,
            VoteToEnd,
            Left
        }

        public static List<string> GetShuffledDeckOfCards()
        {
            Random rnd = new Random();
            var cards = new List<string>();
            HashSet<int> randomNumbers = new HashSet<int>();

            List<string> deckOfCard = new List<string>()
            {
                // 1 = 2, 2 = 3 ... 10 = Jack, 11 = Queen, 12 = King, 13 = Ace
                "1-h", "2-h", "3-h", "4-h", "5-h", "6-h", "7-h", "8-h", "9-h", "10-h", "11-h", "12-h", "13-h",
                "1-d", "2-d", "3-d", "4-d", "5-d", "6-d", "7-d", "8-d", "9-d", "10-d", "11-d", "12-d", "13-d",
                "1-c", "2-c", "3-c", "4-c", "5-c", "6-c", "7-c", "8-c", "9-c", "10-c", "11-c", "12-c", "13-c",
                "1-s", "2-s", "3-s", "4-s", "5-s", "6-s", "7-s", "8-s", "9-s", "10-s", "11-s", "12-s", "13-s"
            };

            while (randomNumbers.Count < 52)
            {
                randomNumbers.Add(rnd.Next(0, 52));
            }                

            foreach (var index in randomNumbers)
            {
                cards.Add(deckOfCard.ElementAt(index));
            }

            return cards;
        }
        public static List<string> CardsToDetermineTheFirstHakem()
        {
            List<string> DeckOfCard = new List<string>()
            {
                "1-s", "2-s", "3-s", "4-s", "5-s", "6-s", "7-s", "8-s", "9-s", "10-s", "11-s", "12-s", "13-s"
            };

            Random rnd = new Random();
            var cards = new List<string>();
            HashSet<int> randomNumbers = new HashSet<int>();

            while (randomNumbers.Count < 4)
            {
                randomNumbers.Add(rnd.Next(0, 13));
            }  

            foreach (var index in randomNumbers)
            {
                cards.Add(DeckOfCard.ElementAt(index));
            } 

            return cards;
        }
        public static int GetTheFirstInitiatorIndex(List<string> cards)
        {
            List<int> values = new List<int>();
            foreach(var card in cards)
            {
                values.Add(GetValueOfCard(card));
            }

            int hakemIndex = GetMaxValue(values);

            return hakemIndex + 1;
        }
        public static int GetMaxValue(List<int> values)
        {
            int max = values[0];
            int maxIndex = 0;

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }
        public static int GetValueOfCard(string card)
        {
            return Convert.ToInt32(card.Split('-').First());
        }
        public static string GetSuitOfCard(string card)
        {
            if (card != null)
            {
                return card.Split('-').Last();
            }
            return null;
        }
        public static int FourthPLayerIndex(int roundStartIndex)
        {
            if (roundStartIndex == 1) return 4;
            else if (roundStartIndex == 2) return 1;
            else if (roundStartIndex == 3) return 2;
            else if (roundStartIndex == 4) return 3;
            else return -1;
        }
        public static GameHistory GetGameHistory(Game game, string status = null, string leftBy = null)
        {
            return new GameHistory()
            {
                GameType = game.GameType,
                TargetScore = game.TargetScore,
                Blue1 = game.Blue1,
                Red1 = game.Red1,
                Blue2 = game.Blue2,
                Red2 = game.Red2,
                Status = string.IsNullOrEmpty(status) ? Completed : status,
                Winner = string.IsNullOrEmpty(status) ? (game.RedTotalScore >= game.TargetScore ? Red : Blue) : null,
                LeftBy = leftBy
            };
        }
        public static int LastMaxClaimPoint(int blue1Claimed, int red1Claimed, int blue2Claimed, int red2Claimed)
        {
            return Math.Max(Math.Max(blue1Claimed, red1Claimed), Math.Max(blue2Claimed, red2Claimed));
        }
        public static int GetNextIndex(int currentIndex)
        {
            if (currentIndex == 1) return 2;
            else if (currentIndex == 2) return 3;
            else if (currentIndex == 3) return 4;
            else return 1;
        }
        public static int GetShelemRoundScore(string card1, string card2, string card3, string card4)
        {
            int score = 5;
            int value = 0;
            value = GetValueOfCard(card1);
            if (value == 4) score += 5;
            if (value == 9 || value == 13) score += 10;

            value = GetValueOfCard(card2);
            if (value == 4) score += 5;
            if (value == 9 || value == 13) score += 10;

            value = GetValueOfCard(card3);
            if (value == 4) score += 5;
            if (value == 9 || value == 13) score += 10;

            value = GetValueOfCard(card4);
            if (value == 4) score += 5;
            if (value == 9 || value == 13) score += 10;

            return score;
        }
    }
}
