namespace Engine.Entities
{
    public class ConnectionTracker : BaseEntity
    {
        public string CurrentId { get; set; }
        public string OldId { get; set; }
        public Player Player { get; set; }
    }
}
