namespace Application.AutoMapper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
        CreateMap<App, App_DTO>()
            .ForMember(
                dest => dest.AddedBy,
                opt => opt.MapFrom(src => (src.AddedBy == null) ? "Unknown" : src.AddedBy.Username)
            );

        CreateMap<App_DTO, App>()
            .ForMember(
                dest => dest.AddedBy,
                opt => opt.Ignore()
            );

        CreateMap<User, User_DTO>();

        CreateMap<User_DTO, User>()
            .ForMember(
                dest => dest.PasswordHash,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.PasswordSalt,
                opt => opt.Ignore()
            );
    }
}

