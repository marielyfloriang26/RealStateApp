using AutoMapper;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Application.ViewModels.TipoPropiedades;
using RealStateApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RealStateApp.Application.Mappings;

public class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        CreateMap<Propiedad, PropiedadViewModel>()
            .ForMember(dest => dest.TipoPropiedadNombre, opt => opt.MapFrom(src => src.TipoPropiedad != null ? src.TipoPropiedad.Nombre : ""))
            .ForMember(dest => dest.TipoVentaNombre, opt => opt.MapFrom(src => src.TipoVenta != null ? src.TipoVenta.Nombre : ""))
            .ForMember(dest => dest.AgenteNombre, opt => opt.MapFrom(src => src.Agente != null ? src.Agente.Nombre : ""))
            .ForMember(dest => dest.AgenteApellido, opt => opt.MapFrom(src => src.Agente != null ? src.Agente.Apellido : ""))
            .ForMember(dest => dest.AgenteTelefono, opt => opt.MapFrom(src => src.Agente != null ? src.Agente.PhoneNumber : ""))
            .ForMember(dest => dest.AgenteFotoUrl, opt => opt.MapFrom(src => src.Agente != null ? src.Agente.FotoUrl : ""))
            .ForMember(dest => dest.AgenteCorreo, opt => opt.MapFrom(src => src.Agente != null ? src.Agente.Email : ""))
            .ForMember(dest => dest.ImagenesUrl, opt => opt.MapFrom(src => src.Imagenes != null ? src.Imagenes.Select(i => i.ImagenUrl).ToList() : new List<string>()))
            .ForMember(dest => dest.Mejoras, opt => opt.MapFrom(src => src.PropiedadMejoras != null ? src.PropiedadMejoras.Select(pm => pm.Mejora != null ? pm.Mejora.Nombre : "").ToList() : new List<string>()));

        CreateMap<TipoPropiedad, TipoPropiedadViewModel>().ReverseMap();

        // Mapeo para Agentes
        CreateMap<Usuario, AgenteViewModel>().ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.PhoneNumber)).ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.Email)).ForMember(dest => dest.CantidadPropiedades, opt => opt.MapFrom(src => src.Propiedades != null ? src.Propiedades.Count(p => p.Estado == "Disponible") : 0));
    }
}