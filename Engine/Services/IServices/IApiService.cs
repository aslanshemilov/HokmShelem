namespace Engine.Services.IServices
{
    public interface IApiService
    {
        Task<PlayerDto> GetPlayerInfoAsync(bool isGuestUser);
        Task<bool> CreateGameHistoryAsync(GameHistory model);
    }
}
