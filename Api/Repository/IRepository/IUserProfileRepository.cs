namespace Api.IRepository
{
    public interface IUserProfileRepository
    {
        Task<bool> AnyUserProfileAsync(int userId);
        Task CreateUserProfileAsync(ApplicationUserAddDto user);
        Task<int> GetUserProfileIdByUserIdAsync(int userId);
        Task<UserProfile> GetUserProfileToUpdateAsync(int userProfileId);
        Task<UserProfileDto> GetUserProfileAsync(int userProfileId);
        Task UpdateUserProfileAsync(UserProfileUpdateDto model, int userProfileId);
        Task<PlayerDto> GetPlayerInfoAsync(int userProfileId);
    }
}
