namespace Engine.Dtos.Game
{
    public class GameUpdateDto
    {
        public GameUpdateDto()
        {
            
        }
        public GameUpdateDto(int hakemIndex = 0, int whosTurnIndex = 0, int roundStartsByIndex = 0, string hokmSuit = null, int claimStartsByIndex = 0)
        {
            HakemIndex = hakemIndex;
            WhosTurnIndex = whosTurnIndex;
            RoundStartsByIndex = roundStartsByIndex;
            HokmSuit = hokmSuit;
            ClaimStartsByIndex = claimStartsByIndex;
        }

        public int HakemIndex { get; set; }
        public int WhosTurnIndex { get; set; }
        public int RoundStartsByIndex { get; set; }
        public string HokmSuit { get; set; }
        public int ClaimStartsByIndex { get; set; }
    }
}
