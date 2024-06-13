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

        public void CreateGame(Room room)
        {
            var gameToAdd = new Game
            {
                Name = room.Name,
                GameType = room.GameType,
                TargetScore = room.TargetScore,
                Blue1 = room.Blue1,
                Red1 = room.Red1,
                Blue2 = room.Blue2,
                Red2 = room.Red2,
            };

            foreach (var player in room.Players)
            {
                player.Game = gameToAdd;
                player.RoomName = null;
            }
            _context.Game.Add(gameToAdd);
        }
        public CurrentGameDto GetCurrentGame(string playerName)
        {
            return _context.Game
                .Where(x => x.Players.Select(p => p.Name).Contains(playerName))
                .ProjectTo<CurrentGameDto>(_mapper.ConfigurationProvider)
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

            if (gameInfo.GS == SD.GS.HakemChooseHokm && gameInfo.GameType == SD.Hokm)
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
            else if ((gameInfo.GameType == SD.Hokm && gameInfo.GS == SD.GS.RoundGameStarted) || (gameInfo.GameType == SD.Shelem))
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

            if (gameInfo.GameType == SD.Shelem)
            {
                if (gameInfo.GS == SD.GS.DetermineTheInitiator)
                {
                    var nextAvailablePoint = SD.LastMaxClaimPoint(gameInfo.Blue1Claimed, gameInfo.Red1Claimed, gameInfo.Blue2Claimed, gameInfo.Red2Claimed);
                    gameInfo.NextAvailablePoint = nextAvailablePoint == 0 ? SD.ShelemMinRoundClaim : nextAvailablePoint + 5;
                }
                else if (gameInfo.GS == SD.GS.HakemChooseHokm && GetPlayerNameByIndex(gameInfo, gameInfo.HakemIndex).Equals(playerName))
                {
                    var hakemCards = _unity.CardRepo.GetFirstOrDefault(x => x.Name.Equals(gameInfo.GameName + "_hakem"));
                    gameInfo.MyCards.AddRange(_unity.CardRepo.GetCardsAsList(hakemCards));
                }

                gameInfo.BlueRoundScore = 0;
                gameInfo.RedRoundScore = 0;
            }

            return gameInfo;
        }
        public bool AllPlayersAreConnected(Game game)
        {
            return (game.Blue1Status == SD.PlayerInGameStatus.Connected &&
                game.Red1Status == SD.PlayerInGameStatus.Connected &&
                game.Blue2Status == SD.PlayerInGameStatus.Connected &&
                game.Red2Status == SD.PlayerInGameStatus.Connected) ? true : false;
        }
        public string HakemConnectionId(Game game)
        {
            if (game.HakemIndex == 1) return _unity.PlayerRepo.GetPlayerConnectionId(game.Blue1);
            else if (game.HakemIndex == 2) return _unity.PlayerRepo.GetPlayerConnectionId(game.Red1);
            else if (game.HakemIndex == 3) return _unity.PlayerRepo.GetPlayerConnectionId(game.Blue2);
            else return _unity.PlayerRepo.GetPlayerConnectionId(game.Blue2);
        }
        public int GetPlayerIndex(Game game, string playerName)
        {
            if (game != null)
            {
                if (game.Blue1 == playerName) return 1;
                else if (game.Red1 == playerName) return 2;
                else if (game.Blue2 == playerName) return 3;
                else return 4;
            }

            return 0;
        }
        public void UpdatePlayerStatusOfTheGame(Game game, string playerName, SD.PlayerInGameStatus status)
        {
            if (game.Blue1.Equals(playerName)) game.Blue1Status = status;
            else if (game.Red1.Equals(playerName)) game.Red1Status = status;
            else if (game.Blue2.Equals(playerName)) game.Blue2Status = status;
            else if (game.Red2.Equals(playerName)) game.Red2Status = status;
        }
        public void UpdateGame(Game game, GameUpdateDto model)
        {
            if (model.HakemIndex > 0) game.HakemIndex = model.HakemIndex;
            if (model.WhosTurnIndex > 0) game.WhosTurnIndex = model.WhosTurnIndex;
            if (model.RoundStartsByIndex > 0) game.RoundStartsByIndex = model.RoundStartsByIndex;
            if (!string.IsNullOrEmpty(model.HokmSuit)) game.HokmSuit = model.HokmSuit;
            if (model.ClaimStartsByIndex > 0) game.ClaimStartsByIndex = model.ClaimStartsByIndex;
        }
        public void ResetShelem(Game game)
        {
            _unity.CardRepo.RemoveAllPlayersCardsFromTheGame(game);
            game.GS = SD.GS.DetermineTheInitiator;
            game.Blue1Claimed = 0;
            game.Red1Claimed = 0;
            game.Blue2Claimed = 0;
            game.Red2Claimed = 0;
            game.RoundTargetScore = 0;
            game.ClaimStartsByIndex = SD.GetNextIndex(game.ClaimStartsByIndex);
            game.WhosTurnIndex = game.ClaimStartsByIndex;
        }
        public int PlayerClaimsPoint(Game game, int playerIndex, int point)
        {
            if (playerIndex == 1) game.Blue1Claimed = point;
            else if (playerIndex == 2) game.Red1Claimed = point;
            else if (playerIndex == 3) game.Blue2Claimed = point;
            else game.Red2Claimed = point;

            if (point > 0) game.RoundTargetScore = point;

            if (game.Blue1Claimed == SD.ShelemMaxRoundClaim || (game.Blue1Claimed > 0 && game.Red1Claimed == -1 && game.Blue2Claimed == -1 && game.Red2Claimed == -1))
            {
                game.Red1Claimed = -1;
                game.Blue2Claimed = -1;
                game.Red2Claimed = -1;
                return 1;
            }

            if (game.Red1Claimed == SD.ShelemMaxRoundClaim || (game.Blue1Claimed == -1 && game.Red1Claimed > 0 && game.Blue2Claimed == -1 && game.Red2Claimed == -1))
            {
                game.Blue1Claimed = -1;
                game.Blue2Claimed = -1;
                game.Red2Claimed = -1;
                return 2;
            }

            if (game.Blue2Claimed == SD.ShelemMaxRoundClaim || (game.Blue1Claimed == -1 && game.Red1Claimed == -1 && game.Blue2Claimed > 0 && game.Red2Claimed == -1))
            {
                game.Blue1Claimed = -1;
                game.Red1Claimed = -1;
                game.Red2Claimed = -1;
                return 3;
            }

            if (game.Red2Claimed == SD.ShelemMaxRoundClaim || (game.Blue1Claimed == -1 && game.Red1Claimed == -1 && game.Blue2Claimed == -1 && game.Red2Claimed > 0))
            {
                game.Blue1Claimed = -1;
                game.Red1Claimed = -1;
                game.Blue2Claimed = -1;
                return 4;
            }

            if (game.Blue1Claimed == -1 && game.Red1Claimed == -1 && game.Blue2Claimed == -1 && game.Red2Claimed == -1)
            {
                // all Players have passed
                return -1;
            }

            // Claim continues
            if (playerIndex == 1)
            {
                if (game.Red1Claimed > -1) game.WhosTurnIndex = 2;
                else if (game.Red1Claimed == -1 && game.Blue2Claimed > -1) game.WhosTurnIndex = 3;
                else game.WhosTurnIndex = 4;
            }
            else if (playerIndex == 2)
            {
                if (game.Blue2Claimed > -1) game.WhosTurnIndex = 3;
                else if (game.Blue2Claimed == -1 && game.Red2Claimed > -1) game.WhosTurnIndex = 4;
                else game.WhosTurnIndex = 1;
            }
            else if (playerIndex == 3)
            {
                if (game.Red2Claimed > -1) game.WhosTurnIndex = 4;
                else if (game.Red2Claimed == -1 && game.Blue1Claimed > -1) game.WhosTurnIndex = 1;
                else game.WhosTurnIndex = 2;
            }
            else if (playerIndex == 4)
            {
                if (game.Blue1Claimed > -1) game.WhosTurnIndex = 1;
                else if (game.Blue1Claimed == -1 && game.Red1Claimed > -1) game.WhosTurnIndex = 2;
                else game.WhosTurnIndex = 3;
            }

            return 0;
        }
        public void AssignPlayersCards(Game game)
        {
            if (!string.IsNullOrEmpty(game.Blue1CardsName)) return;

            int cardsToDistribute = 0;
            if (game.GameType == SD.Hokm) cardsToDistribute = 13;
            else cardsToDistribute = 12;

            var deckOfCard = SD.GetShuffledDeckOfCards();
            List<string> cards = new List<string>();

            for (int i = 0; i < cardsToDistribute; i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Blue1Cards = _unity.CardRepo.SetPlayerCards(game.Blue1, cards);
            cards.Clear();

            for (int i = cardsToDistribute; i < (cardsToDistribute * 2); i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Red1Cards = _unity.CardRepo.SetPlayerCards(game.Red1, cards);
            cards.Clear();

            for (int i = cardsToDistribute * 2; i < (cardsToDistribute * 3); i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Blue2Cards = _unity.CardRepo.SetPlayerCards(game.Blue2, cards);
            cards.Clear();

            for (int i = cardsToDistribute * 3; i < (cardsToDistribute * 4); i++)
            {
                cards.Add(deckOfCard.ElementAt(i));
            }
            game.Red2Cards = _unity.CardRepo.SetPlayerCards(game.Red2, cards);
            cards.Clear();

            if (game.GameType == SD.Shelem)
            {
                for (int i = cardsToDistribute * 4; i < deckOfCard.Count; i++)
                {
                    cards.Add(deckOfCard.ElementAt(i));
                }
                game.HakemCards = _unity.CardRepo.SetPlayerCards($"{game.Name}_hakem", cards);
            }
        }
        public HakemCardsToHokm GetHakemCardsToHokm(Game game)
        {
            Card cards;
            string hakemConnectionId = "";

            if (game.HakemIndex == 1)
            {
                cards = game.Blue1Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Blue1);
            }
            else if (game.HakemIndex == 2)
            {
                cards = game.Red1Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Red1);
            }
            else if (game.HakemIndex == 3)
            {
                cards = game.Blue2Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Blue2);
            }
            else
            {
                cards = game.Red2Cards;
                hakemConnectionId = _unity.PlayerRepo.GetPlayerConnectionId(game.Red2);
            }

            var firstFiveCards = new string[]
            {
                cards.Card1,
                cards.Card2,
                cards.Card3,
                cards.Card4,
                cards.Card5
            };

            return new HakemCardsToHokm(hakemConnectionId, firstFiveCards);
        }
        public void ShelemUpdateHakemCards(Game game, List<string> selectedCards)
        {
            var hakemName = GetHakemName(game);
            var score = _unity.CardRepo.ShelemUpdateHakemCards(game.Name, hakemName, selectedCards);
            if (game.HakemIndex == 1 || game.HakemIndex == 3) game.BlueRoundScore = score;
            else game.RedRoundScore = score;
        }
        public bool HandlePlayerPlayedTheCard(Game game, string card, string playerName, int playerIndex)
        {
            var playerCards = _unity.CardRepo.FindByName(playerName);
            // if the played card by the player is the first round card
            if (game.RoundSuit == null)
            {
                if (game.GameType == SD.Shelem)
                {
                    // for Shelem, the first played card by Hakem must be Hokm
                    if (game.HakemIndex == playerIndex && game.NthCardIsBeingPlayed == 1)
                    {
                        var playerHasRoundSuitCard = PlayerHasTheSuitCardInHand(playerCards, game.HokmSuit);
                        if (playerHasRoundSuitCard && !SD.GetSuitOfCard(card).Equals(game.HokmSuit))
                        {
                            return false;
                        }
                        else
                        {
                            game.RoundSuit = SD.GetSuitOfCard(game.HokmSuit);
                            SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(game, playerCards, card, playerIndex);
                            return true;
                        }
                    }
                }
                game.RoundSuit = SD.GetSuitOfCard(card);
                SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(game, playerCards, card, playerIndex);
                return true;
            }
            else
            {
                var playerHasRoundSuitCard = PlayerHasTheSuitCardInHand(playerCards, game.RoundSuit);

                //  check if player has card of current round suit
                if (playerHasRoundSuitCard)
                {
                    // check if the played card is not the same kind as round suit
                    if (SD.GetSuitOfCard(card) != game.RoundSuit)
                    {
                        return false;
                    }
                }

                SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(game, playerCards, card, playerIndex);

                return true;
            }
        }
        public void RoundCalculation(Game game)
        {
            int blueTotal = 0;
            int redTotal = 0;

            int blue1Value = GetValueOfPlayedCard(game, game.Blue1Card, game.RoundSuit, game.HokmSuit);
            int blue2Value = GetValueOfPlayedCard(game, game.Blue2Card, game.RoundSuit, game.HokmSuit);
            int red1Value = GetValueOfPlayedCard(game, game.Red1Card, game.RoundSuit, game.HokmSuit);
            int red2Value = GetValueOfPlayedCard(game, game.Red2Card, game.RoundSuit, game.HokmSuit);

            if (blue1Value > blue2Value)
            {
                blueTotal = blue1Value;
            }
            else
            {
                blueTotal = blue2Value;
            }

            if (red1Value > red2Value)
            {
                redTotal = red1Value;
            }
            else
            {
                redTotal = red2Value;
            }

            if (blueTotal > redTotal)
            {
                if (blue1Value > blue2Value)
                {
                    game.WhosTurnIndex = 1;
                    game.RoundStartsByIndex = 1;
                }
                else
                {
                    game.WhosTurnIndex = 3;
                    game.RoundStartsByIndex = 3;
                }

                if (game.GameType == SD.Shelem)
                {
                    game.BlueRoundScore += SD.GetShelemRoundScore(game.Blue1Card, game.Red1Card, game.Blue2Card, game.Red2Card);
                }
                else if (game.GameType == SD.Hokm)
                {
                    game.BlueRoundScore++;
                }
            }
            else
            {
                if (red1Value > red2Value)
                {
                    game.WhosTurnIndex = 2;
                    game.RoundStartsByIndex = 2;
                }
                else
                {
                    game.WhosTurnIndex = 4;
                    game.RoundStartsByIndex = 4;
                }

                if (game.GameType == SD.Shelem)
                {
                    game.RedRoundScore += SD.GetShelemRoundScore(game.Blue1Card, game.Red1Card, game.Blue2Card, game.Red2Card);
                }
                else if (game.GameType == SD.Hokm)
                {
                    game.RedRoundScore++;
                }
            }

            // empty the cards
            game.Blue1Card = null;
            game.Red1Card = null;
            game.Blue2Card = null;
            game.Red2Card = null;
            game.RoundSuit = null;
            game.NthCardIsBeingPlayed++;
        }
        public int GetNewHakemIndex(Game game)
        {
            if (game.GameType == SD.Hokm)
            {
                // Blue won the round game
                if (game.BlueRoundScore == SD.HokmEndOfRoundScore)
                {
                    game.BlueTotalScore++;
                    // in case of Kot
                    if (game.RedRoundScore == 0)
                    {
                        game.BlueTotalScore++;

                        // Hakem Kot
                        if (game.HakemIndex == 2 || game.HakemIndex == 4)
                        {
                            game.BlueTotalScore++;
                        }
                    }


                    if (game.HakemIndex == 2) return 3;
                    else if (game.HakemIndex == 4) return 1;
                    else return game.HakemIndex;
                }

                // Red won the round game
                if (game.RedRoundScore == SD.HokmEndOfRoundScore)
                {
                    game.RedTotalScore++;
                    // in case of Kot
                    if (game.BlueRoundScore == 0)
                    {
                        game.RedTotalScore++;
                        // Hakem Kot
                        if (game.HakemIndex == 1 || game.HakemIndex == 3)
                        {
                            game.RedTotalScore++;
                        }
                    }

                    if (game.HakemIndex == 3) return 4;
                    else if (game.HakemIndex == 1) return 2;
                    else return game.HakemIndex;
                }

                return 0;
            }
            else
            {
                // Shelem game calculation
                if (game.BlueRoundScore + game.RedRoundScore == SD.ShelemMaxRoundClaim)
                {
                    if (game.HakemIndex == 1 || game.HakemIndex == 3)
                    {
                        if (game.BlueRoundScore == SD.ShelemMaxRoundClaim)
                        {
                            // Shelem has happened
                            game.BlueTotalScore += SD.ShelemMaxRoundClaim * 2;
                        }
                        else if (game.BlueRoundScore >= game.RoundTargetScore)
                        {
                            game.BlueTotalScore += game.BlueRoundScore;
                        }
                        else
                        {
                            game.BlueTotalScore -= game.RoundTargetScore;
                        }

                        game.RedTotalScore += game.RedRoundScore;
                    }
                    else
                    {
                        if (game.RedRoundScore == SD.ShelemMaxRoundClaim)
                        {
                            // Shelem has happened
                            game.RedTotalScore += SD.ShelemMaxRoundClaim * 2;
                        }
                        else if (game.RedRoundScore >= game.RoundTargetScore)
                        {
                            game.RedTotalScore += game.RedRoundScore;
                        }
                        else
                        {
                            game.RedTotalScore -= game.RoundTargetScore;
                        }

                        game.BlueTotalScore += game.RedRoundScore;
                    }

                    game.ClaimStartsByIndex = SD.GetNextIndex(game.ClaimStartsByIndex);
                    game.WhosTurnIndex = game.ClaimStartsByIndex;

                    return game.WhosTurnIndex;
                }
                else
                {
                    return 0;
                }
            }
        }
        public void ResetRoundGame(Game game, int hakemIndex)
        {
            game.Blue1Card = null;
            game.Red1Card = null;
            game.Blue2Card = null;
            game.Red2Card = null;
            game.RoundSuit = null;
            game.HokmSuit = null;
            game.RedRoundScore = 0;
            game.BlueRoundScore = 0;
            game.RoundTargetScore = 0;
            game.Blue1Claimed = 0;
            game.Red1Claimed = 0;
            game.Blue2Claimed = 0;
            game.Red2Claimed = 0;
            
            if (game.GameType == SD.Hokm)
            {
                game.HakemIndex = hakemIndex;
                game.RoundStartsByIndex = hakemIndex;
                game.WhosTurnIndex = hakemIndex;
            }
            else
            {
                game.HakemIndex = 0;
                game.RoundStartsByIndex = 0;
                game.WhosTurnIndex = 0;
            }
           
            game.NthCardIsBeingPlayed = 1;
        }
        public string EndOfTheGame(Game game)
        {
            string winner = null;
            if (game.BlueTotalScore > game.RedTotalScore)
            {
                if (game.BlueTotalScore >= game.TargetScore)
                {
                    winner = SD.Blue;
                }
            }
            else if (game.RedTotalScore > game.BlueTotalScore)
            {
                if (game.RedTotalScore >= game.TargetScore)
                {
                    winner = SD.Red;
                }
            }

            if (!string.IsNullOrEmpty(winner))
            {
                CloseTheGame(game);
            }

            return winner;
        }
        public void CloseTheGame(Game game)
        {
            _unity.CardRepo.RemoveAllPlayersCardsFromTheGame(game);
            Remove(game);
            foreach (var player in game.Players)
            {
                player.RoomName = null;
            }
        }
        public string GetPlayerNameByIndex(Game game, int index)
        {
            if (index == 1) return game.Blue1;
            else if (index == 2) return game.Red1;
            else if (index == 3) return game.Blue2;
            else return game.Red2;
        }
        public string GetPlayerNameByIndex(GameInfoDto game, int index)
        {
            if (index == 1) return game.Blue1;
            else if (index == 2) return game.Red1;
            else if (index == 3) return game.Blue2;
            else return game.Red2;
        }
        #region Private Methods
        private void SetPlayedCardAndNextPersonTurnAndRemoveCardFromHand(Game game, Card playerCards, string card, int playerIndex)
        {
            if (playerIndex == 1)
            {
                game.Blue1Card = card;
                game.WhosTurnIndex = 2;
            }
            else if (playerIndex == 2)
            {
                game.Red1Card = card;
                game.WhosTurnIndex = 3;
            }
            else if (playerIndex == 3)
            {
                game.Blue2Card = card;
                game.WhosTurnIndex = 4;
            }
            else
            {
                game.Red2Card = card;
                game.WhosTurnIndex = 1;
            }

            _unity.CardRepo.RemoveCardFromPlayerHand(playerCards, card);
        }
        private int GetValueOfPlayedCard(Game game, string card, string roundSuit, string hokmSuit)
        {
            int value = 0;

            if (SD.GetSuitOfCard(card).Equals(roundSuit))
            {
                value = SD.GetValueOfCard(card);
            }

            if (SD.GetSuitOfCard(card).Equals(hokmSuit))
            {
                value = SD.GetValueOfCard(card) + 13;
            }

            return value;
        }
        private string GetHakemName(Game game)
        {
            if (game.HakemIndex == 1) return game.Blue1;
            else if (game.HakemIndex == 2) return game.Red1;
            else if (game.HakemIndex == 3) return game.Blue2;
            else return game.Red2;
        }
        private bool PlayerHasTheSuitCardInHand(Card playerCards, string suit)
        {
            var playerHasRoundSuitCard = false;
            var playerCardsAsList = _unity.CardRepo.GetCardsAsList(playerCards);

            for (int i = 0; i < playerCardsAsList.Count; i++)
            {
                if (SD.GetSuitOfCard(playerCardsAsList[i]).Equals(suit))
                {
                    playerHasRoundSuitCard = true;
                    i = playerCardsAsList.Count;
                }
            }

            return playerHasRoundSuitCard;
        }
        #endregion
    }
}
