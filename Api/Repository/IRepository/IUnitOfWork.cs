namespace Api.IRepository
{
    public interface IUnitOfWork
    {
        IAdminRepository AdminRepository { get; }
        IHokmShelemRepository HokmShelemRepository { get; }
        IContactUsRepository ContactUsRepository { get; }
        IBadgeRepository BadgeRepository { get; }
        ICountryRepository CountryRepository { get; }
        IUserStatusRepository UserStatusRepository { get; }
        IUserProfileRepository UserProfileRepository { get; }
        Task<bool> CompleteAsync();
        bool HasChanges();
    }
}
