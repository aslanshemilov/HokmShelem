namespace Engine.Dtos
{
    public class HubResult
    {
        public HubResult()
        {
            Success = true;
        }
        public HubResult(bool success, string connectionId)
        {
            Success = success;
            ConnectionId = connectionId;
        }

        public bool Success { get; set; }
        public string ConnectionId { get; set; }
    }
}
