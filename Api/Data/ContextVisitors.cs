namespace Api.Data
{
    public class ContextVisitors : DbContext
    {
        public ContextVisitors(DbContextOptions<ContextVisitors> options) : base(options)
        {
            
        }

        public DbSet<Visitor> Visitor { get; set; }
        public DbSet<VisitorMessage> VisitorMessage { get; set; }
    }
}
