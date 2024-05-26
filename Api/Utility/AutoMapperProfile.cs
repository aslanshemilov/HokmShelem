namespace Api.Utility
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Admin
            CreateMap<ApplicationUser, A_MemberViewDto>();

            // HokmShelem
            CreateMap<VisitorMessage, VisitorMessageDto>();
            CreateMap<Visitor, VisitorDto>();
            CreateMap<MessageAddDto, VisitorMessage>().ReverseMap();

            // Badge
            CreateMap<BadgeModel, Badge>();

            // Country
            CreateMap<Country, CountryDto>();

            // ApplicationUser
            CreateMap<ApplicationUser, AppUserToGenerateJWTDto>()
                .ForMember(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserProfileId, opt => opt.MapFrom(src => src.UserProfile.Id))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.UserProfile.Photo.PhotoUrl));
            CreateMap<RegisterDto, ApplicationUserAddDto>();
            CreateMap<RegisterWithExternal, ApplicationUserAddDto>();


            // UserProfile
            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.ApplicationUser.PlayerName))
                .ForMember(dest => dest.AccountCreated, opt => opt.MapFrom(src => src.ApplicationUser.AccountCreated))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
                .ForMember(dest => dest.BadgeColor, opt => opt.MapFrom(src => src.Badge.Color));
            CreateMap<UserProfileUpdateDto, UserProfile>();

            // Player
            CreateMap<UserProfile, PlayerDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.ApplicationUser.PlayerName))
                .ForMember(dest => dest.Badge, opt => opt.MapFrom(src => src.Badge.Color))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo.PhotoUrl))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name));

            // GameHistory
            CreateMap<GameHistoryDto, GameHistory>().ReverseMap();

            // fixing DateTime.Utc in Utc format to have Z
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
        }
    }
}
