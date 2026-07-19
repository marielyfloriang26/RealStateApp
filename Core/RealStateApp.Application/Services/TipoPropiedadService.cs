using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.TipoPropiedades;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Services;

public class TipoPropiedadService : GenericService<SaveTipoPropiedadViewModel, TipoPropiedadViewModel, TipoPropiedad>, ITipoPropiedadService
{
    private readonly ITipoPropiedadRepository _tipoPropiedadRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IPropiedadFavoritaRepository _propiedadFavoritaRepository;
    private readonly IMensajeRepository _mensajeRepository;
    private readonly IOfertaRepository _ofertaRepository;
    private readonly IPropiedadMejoraRepository _propiedadMejoraRepository;
    private readonly IImagenPropiedadRepository _imagenPropiedadRepository;
    private readonly IMapper _mapper;

    public TipoPropiedadService(
        ITipoPropiedadRepository tipoPropiedadRepository, 
        IPropiedadRepository propiedadRepository,
        IPropiedadFavoritaRepository propiedadFavoritaRepository,
        IMensajeRepository mensajeRepository,
        IOfertaRepository ofertaRepository,
        IPropiedadMejoraRepository propiedadMejoraRepository,
        IImagenPropiedadRepository imagenPropiedadRepository,
        IMapper mapper) 
        : base(tipoPropiedadRepository, mapper)
    {
        _tipoPropiedadRepository = tipoPropiedadRepository;
        _propiedadRepository = propiedadRepository;
        _propiedadFavoritaRepository = propiedadFavoritaRepository;
        _mensajeRepository = mensajeRepository;
        _ofertaRepository = ofertaRepository;
        _propiedadMejoraRepository = propiedadMejoraRepository;
        _imagenPropiedadRepository = imagenPropiedadRepository;
        _mapper = mapper;
    }

    public override async Task<List<TipoPropiedadViewModel>> GetAllViewModel()
    {
        var entityList = await _tipoPropiedadRepository.GetAllWithIncludeAsync(new List<string> { "Propiedades" });
        return entityList.Select(e => new TipoPropiedadViewModel
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion,
            CantidadPropiedadesAsociadas = e.Propiedades != null ? e.Propiedades.Count : 0
        }).ToList();
    }

    public async Task<List<TipoPropiedadViewModel>> GetAllAsync()
    {
        var list = await _tipoPropiedadRepository.GetAllAsync();
        return _mapper.Map<List<TipoPropiedadViewModel>>(list);
    }

    public override async Task Delete(int id)
    {
        var propiedades = await _propiedadRepository.GetAllAsync();
        var propiedadesAsociadas = propiedades.Where(p => p.TipoPropiedadId == id).ToList();

        // 1. Delete PropiedadesFavoritas
        var favoritas = await _propiedadFavoritaRepository.GetAllAsync();
        var favoritasAsociadas = favoritas.Where(f => propiedadesAsociadas.Any(p => p.Id == f.PropiedadId)).ToList();
        foreach (var fav in favoritasAsociadas)
        {
            await _propiedadFavoritaRepository.DeleteAsync(fav);
        }

        // 2. Delete Mensajes
        var mensajes = await _mensajeRepository.GetAllAsync();
        var mensajesAsociados = mensajes.Where(m => propiedadesAsociadas.Any(p => p.Id == m.PropiedadId)).ToList();
        foreach (var msg in mensajesAsociados)
        {
            await _mensajeRepository.DeleteAsync(msg);
        }

        // 3. Delete Ofertas
        var ofertas = await _ofertaRepository.GetAllAsync();
        var ofertasAsociadas = ofertas.Where(o => propiedadesAsociadas.Any(p => p.Id == o.PropiedadId)).ToList();
        foreach (var oferta in ofertasAsociadas)
        {
            await _ofertaRepository.DeleteAsync(oferta);
        }

        // 4. Delete PropiedadMejoras
        var mejoras = await _propiedadMejoraRepository.GetAllAsync();
        var mejorasAsociadas = mejoras.Where(m => propiedadesAsociadas.Any(p => p.Id == m.PropiedadId)).ToList();
        foreach (var mejora in mejorasAsociadas)
        {
            await _propiedadMejoraRepository.DeleteAsync(mejora);
        }

        // 5. Delete Imagenes
        var imagenes = await _imagenPropiedadRepository.GetAllAsync();
        var imagenesAsociadas = imagenes.Where(i => propiedadesAsociadas.Any(p => p.Id == i.PropiedadId)).ToList();
        foreach (var img in imagenesAsociadas)
        {
            await _imagenPropiedadRepository.DeleteAsync(img);
        }

        // 6. Delete physical image folders
        foreach (var propiedad in propiedadesAsociadas)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "propiedades", propiedad.Id.ToString());
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
        }

        // 7. Delete Propiedades
        foreach (var propiedad in propiedadesAsociadas)
        {
            await _propiedadRepository.DeleteAsync(propiedad);
        }

        // 8. Delete TipoPropiedad
        await base.Delete(id);
    }
}
