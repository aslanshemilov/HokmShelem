namespace Api.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly Context _context;

        public AdminRepository(Context context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedList<ApplicationUser>> GetMembersAsync(A_MemberParams memberParams)
        {
            var userQuery = _context.Users.Include(c => c.UserProfile).ThenInclude(c => c.Status).AsQueryable();

            userQuery = userQuery.Where(u => u.UserName != memberParams.CurrentUsername && u.UserName != SD.Admin);

            if (!string.IsNullOrEmpty(memberParams.Role) && !memberParams.Role.Equals("all"))
            {
                var roleId = await _context.Roles.Where(c => c.Name == memberParams.Role).Select(c => c.Id).FirstOrDefaultAsync();
                if (roleId > 0)
                {
                    var userIds = await _context.UserRoles.Where(c => c.RoleId == roleId).Select(c => c.UserId).ToListAsync();
                    userQuery = userQuery.Where(u => userIds.Contains(u.Id));
                }
            }

            if (!string.IsNullOrEmpty(memberParams.Status) && !memberParams.Status.Equals("all"))
            {
                var statusId = await _context.UserStatus.Where(c => c.Name.Equals(memberParams.Status)).Select(c => c.Id).FirstOrDefaultAsync();
                if (statusId > 0)
                {
                    userQuery = userQuery.Where(u => u.UserProfile.StatusId == statusId);
                }                
            }

            if (!string.IsNullOrEmpty(memberParams.Provider) && !memberParams.Provider.Equals("all"))
            {
                userQuery = userQuery.Where(u => u.Provider.Equals(memberParams.Provider));
            }

            if (!string.IsNullOrEmpty(memberParams.Search))
            {
                userQuery = userQuery.Where(c => c.UserName.Contains(memberParams.Search.ToLower()) || c.Email.Contains(memberParams.Search.ToLower()));
            }

            userQuery = memberParams.SortBy switch
            {
                "ida" => userQuery.OrderBy(u => u.Id),
                "idd" => userQuery.OrderByDescending(u => u.Id),
                "usernamea" => userQuery.OrderBy(u => u.UserName),
                "usernamed" => userQuery.OrderByDescending(u => u.UserName),
                "playernamea" => userQuery.OrderBy(u => u.PlayerName),
                "playernamed" => userQuery.OrderByDescending(u => u.PlayerName),
                "emaila" => userQuery.OrderBy(u => u.Email),
                "emaild" => userQuery.OrderByDescending(u => u.Email),
                "providera" => userQuery.OrderBy(u => u.Provider),
                "providerd" => userQuery.OrderByDescending(u => u.Provider),
                "createda" => userQuery.OrderBy(u => u.AccountCreated),
                "createdd" => userQuery.OrderByDescending(u => u.AccountCreated),
                "lastactivea" => userQuery.OrderBy(u => u.LastActive),
                "lastactived" => userQuery.OrderByDescending(u => u.LastActive),
                _ => userQuery.OrderBy(u => u.UserName)
            };

            return await PagedList<ApplicationUser>.CreateAsync(userQuery.AsNoTracking(), memberParams.PageNumber, memberParams.PageSize);
        }
    }
}
