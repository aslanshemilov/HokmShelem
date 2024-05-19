namespace Engine.Repository.IRepository
{
    public interface IGameRepo : IBaseRepo<Game>
    {
        string GetGameName(string playerName);
        GameInfoDto GetGameInfo(string gameName, string playerName);
        void UpdatePlayerStatusOfTheGame(Game game, string playerName, SD.PlayerInGameStatus status);
        void UpdateGame(Game game, GameUpdateDto model);
        void AssignPlayersCards(Game game);
        bool EndOfRoundGame(Game game);
        void EmptyRoundCardsAndSuit(Game game);
        bool EndOfTheGame(Game game);

    }
}
