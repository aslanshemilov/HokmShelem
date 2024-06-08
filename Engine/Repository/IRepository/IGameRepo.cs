namespace Engine.Repository.IRepository
{
    public interface IGameRepo : IBaseRepo<Game>
    {
        //int CountOfActiveGames();
        void CreateGame(Room room);
        CurrentGameDto GetCurrentGame(string playerName);
        GameInfoDto GetGameInfo(string gameName, string playerName);
        bool AllPlayersAreConnected(Game game);
        string HakemConnectionId(Game game);
        int GetPlayerIndex(Game game, string playerName);
        void UpdatePlayerStatusOfTheGame(Game game, string playerName, SD.PlayerInGameStatus status);
        void UpdateGame(Game game, GameUpdateDto model);
        void ResetShelem(Game game);
        int PlayerClaimsPoint(Game game, int playerIndex, int point);
        void AssignPlayersCards(Game game);
        HakemCardsToHokm GetHakemCardsToHokm(Game game);
        bool HandlePlayerPlayedTheCard(Game game, string card, string playerName, int playerIndex);
        void RoundCalculation(Game game);
        int GetNewHakemIndex(Game game);
        void ResetRoundGame(Game game, int hakemIndex);
        string EndOfTheGame(Game game);     
        void CloseTheGame(Game game);
    }
}
