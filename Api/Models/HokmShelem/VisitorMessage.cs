namespace Api.Models.HokmShelem
{
    public class VisitorMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [ForeignKey("Visitor")]
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }
    }
}
