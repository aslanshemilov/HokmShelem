namespace Engine.Repository
{
    public class LobbyRepo : BaseRepo<Lobby>, ILobbyRepo
    {
        private readonly Context _context;

        public LobbyRepo(Context context) : base(context)
        {
            _context = context;
        }
    }
}
