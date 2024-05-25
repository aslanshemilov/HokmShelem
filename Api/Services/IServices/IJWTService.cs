namespace Api.IServices
{
    public interface IJWTService
    {
        Task<string> CreateJWTAsync(AppUserToGenerateJWTDto user);
        string CreateGuestJWT(Guest guest);
    }

}
