using Application.Features.Courts.Commands;
using AutoMapper;
using Shared.Courts;

public class CourtMapping : Profile
{
    public CourtMapping()
    {
        CreateMap<Court, CourtDTO>()
            .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.Facility.Name))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ReverseMap(); // Nếu muốn ánh xạ ngược lại

        CreateMap<CreateCourtCommand, Court>();
        CreateMap<UpdateCourtCommand, Court>();
    }
}
