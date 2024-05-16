namespace Api.Models.HokmShelem
{
    public class Visitor
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int NumberOfVisit { get; set; } = 1;
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool Is_Proxy { get; set; }
        public DateTime LastVisit { get; set; } = DateTime.UtcNow;

        public ICollection<VisitorMessage> Messsages { get; set; } = new List<VisitorMessage>();
    }
}
