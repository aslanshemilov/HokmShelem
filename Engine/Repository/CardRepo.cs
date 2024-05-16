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

        public Card SetPlayerCards(string playerName,List<string> cards)
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
    }
}
