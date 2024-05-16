namespace Api.Dtos.HokmShelem
{
    public class VisitorDto
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int NumberOfVisit { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool Is_Proxy { get; set; }
        public string LastVisit { get; set; }

        public List<MessageAddDto> Messages { get; set; } = new List<MessageAddDto>();
    }
}
