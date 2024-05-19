namespace Engine.Dtos.Game
{
    public class HakemCardsToHokm
    {
        public HakemCardsToHokm()
        {
            
        }
        public HakemCardsToHokm(string hakemConnectionId, string[] cards)
        {
            HakemConnectionId = hakemConnectionId;
            Cards = cards;
        }

        public string HakemConnectionId { get; set; }
        public string[] Cards { get; set; }
    }
}
