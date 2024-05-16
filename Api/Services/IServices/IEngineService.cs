namespace Api.Services.IServices
{
    public interface IEngineService
    {
        Task AddPlayerAsync(PlayerDto model);
        Task AddTest();
    }
}
