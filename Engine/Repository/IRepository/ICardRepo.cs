namespace Engine.Repository.IRepository
{
    public interface ICardRepo : IBaseRepo<Card>
    {
        List<string> GetCardsAsList(string name);
        List<string> GetCardsAsList(Card card);
        Card SetPlayerCards(string playerName, List<string> cards);
        int ShelemUpdateHakemCards(string gameName, string playerName, List<string> selectedCards);
        void RemoveCardFromPlayerHand(Card card, string cardToRemove);
        void RemoveAllPlayersCardsFromTheGame(Game game);
    }
}
