namespace Engine.Services.IServices
{
    public interface IApiService
    {
        Task<PlayerDto> GetPlayerInfoAsync();
    }
}
