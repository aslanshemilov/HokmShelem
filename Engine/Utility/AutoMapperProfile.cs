namespace Engine.Utility
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Player
            CreateMap<PlayerDto, Player>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlayerName));

            CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Player, Player>();

            // Room
            CreateMap<Room, RoomToJoinDto>()
               .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players.Select(p => p.Name)));

            CreateMap<Room, RoomDto>()
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Name));

            // Game
            CreateMap<Game, CurrentGameDto>();
            CreateMap<Room, Game>();
            CreateMap<Game, GameInfoDto>()
                .ForMember(dest => dest.GameName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
