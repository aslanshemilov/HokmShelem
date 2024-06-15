using System.Security.AccessControl;

namespace Engine.Data
{
    public static class SeedContext
    {
        public static async Task InitializeAsync(Context context,
            IMapper mapper)
        {
            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            if (!context.Lobby.Any())
            {
                var lobby = new Lobby
                {
                    Name = SD.HSLobby
                };

                context.Lobby.Add(lobby);
            }

            if (context.Player.Any())
            {
                var players = await context.Player.ToListAsync();
                context.RemoveRange(players);
            }

            if (context.Room.Any())
            {
                var rooms = await context.Room.ToListAsync();
                context.RemoveRange(rooms);
            }

            if (context.Game.Any())
            {
                var games = await context.Game.ToListAsync();
                context.RemoveRange(games);
            }

            if (context.Card.Any())
            {
                var cards = await context.Card.ToListAsync();
                context.RemoveRange(cards);
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
            }

            // adding a test game
            //if (!context.Player.Any())
            //{
            //    var kim = new Player
            //    {
            //        Name = "Kim",
            //        Badge = "grey",
            //        Rate = 1854,
            //        HokmScore = 474,
            //        ShelemScore = 1806826,
            //        GamesWon = 6101,
            //        GamesLost = 3323,
            //        GamesLeft = 580,
            //        PhotoUrl = "https://randomuser.me/api/portraits/men/50.jpg",
            //        Country = "New-Zealand"
            //    };

            //    var todd = new Player
            //    {
            //        Name = "Todd",
            //        Badge = "pink",
            //        Rate = 0,
            //        HokmScore = 0,
            //        ShelemScore = 0,
            //        GamesWon = 0,
            //        GamesLost = 0,
            //        GamesLeft = 0,
            //        PhotoUrl = "https://randomuser.me/api/portraits/men/9.jpg",
            //        Country = "Canada"
            //    };

            //    var wong = new Player
            //    {
            //        Name = "Wong",
            //        Badge = "gold",
            //        Rate = 5076,
            //        HokmScore = 2547,
            //        ShelemScore = 2266965,
            //        GamesWon = 6653,
            //        GamesLost = 7471,
            //        GamesLeft = 4317,
            //        PhotoUrl = "https://randomuser.me/api/portraits/men/85.jpg",
            //        Country = "United-States-of-America",
            //    };

            //    var miler = new Player
            //    {
            //        Name = "Miller",
            //        Badge = "gold",
            //        Rate = 6097,
            //        HokmScore = 1068,
            //        ShelemScore = 3765630,
            //        GamesWon = 5004,
            //        GamesLost = 6118,
            //        GamesLeft = 4598,
            //        PhotoUrl = "https://randomuser.me/api/portraits/men/28.jpg",
            //        Country = "United-States-of-America",
            //    };

            //    var game = new Game
            //    {
            //        Name = "kim",
            //        GameType = "shelem",
            //        TargetScore = 50,
            //        Blue1 = "Kim",
            //        Red1 = "Todd",
            //        Blue2 = "Wong",
            //        Red2 = "Miller"
            //    };

            //    game.Players.Add(kim);
            //    game.Players.Add(todd);
            //    game.Players.Add(wong);
            //    game.Players.Add(miler);

            //    context.Game.Add(game);

            //    await context.SaveChangesAsync();
            //}
        }
    }
}
