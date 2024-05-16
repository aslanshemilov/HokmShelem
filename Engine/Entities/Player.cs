namespace Engine.Entities
{
    public class Player : BaseEntity
    {
        public string ConnectionId { get; set; }
        public string Badge { get; set; }
        public int Rate { get; set; }
        public int HokmScore { get; set; }
        public int ShelemScore { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int GamesLeft { get; set; }
        public string PhotoUrl { get; set; }
        public string Country { get; set; }

        public string LobbyName { get; set; }
        public Lobby Lobby { get; set; }

        public string RoomName { get; set; }
        public Room Room { get; set; }

        public string GameName { get; set; }
        public Game Game { get; set; }

        public ConnectionTracker ConnectionTracker { get; set; }
    }
}
