namespace Api.Repository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly Context _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserProfileRepository(Context context,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AnyUserProfileAsync(int userId)
        {
            return await _context.UserProfile.AnyAsync(p => p.UserId == userId);
        }

        public async Task CreateUserProfileAsync(ApplicationUserAddDto model)
        {
            var pinkBadgeId = await _unitOfWork.BadgeRepository.GetBadgeIdByColorAsync(SD.Pink);
            var offlineStatusId = await _unitOfWork.UserStatusRepository.GetStatusIdByNameAsync(SD.Offline);

            var userProfileToAdd = new UserProfile
            {
                BadgeId = pinkBadgeId,
                CountryId = model.CountryId,
                StatusId = offlineStatusId,
                UserId = model.ApplicationUserId
            };

            _context.UserProfile.Add(userProfileToAdd);
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userProfileId)
        {
            return await _context.UserProfile
                .Where(x => x.Id == userProfileId)
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfile> GetUserProfileToUpdateAsync(int userProfileId)
        {
            return await _context.UserProfile.FindAsync(userProfileId);
        }
        public async Task UpdateUserProfileAsync(UserProfileUpdateDto model, int userProfileId)
        {
            var userProfile = await _context.UserProfile.FindAsync(userProfileId);
        }

        public async Task<PlayerDto> GetPlayerInfoAsync(int userProfileId)
        {
            return await _context.UserProfile
                .Where(x => x.Id == userProfileId)
                .ProjectTo<PlayerDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetUserProfileIdByUserIdAsync(int userId)
        {
            return await _context.UserProfile.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
