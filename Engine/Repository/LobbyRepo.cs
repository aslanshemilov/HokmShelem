namespace Engine.Repository
{
    public class LobbyRepo : BaseRepo<Lobby>, ILobbyRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public LobbyRepo(Context context,
            IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
