namespace Engine.Services.IServices
{
    public interface IGameTrackerService
    {
        bool PlayerConnectedToGameTracker(string gameName, string playerName);
        List<string> GetGameTrackerConnectedPlayers(string gameName);
        bool GameTrackerHasPlayer(string gameName, string playerName);
        void RemovePlayerFromGameTracker(string gameName, string playerName);
        void RemoveGameTracker(string gameName);
    }
}
