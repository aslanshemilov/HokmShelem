namespace Engine.Entities
{
    public class Lobby : BaseEntity
    {
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
