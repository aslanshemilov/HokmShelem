namespace Api.IRepository
{
    public interface IBadgeRepository
    {
        Task<Badge> GetBadgeByColorAsync(string color);
        Task<int> GetBadgeIdByColorAsync(string color);
    }
}
