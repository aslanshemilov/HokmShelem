namespace Engine.Repository.IRepository
{
    public interface IPlayerRepo : IBaseRepo<Player>
    {
        //int CountOfActivePlayers();
        string AddUpdatePlayer(Player player, string connectionId);
        string GetPlayerConnectionId(string playerName);
        string GetGameName(string playerName);
    }
}
