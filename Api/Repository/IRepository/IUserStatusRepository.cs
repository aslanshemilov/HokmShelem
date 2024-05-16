namespace Api.IRepository
{
    public interface IUserStatusRepository
    {
        Task<UserStatus> GetStatusByNameAsync(string name);
        Task<int> GetStatusIdByNameAsync(string name);
    }
}
