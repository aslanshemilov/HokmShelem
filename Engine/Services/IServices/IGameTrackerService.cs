namespace Engine.Services.IServices
{
    public interface IGameTrackerService
    {
        bool PlayerConnectedToGame(string gameName, string playerName);
        void PlayerDisconnected(string gameName, string playerName);
        List<string> GetConnectedPlayersOfGame(string gameName);
    }
}
