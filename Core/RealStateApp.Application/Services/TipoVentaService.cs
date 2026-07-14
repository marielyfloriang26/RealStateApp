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

    public TipoVentaService(ITipoVentaRepository tipoVentaRepository, IPropiedadRepository propiedadRepository, IMapper mapper) 
        : base(tipoVentaRepository, mapper)
    {
        _tipoVentaRepository = tipoVentaRepository;
        _propiedadRepository = propiedadRepository;
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

        foreach (var propiedad in propiedadesAsociadas)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "propiedades", propiedad.Id.ToString());
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
        }

        await base.Delete(id);
    }
}
