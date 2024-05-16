using Mailjet.Client.Resources;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Authorize(Roles = SD.Admin)]
    public class AdminController : ApiCoreController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("messages")]
        public async Task<ActionResult<IEnumerable<VisitorMessageDto>>> GetMessages()
        {
            return Ok(await UnitOfWork.HokmShelemRepository.GetAllVisitorMessagesAsync());
        }

        [HttpGet("visitors")]
        public async Task<ActionResult<IEnumerable<VisitorDto>>> GetVisitors()
        {
            return Ok(await UnitOfWork.HokmShelemRepository.GetAllVisitorsAsync());
        }

        [HttpGet("visitor/{id}")]
        public async Task<ActionResult<VisitorDto>> GetVisitor(int id)
        {
            var visitor = await UnitOfWork.HokmShelemRepository.GetVisitorDtoByIdAsync(id);
            if (visitor == null) return NotFound();
            return visitor;
        }

        [HttpDelete("visitor/{id}")]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            var visitor = await UnitOfWork.HokmShelemRepository.GetVisitorByIdAsync(id);
            if (visitor == null) return NotFound();

            UnitOfWork.HokmShelemRepository.DeleteVisitor(visitor);
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("get-dropdown-filters")]
        public async Task<ActionResult<A_DropdownFilters>> GetDropdownFilters()
        {
            var toReturn = new A_DropdownFilters();
            toReturn.Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            toReturn.Roles.Insert(0, "all");

            toReturn.Statuses = await Context.UserStatus.Select(x => x.Name).ToListAsync();
            toReturn.Statuses.Insert(0, "all");

            toReturn.Providers = new List<string> { "all", SD.Google, SD.Facebook };

            return Ok(toReturn);
        }

        [HttpGet("get-members")]
        public async Task<ActionResult<PagedList<A_MemberViewDto>>> GetMembers([FromQuery] A_MemberParams memberParams)
        {
            memberParams.CurrentUsername = User.GetUserName();
            var users = await UnitOfWork.AdminRepository.GetMembersAsync(memberParams);
            var items = Mapper.Map<PagedList<A_MemberViewDto>>(users);
            var members = new PagedList<A_MemberViewDto>(items);

            foreach (var member in members)
            {
                member.IsLocked = await _userManager.IsLockedOutAsync(users.FirstOrDefault(c => c.UserName == member.UserName));
                member.Roles = await _userManager.GetRolesAsync(users.FirstOrDefault(c => c.UserName == member.UserName));
            }

            return Ok(new PaginatedResult<A_MemberViewDto>(users.TotalItemsCount, users.PageNumber, users.PageSize, users.TotalPages, members));
        }

        [HttpGet("get-member/{id}")]
        public async Task<ActionResult<GetMemberDto>> GetMember(int id)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var member = new GetMemberDto
            {
                Id = user.Id,
                UserName = user.UserName,
                PlayerName = user.PlayerName,
                Provider = user.Provider,
                Email = user.Email
            };

            return Ok(member);
        }

        // Create an action and name it as "Uptade-User" which takes Id for the ApplicationUserId and update Username and PlayerName and Email
        [HttpPut("update-member")]
        public async Task<IActionResult> UptadeMember(UpdateMemberDto dto)
        {
            var exmaple = _userManager.Users.SingleOrDefault(c => c.Id == dto.Id);

            if (exmaple == null) return NotFound();

            exmaple.UserName = dto.UserName;
            exmaple.PlayerName = dto.PlayerName;
            exmaple.Email = dto.Email;
            exmaple.Provider = dto.Provider;
            exmaple.EmailConfirmed = dto.EmailConfirme;

            if (dto.NewPassword != null)
            {
                var decodedToken = await _userManager.GeneratePasswordResetTokenAsync(exmaple);
                var result = await _userManager.ResetPasswordAsync(exmaple, decodedToken, dto.NewPassword);
            }
            

            await _userManager.UpdateAsync(exmaple);


            return Ok(new ApiResponse(200, title: "Save", message: "your change is saved"));

        }
    }
}
