using AutoMapper;
using RealStateApp.Application.ViewModels.TipoPropiedades;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Mappings;

public class TipoPropiedadProfile : Profile
{
    public TipoPropiedadProfile()
    {
        CreateMap<TipoPropiedad, TipoPropiedadViewModel>()
            .ForMember(dest => dest.CantidadPropiedadesAsociadas, opt => opt.MapFrom(src => src.Propiedades != null ? src.Propiedades.Count : 0))
            .ReverseMap()
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());

        CreateMap<TipoPropiedad, SaveTipoPropiedadViewModel>()
            .ReverseMap()
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());
    }
}
