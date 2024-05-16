namespace Engine.Data.Config
{
    public class PlayerConfig : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(o => o.Name);

            builder.HasOne(o => o.Lobby).WithMany(l => l.Players).HasForeignKey(o => o.LobbyName).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(o => o.Room).WithMany(r => r.Players).HasForeignKey(o => o.RoomName).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(o => o.Game).WithMany(g => g.Players).HasForeignKey(o => o.GameName).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
