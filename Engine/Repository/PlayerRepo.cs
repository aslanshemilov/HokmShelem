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

        public string AddUpdatePlayer(Player player)
        {
            var fetchedPlayer = FindByName(player.Name);
            
            if (fetchedPlayer == null)
            {
                Add(player);
                _unity.ConnectionTrackerRepo.AddConnectionTracker(player.Name, player.ConnectionId);

                return null;
            }
            else
            {
                string oldConnectionId = null;
                
                if (!string.IsNullOrEmpty(fetchedPlayer.ConnectionId))
                {
                    oldConnectionId = fetchedPlayer.ConnectionId;
                }

                var fetchedTracker = _unity.ConnectionTrackerRepo.FindByName(player.Name);
                if (fetchedTracker == null)
                {
                    _unity.ConnectionTrackerRepo.AddConnectionTracker(player.Name, player.ConnectionId);
                }
                else
                {
                    fetchedTracker.OldId = oldConnectionId;
                    fetchedTracker.CurrentId = player.ConnectionId;
                }
                player.RoomName = fetchedPlayer.RoomName;
                _context.Entry(fetchedPlayer).CurrentValues.SetValues(player);
                return oldConnectionId;
            }
        }
        public string GetGameName(string playerName)
        {
            return _context.Player.Where(p => p.Name.Equals(playerName)).Select(r => r.GameName).FirstOrDefault();
        }
    }
}
