namespace Engine.Repository.IRepository
{
    public interface IConnectionTrackerRepo : IBaseRepo<ConnectionTracker>
    {
        void AddConnectionTracker(string playerName, string connectionId);
    }
}
