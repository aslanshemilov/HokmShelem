namespace Engine.Repository
{
    public class PlayerRepo : BaseRepo<Player>, IPlayerRepo
    {
        private readonly IUnityRepo _unity;
        private readonly Context _context;

        public PlayerRepo(IUnityRepo unity,
            Context context) : base(context)
        {
            _unity = unity;
            _context = context;
        }

        public string AddUpdatePlayer(Player model, string connectionId)
        {
            var fetchedPlayer = FindByName(model.Name);
            
            if (fetchedPlayer == null)
            {
                model.ConnectionId = connectionId;
                Add(model);
                _unity.ConnectionTrackerRepo.AddConnectionTracker(model.Name, connectionId);

                return null;
            }
            else
            {
                string oldConnectionId = null;
                
                if (!string.IsNullOrEmpty(fetchedPlayer.ConnectionId))
                {
                    oldConnectionId = fetchedPlayer.ConnectionId;
                }

                var fetchedTracker = _unity.ConnectionTrackerRepo.FindByName(model.Name);
                if (fetchedTracker == null)
                {
                    _unity.ConnectionTrackerRepo.AddConnectionTracker(model.Name, connectionId);
                }
                else
                {
                    fetchedTracker.OldId = oldConnectionId;
                    fetchedTracker.CurrentId = connectionId;
                }

                model.ConnectionId = connectionId;
                model.RoomName = fetchedPlayer.RoomName;
                model.GameName = fetchedPlayer.GameName;

                _context.Entry(fetchedPlayer).CurrentValues.SetValues(model);
                return oldConnectionId;
            }
        }
        public string GetPlayerConnectionId(string playerName)
        {
            return _context.Player
                .Where(p => p.Name == playerName)
                .Select(x => x.ConnectionId)
                .FirstOrDefault();
        }
        public string GetGameName(string playerName)
        {
            return _context.Player.Where(p => p.Name.Equals(playerName)).Select(r => r.GameName).FirstOrDefault();
        }
    }
}
