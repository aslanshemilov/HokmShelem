namespace Engine.Repository
{
    public class UnityRepo : IUnityRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public UnityRepo(Context context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IPlayerRepo PlayerRepo => new PlayerRepo(this, _context);
        public IConnectionTrackerRepo ConnectionTrackerRepo => new ConnectionTrackerRepo(_context);
        public ILobbyRepo LobbyRepo => new LobbyRepo(_context, _mapper);
        public IRoomRepo RoomRepo => new RoomRepo(_context, _mapper);
        public IGameRepo GameRepo => new GameRepo(this, _context, _mapper);
        public ICardRepo CardRepo => new CardRepo(_context, _mapper);
        public void Dispose()
        {
            _context.Dispose();
        }
        public bool Complete()
        {
            bool result = false;
            if (_context.ChangeTracker.HasChanges())
            {
                result = _context.SaveChanges() > 0;
            }

            return result;
        }
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
