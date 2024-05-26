namespace Api.Data
{
    public class Context : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Badge> Badge { get; set; }
        public DbSet<UserStatus> UserStatus { get; set; }
        public DbSet<GameHistory> GameHistory { get; set; }
        public DbSet<Guest> Guest { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
               .HasOne(x => x.Badge)
               .WithMany(x => x.UserProfiles)
               .HasForeignKey(x => x.BadgeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
              .HasOne(x => x.Country)
              .WithMany(x => x.UserProfiles)
              .HasForeignKey(x => x.CountryId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
             .HasOne(x => x.Status)
             .WithMany(x => x.UserProfiles)
             .HasForeignKey(x => x.StatusId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

