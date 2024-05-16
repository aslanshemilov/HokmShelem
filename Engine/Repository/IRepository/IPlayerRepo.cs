namespace Engine.Repository.IRepository
{
    public interface IPlayerRepo : IBaseRepo<Player>
    {
        string AddUpdatePlayer(Player player);
        string GetGameName(string playerName);
    }
}
