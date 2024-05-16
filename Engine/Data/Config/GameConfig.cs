namespace Engine.Data.Config
{
    public class GameConfig : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasKey(r => r.Name);

            builder.Property(s => s.Blue1Status).HasConversion(
                  o => o.ToString(),
                  o => (SD.PlayerInGameStatus)Enum.Parse(typeof(SD.PlayerInGameStatus), o));
            builder.Property(s => s.Red1Status).HasConversion(
                  o => o.ToString(),
                  o => (SD.PlayerInGameStatus)Enum.Parse(typeof(SD.PlayerInGameStatus), o));
            builder.Property(s => s.Blue2Status).HasConversion(
                  o => o.ToString(),
                  o => (SD.PlayerInGameStatus)Enum.Parse(typeof(SD.PlayerInGameStatus), o));
            builder.Property(s => s.Red2Status).HasConversion(
                  o => o.ToString(),
                  o => (SD.PlayerInGameStatus)Enum.Parse(typeof(SD.PlayerInGameStatus), o));
        }
    }
}
