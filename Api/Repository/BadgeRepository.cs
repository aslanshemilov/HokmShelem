namespace Api.Repository
{
    public class BadgeRepository : IBadgeRepository
    {
        private readonly Context _context;

        public BadgeRepository(Context context)
        {
            _context = context;
        }
        public async Task<Badge> GetBadgeByColorAsync(string color)
        {
            return await _context.Badge.FirstOrDefaultAsync(x => x.Color == color);
        }
        public async Task<int> GetBadgeIdByColorAsync(string color)
        {
            return await _context.Badge.Where(x => x.Color == color).Select(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
