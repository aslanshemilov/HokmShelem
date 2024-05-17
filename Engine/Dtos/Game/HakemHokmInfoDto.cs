namespace Engine.Dtos.Game
{
    public class HakemHokmInfoDto
    {
        public HakemHokmInfoDto()
        {
            
        }
        public HakemHokmInfoDto(string hakemConnectionId, List<string> otherPlayersConnectionIds, string[] cards)
        {
            HakemConnectionId = hakemConnectionId;
            OtherPlayersConnectionId = otherPlayersConnectionIds;
            Cards = cards;
        }

        public List<string> OtherPlayersConnectionId { get; set; }
        public string HakemConnectionId { get; set; }
        public string[] Cards { get; set; }
    }
}
