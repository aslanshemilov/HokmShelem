namespace Api.Models
{
    public class GameHistoryUserProfileBridge
    {
        public int GameHistoryId { get; set; }
        public GameHistory GameHistory { get; set; }

        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
