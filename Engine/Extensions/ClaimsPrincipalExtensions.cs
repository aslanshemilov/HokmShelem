namespace Engine.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetPlayerName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.GivenName)?.Value;
        }
    }
}
