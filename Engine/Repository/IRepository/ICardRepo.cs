namespace Engine.Repository.IRepository
{
    public interface ICardRepo : IBaseRepo<Card>
    {
        List<string> GetCardsAsList(Card card);
        Card SetPlayerCards(string playerName, List<string> cards);
        void RemoveCardFromPlayerHand(Card card, string cardToRemove);
        void RemoveAllPlayersCardsFromTheGame(Game game);
    }
}
