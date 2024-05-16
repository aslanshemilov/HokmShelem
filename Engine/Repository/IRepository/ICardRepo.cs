namespace Engine.Repository.IRepository
{
    public interface ICardRepo : IBaseRepo<Card>
    {
        Card SetPlayerCards(string playerName, List<string> cards);
    }
}
