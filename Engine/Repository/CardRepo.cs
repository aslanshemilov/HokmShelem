namespace Engine.Repository
{
    public class CardRepo : BaseRepo<Card>, ICardRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public CardRepo(Context context,
            IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public Card SetPlayerCards(string playerName, List<string> cards)
        {
            var cardToAdd = new Card()
            {
                Name = playerName,
                Card1 = cards.ElementAt(0),
                Card2 = cards.ElementAt(1),
                Card3 = cards.ElementAt(2),
                Card4 = cards.ElementAt(3),
                Card5 = cards.ElementAt(4),
                Card6 = cards.ElementAt(5),
                Card7 = cards.ElementAt(6),
                Card8 = cards.ElementAt(7),
                Card9 = cards.ElementAt(8),
                Card10 = cards.ElementAt(9),
                Card11 = cards.ElementAt(10),
                Card12 = cards.ElementAt(11),
                Card13 = cards.ElementAt(12)
            };

            return cardToAdd;
        }

        public List<string> GetCardsAsList(Card card)
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
        public List<string> GetPlayerCardsAsList(Game game, string playerName)
        {
            if (game.Blue1 == playerName)
            {
                return GetCardsAsList(game.Blue1Cards);
            }
            else if (game.Red1.Equals(playerName))
            {
                return GetCardsAsList(game.Red1Cards);
            }
            else if (game.Blue2.Equals(playerName))
            {
                return GetCardsAsList(game.Blue2Cards);
            }
            else
            {
                return GetCardsAsList(game.Red2Cards);
            }
        }
    }
}
