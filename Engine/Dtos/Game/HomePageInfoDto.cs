namespace Engine.Dtos.Game
{
    public class HomePageInfoDto
    {
        public HomePageInfoDto()
        {
            
        }
        public HomePageInfoDto(int playersCount, int gamesCount)
        {
            PlayersCount = playersCount;
            GamesCount = gamesCount;
        }

        public int PlayersCount { get; set; }
        public int GamesCount { get; set; }
    }
}
