namespace Engine.Data.Config
{
    public class ConnectionTrackerConfig : IEntityTypeConfiguration<ConnectionTracker>
    {
        public void Configure(EntityTypeBuilder<ConnectionTracker> builder)
        {
            builder.HasKey(r => r.Name);
            builder.HasOne(x => x.Player)
                .WithOne(x => x.ConnectionTracker)
                .HasForeignKey<ConnectionTracker>(x => x.Name)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
