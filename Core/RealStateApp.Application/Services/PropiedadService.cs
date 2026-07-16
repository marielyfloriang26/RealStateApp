using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Application.Services;

public class PropiedadService : IPropiedadService
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IMapper _mapper;

    public PropiedadService(IPropiedadRepository propiedadRepository, IMapper mapper)
    {
        _propiedadRepository = propiedadRepository;
        _mapper = mapper;
    }

    public async Task<List<PropiedadViewModel>> GetAllWithIncludeAsync()
    {
        var propiedades = await _propiedadRepository.GetAllWithIncludeAsync();
        // Solo muestra propiedades en estado Disponible, de la mas reciente a la mas antigua
        var disponibles = propiedades.Where(p => p.Estado == "Disponible").OrderByDescending(p => p.FechaCreacion)
        .ToList();
        return _mapper.Map<List<PropiedadViewModel>>(disponibles);
    }

    public async Task<PropiedadViewModel?> GetByIdWithIncludeAsync(int id)
    {
        var propiedad = await _propiedadRepository.GetByIdWithIncludeAsync(id);
        if (propiedad == null || propiedad.Estado != "Disponible")
        {
            return null;
        }
        return _mapper.Map<PropiedadViewModel>(propiedad);
    }

    public async Task<PropiedadViewModel?> GetByCodeWithIncludeAsync(string code)
    {
        var propiedades = await _propiedadRepository.GetAllWithIncludeAsync();
        var propiedad = propiedades.FirstOrDefault(p => p.Codigo == code && p.Estado == "Disponible");
        if (propiedad == null)
        {
            return null;
        }
        return _mapper.Map<PropiedadViewModel>(propiedad);
    }

    public async Task<List<PropiedadViewModel>> GetAllFilteredAsync(FiltroPropiedadViewModel filter)
    {
        var list = await GetAllWithIncludeAsync();

        if (filter.TipoPropiedadId.HasValue)
        {
            list = list.Where(p => p.TipoPropiedadId == filter.TipoPropiedadId.Value).ToList();
        }

        if (filter.PrecioMinimo.HasValue)
        {
            list = list.Where(p => p.Precio >= filter.PrecioMinimo.Value).ToList();
        }

        if (filter.PrecioMaximo.HasValue)
        {
            list = list.Where(p => p.Precio <= filter.PrecioMaximo.Value).ToList();
        }

        if (filter.CantidadHabitaciones.HasValue)
        {
            list = list.Where(p => p.CantidadHabitaciones == filter.CantidadHabitaciones.Value).ToList();
        }

        if (filter.CantidadBanos.HasValue)
        {
            list = list.Where(p => p.CantidadBanos == filter.CantidadBanos.Value).ToList();
        }

        return list;
    }
    public async Task<int> CountByStatus(string status)
    {
        // Asumiendo que tienes acceso a tu repositorio de propiedades
        // Esto es un ejemplo genérico, adáptalo a tu repositorio:
        var propiedades = await _propiedadRepository.GetAllAsync(); 
        return propiedades.Count(p => p.TipoVenta.Nombre == status); 
        // O si tienes el estado directamente en la entidad:
        // return propiedades.Count(p => p.Estado == status);
    }
}