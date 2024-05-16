namespace Api.Dtos.Admin
{
    public class VisitorMessageDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public VisitorDto Visitor { get; set; }
    }
}
