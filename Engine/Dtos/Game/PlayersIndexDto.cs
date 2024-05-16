namespace Engine.Dtos.Game
{
    public class PlayersIndexDto
    {
        public PlayersIndexDto(int blue1Index, int red1Index, int blue2Index, int red2Index)
        {
            Blue1Index = blue1Index;
            Red1Index = red1Index;
            Blue2Index = blue2Index;
            Red2Index = red2Index;
        }

        public int Blue1Index { get; set; }
        public int Red1Index { get; set; }
        public int Blue2Index { get; set; }
        public int Red2Index { get; set; }
    }
}
