namespace Api.Repository.IRepository
{
    public interface IAdminRepository
    {
        Task<PagedList<ApplicationUser>> GetMembersAsync(A_MemberParams memberParams);
    }
}
