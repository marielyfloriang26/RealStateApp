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
    private readonly IMapper _mapper;

    public TipoPropiedadService(ITipoPropiedadRepository tipoPropiedadRepository, IPropiedadRepository propiedadRepository, IMapper mapper) 
        : base(tipoPropiedadRepository, mapper)
    {
        _tipoPropiedadRepository = tipoPropiedadRepository;
        _propiedadRepository = propiedadRepository;
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

    public async Task<List<RealStateApp.Application.ViewModels.Propiedad.TipoPropiedadViewModel>> GetAllAsync()
    {
        var list = await _tipoPropiedadRepository.GetAllAsync();
        return _mapper.Map<List<RealStateApp.Application.ViewModels.Propiedad.TipoPropiedadViewModel>>(list);
    }

    public override async Task Delete(int id)
    {
        var propiedades = await _propiedadRepository.GetAllAsync();
        var propiedadesAsociadas = propiedades.Where(p => p.TipoPropiedadId == id).ToList();

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
