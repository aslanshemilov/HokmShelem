namespace Api.IRepository
{
    public interface IHokmShelemRepository
    {
        Task<IEnumerable<VisitorMessageDto>> GetAllVisitorMessagesAsync();
        Task<IEnumerable<VisitorDto>> GetAllVisitorsAsync();
        Task<Visitor> GetVisitorByIdAsync(int id);
        Task<VisitorDto> GetVisitorDtoByIdAsync(int id);
        Task HandleVisitorAsync(string visitorIpAddress);
        void DeleteVisitor(Visitor visitor);
        Task AddMessageAsync(string visitorIpAddress, MessageAddDto model);
    }
}
