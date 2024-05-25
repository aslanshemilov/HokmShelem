namespace Engine.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetPlayerName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.GivenName)?.Value;
        }
        public static int GetProfileId(this ClaimsPrincipal user)
        {
            var profileId = user.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(profileId))
            {
                return 0;
            }
            else
            {
                return int.Parse(profileId);
            }
        }
    }
}
