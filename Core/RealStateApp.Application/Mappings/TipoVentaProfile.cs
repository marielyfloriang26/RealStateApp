using AutoMapper;
using RealStateApp.Application.ViewModels.TipoVentas;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Mappings;

public class TipoVentaProfile : Profile
{
    public TipoVentaProfile()
    {
        CreateMap<TipoVenta, TipoVentaViewModel>()
            .ForMember(dest => dest.CantidadPropiedadesAsociadas, opt => opt.MapFrom(src => src.Propiedades != null ? src.Propiedades.Count : 0))
            .ReverseMap()
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());

        CreateMap<TipoVenta, SaveTipoVentaViewModel>()
            .ReverseMap()
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());
    }
}
