using Engine.Entities;

namespace Engine.Repository
{
    public class GameRepo : BaseRepo<Game>, IGameRepo
    {
        private readonly Context _context;
        private readonly IUnityRepo _unity;
        private readonly IMapper _mapper;

        public GameRepo(IUnityRepo unity,
            Context context,
            IMapper mapper) : base(context)
        {
            _context = context;
            _unity = unity;
            _mapper = mapper;
        }
        public string GetGameName(string playerName)
        {
            return _context.Game
                .Where(x => x.Players.Select(p => p.Name).Contains(playerName))
                .Select(x => x.Name)
                .FirstOrDefault();
        }

        public GameInfoDto GetGameInfo(string gameName, string playerName)
        {
            var gameInfo = _context.Game
               .Where(x => x.Name == gameName)
               .ProjectTo<GameInfoDto>(_mapper.ConfigurationProvider)
               .SingleOrDefault();

            if (gameInfo == null) return null;

            gameInfo.MyPlayerName = playerName;
            gameInfo.PlayersIndex = GetIndex(gameInfo, playerName);

            if (gameInfo.Blue1 == playerName)
            {
                gameInfo.MyIndex = 1;
            }
            else if (gameInfo.Red1 == playerName)
            {
                gameInfo.MyIndex = 2;
            }
            else if (gameInfo.Blue2 == playerName)
            {
                gameInfo.MyIndex = 3;
            }
            else
            {
                gameInfo.MyIndex = 4;
            }

            if (gameInfo.GS == SD.GS.HakemChooseHokm)
            {
                Card cards = null;

                if (gameInfo.HakemIndex == 1)
                {
                    if (gameInfo.Blue1 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Blue1);
                    }
                }
                else if (gameInfo.HakemIndex == 2)
                {
                    if (gameInfo.Red1 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Red1);
                    }
                }
                else if (gameInfo.HakemIndex == 3)
                {
                    if (gameInfo.Blue2 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Blue2);
                    }
                }
                else if (gameInfo.HakemIndex == 4)
                {
                    if (gameInfo.Red2 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Red2);
                    }
                }

                if (cards != null)
                {
                    var firstFiveCards = new string[]
                    {
                        cards.Card1,
                        cards.Card2,
                        cards.Card3,
                        cards.Card4,
                        cards.Card5
                    };

                    gameInfo.MyCards = firstFiveCards;
                }
            }

            return gameInfo;
        }
        public void AssignPlayersCards(Game game)
        {
            var deckOfCard = SD.GetShuffledDeckOfCards();
            List<string> cards = new List<string>();

            for (int i = 0; i < 13; i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Blue1Cards = _unity.CardRepo.SetPlayerCards(game.Blue1, cards);
            cards.Clear();

            for (int i = 13; i < 26; i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Red1Cards = _unity.CardRepo.SetPlayerCards(game.Red1, cards);
            cards.Clear();

            for (int i = 26; i < 39; i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Blue2Cards = _unity.CardRepo.SetPlayerCards(game.Blue2, cards);
            cards.Clear();

            for (int i = 39; i < deckOfCard.Count; i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Red2Cards = _unity.CardRepo.SetPlayerCards(game.Red2, cards);
        }

        #region Private Methods
        private PlayersIndexDto GetIndex(GameInfoDto game, string playerName)
        {
            PlayersIndexDto index;
            // 1 2 3 4 -> 
            // 1 2 3 4
            if (game.Blue1 == playerName)
            {
                index = new PlayersIndexDto(1, 2, 3, 4);
            }
            // 2 3 4 1 ->  
            // 1 2 3 4
            else if (game.Red1 == playerName)
            {
                index = new PlayersIndexDto(4, 1, 2, 3);
            }
            // 3 4 1 2 -> 
            // 1 2 3 4
            else if (game.Blue2 == playerName)
            {
                index = new PlayersIndexDto(3, 4, 1, 2);
            }
            // 4 1 2 3 ->
            // 1 2 3 4
            else
            {
                index = new PlayersIndexDto(2, 3, 4, 1);
            }
            return index;
        }
        #endregion
    }
}
