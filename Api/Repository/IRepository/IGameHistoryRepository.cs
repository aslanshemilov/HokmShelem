namespace Api.Repository.IRepository
{
    public interface IGameHistoryRepository
    {
        Task CreateGameHistoryAsync(GameHistoryDto model);
        Task<List<GameHistoryDto>> GetAllGameHistories();
    }
}
