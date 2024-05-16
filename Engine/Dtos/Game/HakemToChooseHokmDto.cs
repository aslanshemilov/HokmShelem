namespace Engine.Dtos.Game
{
    public class HakemToChooseHokmDto
    {
        public HakemToChooseHokmDto()
        {
            
        }
        public HakemToChooseHokmDto(string connectionId, string[] cards)
        {
            ConnectionId = connectionId;
            Cards = cards;
        }

        public string ConnectionId { get; set; }
        public string[] Cards { get; set; }
    }
}
