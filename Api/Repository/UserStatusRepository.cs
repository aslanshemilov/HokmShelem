namespace Api.Repository
{
    public class UserStatusRepository : IUserStatusRepository
    {
        private readonly Context _context;

        public UserStatusRepository(Context context)
        {
            _context = context;
        }

        public async Task<UserStatus> GetStatusByNameAsync(string name)
        {
            return await _context.UserStatus.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<int> GetStatusIdByNameAsync(string name)
        {
            return await _context.UserStatus.Where(x => x.Name == name).Select(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
