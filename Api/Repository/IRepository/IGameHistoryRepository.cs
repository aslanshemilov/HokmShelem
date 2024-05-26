namespace Api.Repository.IRepository
{
    public interface IGameHistoryRepository
    {
        void CreateGameHistory(GameHistoryDto model);
        Task<List<GameHistoryDto>> GetAllGameHistories();
    }
}
