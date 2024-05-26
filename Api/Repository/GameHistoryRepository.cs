namespace Api.Repository
{
    public class GameHistoryRepository : IGameHistoryRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public GameHistoryRepository(Context context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void CreateGameHistory(GameHistoryDto model)
        {
            _context.GameHistory.Add(_mapper.Map<GameHistory>(model));
        }

        public async Task<List<GameHistoryDto>> GetAllGameHistories()
        {
            return _mapper.Map<List<GameHistoryDto>>(await _context.GameHistory.ToListAsync());
        }
    }
}
