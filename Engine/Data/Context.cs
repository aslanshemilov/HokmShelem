namespace Engine.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Player> Player { get; set; }
        public DbSet<ConnectionTracker> ConnectionTracker { get; set; }
        public DbSet<Lobby> Lobby { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<Card> Card { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
