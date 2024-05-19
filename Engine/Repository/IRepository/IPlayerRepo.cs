namespace Engine.Repository.IRepository
{
    public interface IPlayerRepo : IBaseRepo<Player>
    {
        string AddUpdatePlayer(Player player, string connectionId);
        string GetPlayerConnectionId(string playerName);
        string GetGameName(string playerName);
    }
}
