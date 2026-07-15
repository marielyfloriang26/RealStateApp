using AutoMapper;
using RealStateApp.Application.ViewModels.Mejoras;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Mappings;

public class MejoraProfile : Profile
{
    public MejoraProfile()
    {
        CreateMap<Mejora, MejoraViewModel>()
            .ForMember(dest => dest.CantidadPropiedades, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Mejora, SaveMejoraViewModel>()
            .ForMember(dest => dest.HasError, opt => opt.Ignore())
            .ForMember(dest => dest.Error, opt => opt.Ignore())
            .ReverseMap();
    }
}
