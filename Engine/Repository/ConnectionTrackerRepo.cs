namespace Engine.Repository
{
    public class ConnectionTrackerRepo : BaseRepo<ConnectionTracker>, IConnectionTrackerRepo
    {
        private readonly Context _context;

        public ConnectionTrackerRepo(Context context) : base(context)
        {
            _context = context;
        }
        public void AddConnectionTracker(string playerName, string connectionId)
        {
            var connectionTrackerToAdd = new ConnectionTracker()
            {
                Name = playerName,
                CurrentId = connectionId
            };

            Add(connectionTrackerToAdd);
        }
    }
}
