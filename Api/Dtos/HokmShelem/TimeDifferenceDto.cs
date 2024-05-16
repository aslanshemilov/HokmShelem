namespace Api.Dtos.HokmShelem
{
    public class TimeDifferenceDto
    {
        public TimeDifferenceDto()
        {
            
        }
        public TimeDifferenceDto(int days, int hours, int minutes)
        {
            Days = days;
            Hours = hours;
            Minutes = minutes;
        }

        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }
}
