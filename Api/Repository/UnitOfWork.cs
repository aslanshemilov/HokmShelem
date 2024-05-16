using Microsoft.AspNetCore.Identity;

namespace Api.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly Context _context;
        private readonly ContextVisitors _hSContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UnitOfWork(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            Context context,
            ContextVisitors hSContext,
            IConfiguration config,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _hSContext = hSContext;
            _config = config;
            _mapper = mapper;
        }

        public IAdminRepository AdminRepository => new AdminRepository(_context, _userManager, _roleManager, _mapper);
        public IHokmShelemRepository HokmShelemRepository => new HokmShelemRepository(_context, _hSContext, _config, _mapper);
        public IContactUsRepository ContactUsRepository => new ContactUsRepository(_context);
        public IBadgeRepository BadgeRepository => new BadgeRepository(_context);

        public ICountryRepository CountryRepository => new CountryRepository(_context);
        public IUserStatusRepository UserStatusRepository => new UserStatusRepository(_context);
        public IUserProfileRepository UserProfileRepository => new UserProfileRepository(_context, this, _mapper);

        public async Task<bool> CompleteAsync()
        {
            bool result = false;
            if (_context.ChangeTracker.HasChanges())
            {
                result = await _context.SaveChangesAsync() > 0;
            }

            if (_hSContext.ChangeTracker.HasChanges())
            {
                result = await _hSContext.SaveChangesAsync() > 0;
            }

            return result;
        }
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
