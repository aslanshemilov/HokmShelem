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
                    var firstFiveCards = new List<string>()
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
            else if (gameInfo.GS == SD.GS.GameHasStarted)
            {
                Card cards = null;
                if (gameInfo.MyIndex == 1)
                {
                    if (gameInfo.Blue1 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Blue1);
                    }
                }
                else if (gameInfo.MyIndex == 2)
                {
                    if (gameInfo.Red1 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Red1);
                    }
                }
                else if (gameInfo.MyIndex == 3)
                {
                    if (gameInfo.Blue2 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Blue2);
                    }
                }
                else if (gameInfo.MyIndex == 4)
                {
                    if (gameInfo.Red2 == playerName)
                    {
                        cards = _unity.CardRepo.GetFirstOrDefault(x => x.Name == gameInfo.Red2);
                    }
                }

                gameInfo.MyCards = _unity.CardRepo.GetCardsAsList(cards);
            }

            return gameInfo;
        }
        public void UpdatePlayerStatusOfTheGame(Game game, string playerName, SD.PlayerInGameStatus status)
        {
            if (game.Blue1.Equals(playerName))
            {
                game.Blue1Status = status;
            }
            else if (game.Red1.Equals(playerName))
            {
                game.Red1Status = status;
            }
            else if (game.Blue2.Equals(playerName))
            {
                game.Blue2Status = status;
            }
            else if (game.Red2.Equals(playerName))
            {
                game.Red2Status = status;
            }
        }
        public void UpdateGame(Game game, GameUpdateDto model)
        {
            if (model.HakemIndex > 0)
            {
                game.HakemIndex = model.HakemIndex;
            }

            if (model.WhosTurnIndex > 0)
            {
                game.WhosTurnIndex = model.WhosTurnIndex;
            }

            if (model.RoundStartsByIndex > 0)
            {
                game.RoundStartsByIndex = model.RoundStartsByIndex;
            }

            if (!string.IsNullOrEmpty(model.HokmSuit))
            {
                game.HokmSuit = model.HokmSuit;
            }
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
        public bool EndOfRoundGame(Game game)
        {
            // Blue won the round game
            if (game.BlueRoundScore == SD.HokmEndOfRoundScore)
            {
                game.BlueTotalScore++;
                game.BlueRoundScore = 0;

                if (game.HakemIndex == 2)
                {
                    game.HakemIndex = 3;
                }

                if (game.HakemIndex == 4)
                {
                    game.HakemIndex = 1;
                }

                game.RoundStartsByIndex = game.HakemIndex;
                game.WhosTurnIndex = game.HakemIndex;

                return true;
            }

            // Red won the round game
            if (game.RedRoundScore == SD.HokmEndOfRoundScore)
            {
                game.RedTotalScore++;
                game.RedRoundScore = 0;

                if (game.HakemIndex == 3)
                {
                    game.HakemIndex = 4;
                }

                if (game.HakemIndex == 1)
                {
                    game.HakemIndex = 2;
                }

                game.RoundStartsByIndex = game.HakemIndex;
                game.WhosTurnIndex = game.HakemIndex;

                return true;
            }

            return false;
        }
        public bool EndOfTheGame(Game game)
        {
            if (game.BlueTotalScore == game.TargetScore)
            {
                return true;
            }

            if (game.RedTotalScore == game.TargetScore)
            {
                return true;
            }

            return false;
        }

        public void EmptyRoundCardsAndSuit(Game game)
        {
            game.Blue1Card = null;
            game.Red1Card = null;
            game.Blue2Card = null;
            game.Red2Card = null;
            game.RoundSuit = null;
            game.HokmSuit = null;
        }
    }
}
