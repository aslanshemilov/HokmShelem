using AutoMapper.QueryableExtensions;

namespace Engine.Repository
{
    public class RoomRepo : BaseRepo<Room>, IRoomRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public RoomRepo(Context context,
            IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public Room CreateRoom(RoomToCreateDto model, Player host)
        {
            if (AnyByName(model.RoomName)) return null;

            if (model.GameType == SD.Hokm)
            {
                if (model.TargetScore < SD.HokmMinScore && model.TargetScore > SD.HokmMaxScore) return null;
            }

            if (model.GameType == SD.Shelem)
            {
                if (model.TargetScore < SD.ShelemMinScore && model.TargetScore > SD.ShelemMaxScore) return null;
            }

            var room = new Room() { Name = model.RoomName, HostName = host.Name, TargetScore = model.TargetScore, GameType = model.GameType };
            host.RoomName = room.Name;
            host.LobbyName = null;
            Add(room);
            return room;
        }

        public IEnumerable<RoomToJoinDto> GetRoomsToJoin()
        {
            return _context.Room
                .ProjectTo<RoomToJoinDto>(_mapper.ConfigurationProvider)
                .OrderBy(r => r.Created)
                .ToList();
        }
    }
}
