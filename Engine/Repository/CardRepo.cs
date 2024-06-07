using Engine.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Engine.Repository
{
    public class CardRepo : BaseRepo<Card>, ICardRepo
    {
        private readonly Context _context;

        public CardRepo(Context context) : base(context)
        {
            _context = context;
        }

        public List<string> GetCardsAsList(Card card)
        {
            if (card != null)
            {
                List<string> cards = new List<string>();

                if (!string.IsNullOrEmpty(card.Card1)) cards.Add(card.Card1);
                if (!string.IsNullOrEmpty(card.Card2)) cards.Add(card.Card2);
                if (!string.IsNullOrEmpty(card.Card3)) cards.Add(card.Card3);
                if (!string.IsNullOrEmpty(card.Card4)) cards.Add(card.Card4);
                if (!string.IsNullOrEmpty(card.Card5)) cards.Add(card.Card5);
                if (!string.IsNullOrEmpty(card.Card6)) cards.Add(card.Card6);
                if (!string.IsNullOrEmpty(card.Card7)) cards.Add(card.Card7);
                if (!string.IsNullOrEmpty(card.Card8)) cards.Add(card.Card8);
                if (!string.IsNullOrEmpty(card.Card9)) cards.Add(card.Card9);
                if (!string.IsNullOrEmpty(card.Card10)) cards.Add(card.Card10);
                if (!string.IsNullOrEmpty(card.Card11)) cards.Add(card.Card11);
                if (!string.IsNullOrEmpty(card.Card12)) cards.Add(card.Card12);
                if (!string.IsNullOrEmpty(card.Card13)) cards.Add(card.Card13);

                return cards;
            }

            return null;
        }
        public Card SetPlayerCards(string playerName, List<string> cards)
        {
            var cardToAdd = new Card();
            cardToAdd.Name = playerName;

            cardToAdd.Card1 = cards.ElementAt(0);
            cardToAdd.Card2 = cards.ElementAt(1);
            cardToAdd.Card3 = cards.ElementAt(2);
            cardToAdd.Card4 = cards.ElementAt(3);

            if (cards.Count >= 12)
            {
                cardToAdd.Card5 = cards.ElementAt(4);
                cardToAdd.Card6 = cards.ElementAt(5);
                cardToAdd.Card7 = cards.ElementAt(6);
                cardToAdd.Card8 = cards.ElementAt(7);
                cardToAdd.Card9 = cards.ElementAt(8);
                cardToAdd.Card10 = cards.ElementAt(9);
                cardToAdd.Card11 = cards.ElementAt(10);
                cardToAdd.Card12 = cards.ElementAt(11);
            }

            if (cards.Count == 13)
            {
                cardToAdd.Card13 = cards.ElementAt(12);
            }

            return cardToAdd;
        }
        public void RemoveCardFromPlayerHand(Card card, string cardToRemove)
        {
            if (card.Card1 == cardToRemove) card.Card1 = null;
            else if (card.Card2 == cardToRemove) card.Card2 = null;
            else if (card.Card3 == cardToRemove) card.Card3 = null;
            else if (card.Card4 == cardToRemove) card.Card4 = null;
            else if (card.Card5 == cardToRemove) card.Card5 = null;
            else if (card.Card6 == cardToRemove) card.Card6 = null;
            else if (card.Card7 == cardToRemove) card.Card7 = null;
            else if (card.Card8 == cardToRemove) card.Card8 = null;
            else if (card.Card9 == cardToRemove) card.Card9 = null;
            else if (card.Card10 == cardToRemove) card.Card10 = null;
            else if (card.Card11 == cardToRemove) card.Card11 = null;
            else if (card.Card12 == cardToRemove) card.Card12 = null;
            else card.Card13 = null;
        }
        public void RemoveAllPlayersCardsFromTheGame(Game game)
        {
            if (game != null)
            {
                if (!string.IsNullOrEmpty(game.Blue1CardsName)) RemovePlayerCards(game.Blue1);
                if (!string.IsNullOrEmpty(game.Red1CardsName)) RemovePlayerCards(game.Red1);
                if (!string.IsNullOrEmpty(game.Blue2CardsName)) RemovePlayerCards(game.Blue2);
                if (!string.IsNullOrEmpty(game.Red2CardsName)) RemovePlayerCards(game.Red2);
                if (!string.IsNullOrEmpty(game.HakemCardsName)) RemovePlayerCards(game.HakemCardsName);
            }
        }

        #region Private Methods
        private void RemovePlayerCards(string playerName)
        {
            var card = _context.Card.Where(p => p.Name == playerName).FirstOrDefault();
            if (card != null)
            {
                _context.Card.Remove(card);
            }
        }
        #endregion
    }
}
