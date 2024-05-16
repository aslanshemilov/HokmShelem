namespace Engine.SignalR
{
    [Authorize]
    public class LobbyHub : Hub
    {
        private readonly IUnityRepo _unity;
        private readonly IApiService _apiService;
        private readonly IMapper _mapper;

        public LobbyHub(IUnityRepo unity,
            IApiService apiService,
            IMapper mapper)
        {
            _unity = unity;
            _apiService = apiService;
            _mapper = mapper;
        }
        public override async Task OnConnectedAsync()
        {
            var playerFromApi = _mapper.Map<Player>(await _apiService.GetPlayerInfoAsync());
            playerFromApi.ConnectionId = Context.ConnectionId;
            playerFromApi.LobbyName = SD.HSLobby;
            var oldConnectionId = _unity.PlayerRepo.AddUpdatePlayer(playerFromApi);

            // handle on multiple device connection
            if (!string.IsNullOrEmpty(oldConnectionId))
            {
                await Groups.RemoveFromGroupAsync(oldConnectionId, SD.HSLobby);
                var notification = new NotificationMessage(SM.MultipleDeviceTitle, SM.MultipleDeviceMessage, false, false);
                await NotificationForPlayerAsync(oldConnectionId, notification);
                var player = _unity.PlayerRepo.FindByName(PlayerName());
                if (player.RoomName != null)
                {
                    var room = _unity.RoomRepo.FindByName(player.RoomName, includeProperties: "Players");
                    await HandlePlayerOnRoomLeaveAsync(player, room);
                }
            }

            _unity.Complete();
            await Groups.AddToGroupAsync(Context.ConnectionId, SD.HSLobby);
            await UpdateLobbyRoomPlayerInfoAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = _unity.PlayerRepo.FindByName(Context.User.GetPlayerName(), includeProperties: "ConnectionTracker");
            if (Context.ConnectionId.Equals(player.ConnectionTracker.OldId))
            {
                await Groups.RemoveFromGroupAsync(player.ConnectionTracker.OldId, SD.HSLobby);
                player.ConnectionTracker.OldId = null;
                _unity.Complete();
            }
            else
            {
                if (player.RoomName != null)
                {
                    await HandlePlayerOnRoomLeaveAsync(player);
                }

                player.ConnectionId = null;
                player.LobbyName = null;
                _unity.Complete();
                await UpdateLobbyRoomPlayerInfoAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task UpdateLobbyRoomPlayerInfoAsync()
        {
            var players = _unity.LobbyRepo.GetFirstOrDefault(c => c.Name == SD.HSLobby, "Players").Players.ToList();
            await Clients.Group(SD.HSLobby).SendAsync("LobbyPlayers", _mapper.Map<IEnumerable<PlayerDto>>(players.OrderBy(c => c.Name)));

            var rooms = _unity.RoomRepo.GetRoomsToJoin();
            await Clients.Group(SD.HSLobby).SendAsync("AllRooms", rooms);
        }

        #region Receiving commands from client
        public async Task MessageReceived(string message, string roomName)
        {
            if (!string.IsNullOrEmpty(message.Trim()) && message.Length <= 1000)
            {
                var messageThread = new MessageThread
                {
                    From = Context.User.GetPlayerName(),
                    Message = message
                };

                if (!string.IsNullOrEmpty(roomName))
                {
                    await Clients.Group(roomName).SendAsync("NewRoomMessageReceived", messageThread);
                }
                else
                {
                    await Clients.Group(SD.HSLobby).SendAsync("NewLobbyMessageReceived", messageThread);
                }
            }
        }

        public async Task CreateRoom(RoomToCreateDto model)
        {
            var room = _unity.RoomRepo.CreateRoom(model, _unity.PlayerRepo.FindByName(Context.User.GetPlayerName()));
            if (room != null)
            {
                _unity.Complete();
                await Groups.AddToGroupAsync(ConnectionId(), model.RoomName);
                await NotificationForPlayerAsync(Context.ConnectionId, new NotificationMessage(SM.RoomCreated));
                await Clients.Caller.SendAsync("JoinTheRoom", _mapper.Map<RoomDto>(room));
                await UpdateLobbyRoomPlayerInfoAsync();
            }
            else
            {
                await NotificationForPlayerAsync(Context.ConnectionId, new NotificationMessage("Unable to create room"));
            }
        }
        public async Task JoinTheRoom(string roomName)
        {
            var room = _unity.RoomRepo.FindByName(roomName, includeProperties: "Players");
            if (room != null)
            {
                if (room.Players.Count < 4)
                {
                    var player = _unity.PlayerRepo.FindByName(PlayerName());
                    if (player.RoomName != null)
                    {
                        await HandlePlayerOnRoomLeaveAsync(player);
                    }

                    player.LobbyName = null;
                    room.Players.Add(player);
                    UnreadyAllPlayers(room);
                    _unity.Complete();

                    await Groups.AddToGroupAsync(ConnectionId(), roomName);
                    await Clients.Caller.SendAsync("JoinTheRoom", _mapper.Map<RoomDto>(room));
                    await Clients.Group(roomName).SendAsync("RoomUpdate", GetRoomDto(room));
                    await UpdateLobbyRoomPlayerInfoAsync();
                }
                else
                {
                    await NotificationForPlayerAsync(Context.ConnectionId, new NotificationMessage($"{roomName} is already full."));
                }
            }
            else
            {
                await NotificationForPlayerAsync(Context.ConnectionId, new NotificationMessage("Unable to join the room"));
            }
        }
        public async Task LeaveTheRoom(string roomName)
        {
            var room = _unity.RoomRepo.FindByName(roomName, includeProperties: "Players");
            if (room != null)
            {
                var player = room.Players.Where(p => p.Name == PlayerName()).FirstOrDefault();
                await HandlePlayerOnRoomLeaveAsync(player, room, true);
                _unity.Complete();
                await UpdateLobbyRoomPlayerInfoAsync();
            }
        }
        public async Task SelectSit(string roomName, string selectedSitNumber)
        {
            var room = _unity.RoomRepo.FindByName(roomName);
            var playerName = PlayerName();
            var currentPlayerSit = GetCurrentSitNumberForThePlayer(room, playerName);
            bool sitSelectedSuccess = false;

            switch (selectedSitNumber)
            {
                case "b1":
                    if (room.Blue1 == null)
                    {
                        room.Blue1 = playerName;
                        sitSelectedSuccess = true;
                    }
                    else if (currentPlayerSit == "b1")
                    {
                        sitSelectedSuccess = true;
                    }
                    break;
                case "r1":
                    if (room.Red1 == null)
                    {
                        room.Red1 = playerName;
                        sitSelectedSuccess = true;
                    }
                    else if (currentPlayerSit == "r1")
                    {
                        sitSelectedSuccess = true;
                    }
                    break;
                case "b2":
                    if (room.Blue2 == null)
                    {
                        room.Blue2 = playerName;
                        sitSelectedSuccess = true;
                    }
                    else if (currentPlayerSit == "b2")
                    {
                        sitSelectedSuccess = true;
                    }
                    break;
                case "r2":
                    if (room.Red2 == null)
                    {
                        room.Red2 = playerName;
                        sitSelectedSuccess = true;
                    }
                    else if (currentPlayerSit == "r2")
                    {
                        sitSelectedSuccess = true;
                    }
                    break;
                default:
                    break;
            }

            if (sitSelectedSuccess)
            {
                if (!string.IsNullOrEmpty(currentPlayerSit))
                {
                    UnselectCurrentPlayerSit(room, currentPlayerSit);
                }

                UnreadyAllPlayers(room);
                _unity.Complete();
                await Clients.Group(roomName).SendAsync("RoomUpdate", GetRoomDto(room));
            }
            else
            {
                await NotificationForPlayerAsync(ConnectionId(), new NotificationMessage(SM.SitIsTakenMessage, isSuccess: false));
            }
        }
        public async Task ReadyButtonClicked(string roomName)
        {
            var room = _unity.RoomRepo.FindByName(roomName);
            string playerName = PlayerName();
            if (room.Blue1 == playerName)
                room.Blue1Ready = true;
            else if (room.Red1 == playerName)
                room.Red1Ready = true;
            else if (room.Blue2 == playerName)
                room.Blue2Ready = true;
            else if (room.Red2 == playerName)
                room.Red2Ready = true;

            _unity.Complete();
            await Clients.Group(roomName).SendAsync("RoomUpdate", GetRoomDto(room));
        }

        public async Task StartTheGame(string roomName)
        {
            var room = _unity.RoomRepo.FindByName(roomName, includeProperties: "Players");
            foreach(var player in room.Players)
            {
                player.RoomName = null;
            }

            _unity.RoomRepo.Remove(room);
            _unity.Complete();
            await Clients.Group(roomName).SendAsync("GameAboutToStart", 5);
        }
        #endregion


        #region Private Helper Methods
        private string PlayerName()
        {
            return Context.User.GetPlayerName();
        }
        private string ConnectionId()
        {
            return Context.ConnectionId;
        }
        private async Task NotificationForPlayerAsync(string connectionId, NotificationMessage notification)
        {
            await Clients.Client(connectionId).SendAsync("NotificationMessage", notification);
        }
        private async Task HandlePlayerOnRoomLeaveAsync(Player player, Room room = null, bool assignToLobby = false)
        {
            if (player != null && player.RoomName != null)
            {
                if (room == null)
                {
                    room = _unity.RoomRepo.FindByName(player.RoomName, includeProperties: "Players");
                }

                if (room == null)
                {
                    player.RoomName = null;
                    return;
                }

                if (room.HostName.Equals(player.Name))
                {
                    await KickAllAndDisplayHostLeftNotification(room, room.Players, room.HostName);
                }
                else
                {
                    var currentPlayerSit = GetCurrentSitNumberForThePlayer(room, player.Name);
                    if (!string.IsNullOrEmpty(currentPlayerSit)) UnselectCurrentPlayerSit(room, currentPlayerSit);
                    room.Players.Remove(player);
                    if (assignToLobby)
                    {
                        player.LobbyName = SD.HSLobby;
                    }
                    await Groups.RemoveFromGroupAsync(ConnectionId(), room.Name);
                    UnreadyAllPlayers(room);
                    await Clients.Group(room.Name).SendAsync("RoomUpdate", GetRoomDto(room));
                }
            }
        }
        private async Task KickAllAndDisplayHostLeftNotification(Room room, ICollection<Player> players, string hostName)
        {
            foreach (var p in players)
            {
                p.RoomName = null;
                p.LobbyName = SD.HSLobby;
                await Groups.RemoveFromGroupAsync(p.ConnectionId, room.Name);
                if (!p.Name.Equals(hostName))
                {
                    await NotificationForPlayerAsync(p.ConnectionId, new NotificationMessage("Host Left", "Host has left the room!", isSuccess: false, useToastr: false));
                }
            }

            _unity.RoomRepo.Remove(room);
        }
        private string GetCurrentSitNumberForThePlayer(Room room, string playerName)
        {
            if (room.Blue1 == playerName)
            {
                return "b1";
            }
            else if (room.Red1 == playerName)
            {
                return "r1";
            }
            else if (room.Blue2 == playerName)
            {
                return "b2";
            }
            else if (room.Red2 == playerName)
            {
                return "r2";
            }
            else
            {
                return null;
            }
        }
        private void UnselectCurrentPlayerSit(Room room, string currentPlayerSit)
        {
            switch (currentPlayerSit)
            {
                case "b1":
                    room.Blue1 = null;
                    break;
                case "r1":
                    room.Red1 = null;
                    break;
                case "b2":
                    room.Blue2 = null;
                    break;
                case "r2":
                    room.Red2 = null;
                    break;
            }
        }
        private bool CheckReadyButtonEnabled(Room room)
        {
            if (room.Blue1 == null) return false;
            if (room.Red1 == null) return false;
            if (room.Blue2 == null) return false;
            if (room.Red2 == null) return false;
            return true;
        }
        private void UnreadyAllPlayers(Room room)
        {
            room.Blue1Ready = false;
            room.Red1Ready = false;
            room.Blue2Ready = false;
            room.Red2Ready = false;
        }
        private RoomDto GetRoomDto(Room room)
        {
            var roomDto = _mapper.Map<RoomDto>(room);
            roomDto.ReadyButtonEnabled = CheckReadyButtonEnabled(room);
            return roomDto;
        }
        #endregion
    }
}
