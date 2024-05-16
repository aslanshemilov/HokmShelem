namespace Engine.Entities
{
    public class BaseEntity
    {
        [Key]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
