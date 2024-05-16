namespace Engine.Repository.IRepository
{
    public interface IGameRepo : IBaseRepo<Game>
    {
        string GetGameName(string playerName);
        GameInfoDto GetGameInfo(string gameName, string playerName);
        void AssignPlayersCards(Game game);
    }
}
