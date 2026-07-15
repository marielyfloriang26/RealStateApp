using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.TipoVentas;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Services;

public class TipoVentaService : GenericService<SaveTipoVentaViewModel, TipoVentaViewModel, TipoVenta>, ITipoVentaService
{
    private readonly ITipoVentaRepository _tipoVentaRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IPropiedadMejoraRepository _propiedadMejoraRepository;
    private readonly IImagenPropiedadRepository _imagenPropiedadRepository;
    private readonly IPropiedadFavoritaRepository _propiedadFavoritaRepository;
    private readonly IOfertaRepository _ofertaRepository;
    private readonly IMensajeRepository _mensajeRepository;

    public TipoVentaService(ITipoVentaRepository tipoVentaRepository, IPropiedadRepository propiedadRepository, 
        IPropiedadMejoraRepository propiedadMejoraRepository, IImagenPropiedadRepository imagenPropiedadRepository,
        IPropiedadFavoritaRepository propiedadFavoritaRepository, IOfertaRepository ofertaRepository,
        IMensajeRepository mensajeRepository, IMapper mapper) 
        : base(tipoVentaRepository, mapper)
    {
        _tipoVentaRepository = tipoVentaRepository;
        _propiedadRepository = propiedadRepository;
        _propiedadMejoraRepository = propiedadMejoraRepository;
        _imagenPropiedadRepository = imagenPropiedadRepository;
        _propiedadFavoritaRepository = propiedadFavoritaRepository;
        _ofertaRepository = ofertaRepository;
        _mensajeRepository = mensajeRepository;
    }

    public override async Task<List<TipoVentaViewModel>> GetAllViewModel()
    {
        var entityList = await _tipoVentaRepository.GetAllWithIncludeAsync(new List<string> { "Propiedades" });
        return entityList.Select(e => new TipoVentaViewModel
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion,
            CantidadPropiedadesAsociadas = e.Propiedades != null ? e.Propiedades.Count : 0
        }).ToList();
    }

    public override async Task Delete(int id)
    {
        var propiedades = await _propiedadRepository.GetAllAsync();
        var propiedadesAsociadas = propiedades.Where(p => p.TipoVentaId == id).ToList();

        var mejoras = await _propiedadMejoraRepository.GetAllAsync();
        var imagenes = await _imagenPropiedadRepository.GetAllAsync();
        var favoritas = await _propiedadFavoritaRepository.GetAllAsync();
        var ofertas = await _ofertaRepository.GetAllAsync();
        var mensajes = await _mensajeRepository.GetAllAsync();

        foreach (var propiedad in propiedadesAsociadas)
        {
            var propMejoras = mejoras.Where(m => m.PropiedadId == propiedad.Id).ToList();
            foreach (var propMejora in propMejoras)
            {
                await _propiedadMejoraRepository.DeleteAsync(propMejora);
            }

            var propImagenes = imagenes.Where(i => i.PropiedadId == propiedad.Id).ToList();
            foreach (var propImagen in propImagenes)
            {
                await _imagenPropiedadRepository.DeleteAsync(propImagen);
            }

            var propFavoritas = favoritas.Where(f => f.PropiedadId == propiedad.Id).ToList();
            foreach (var propFavorita in propFavoritas)
            {
                await _propiedadFavoritaRepository.DeleteAsync(propFavorita);
            }

            var propOfertas = ofertas.Where(o => o.PropiedadId == propiedad.Id).ToList();
            foreach (var propOferta in propOfertas)
            {
                await _ofertaRepository.DeleteAsync(propOferta);
            }

            var propMensajes = mensajes.Where(m => m.PropiedadId == propiedad.Id).ToList();
            foreach (var propMensaje in propMensajes)
            {
                await _mensajeRepository.DeleteAsync(propMensaje);
            }

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "propiedades", propiedad.Id.ToString());
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }

            await _propiedadRepository.DeleteAsync(propiedad);
        }

        await base.Delete(id);
    }
}
