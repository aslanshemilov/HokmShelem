namespace Engine.Dtos.Game
{
    public class GameUpdateDto
    {
        public GameUpdateDto()
        {
            
        }
        public GameUpdateDto(int hIndex = 0, int wti = 0, int rsbi = 0, string hs = null)
        {
            HakemIndex = hIndex;
            WhosTurnIndex = wti;
            RoundStartsByIndex = rsbi;
            HokmSuit = hs;
        }

        public int HakemIndex { get; set; }
        public int WhosTurnIndex { get; set; }
        public int RoundStartsByIndex { get; set; }
        public string HokmSuit { get; set; }
    }
}
