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
        public const int HokmEndOfRoundScore = 1;
        public enum GS // GS -> GameStage
        {
            GameHasNotStarted,        // 0
            DetermineTheFirstHakem,   // 1
            HakemChooseHokm,          // 2
            GameHasStarted            // 3
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
        public static int GetTheFirstHakemIndex(List<string> cards)
        {
            var max = GetValueOfCard(cards.ElementAt(0));
            int hakemIndex = 0;
            for (int i = 1; i < cards.Count; i++)
            {
                if (GetValueOfCard(cards.ElementAt(i)) > max)
                {
                    max = GetValueOfCard(cards.ElementAt(i));
                    hakemIndex = i;
                }
            }

            return hakemIndex + 1;
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
    }
}
