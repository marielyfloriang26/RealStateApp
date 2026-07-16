using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Application.Services;

public class PropiedadFavoritaService : IPropiedadFavoritaService
{
    private readonly IPropiedadFavoritaRepository _favoritoRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IMapper _mapper;

    public PropiedadFavoritaService(
        IPropiedadFavoritaRepository favoritoRepository, 
        IPropiedadRepository propiedadRepository, 
        IMapper mapper)
    {
        _favoritoRepository = favoritoRepository;
        _propiedadRepository = propiedadRepository;
        _mapper = mapper;
    }

    public async Task AddFavoriteAsync(int clientId, int propertyId)
    {
        var favoritos = await _favoritoRepository.GetAllAsync();
        // Evita duplicados
        if (favoritos.Any(f => f.ClienteId == clientId && f.PropiedadId == propertyId))
        {
            return;
        }

        var favorito = new PropiedadFavorita
        {
            ClienteId = clientId,
            PropiedadId = propertyId
        };
        await _favoritoRepository.AddAsync(favorito);
    }

    public async Task RemoveFavoriteAsync(int clientId, int propertyId)
    {
        var favoritos = await _favoritoRepository.GetAllAsync();
        var favorito = favoritos.FirstOrDefault(f => f.ClienteId == clientId && f.PropiedadId == propertyId);
        if (favorito != null)
        {
            await _favoritoRepository.DeleteAsync(favorito);
        }
    }

    public async Task<List<PropiedadViewModel>> GetFavoritesByClientIdAsync(int clientId)
    {
        var favoritos = await _favoritoRepository.GetAllAsync();
        var propiedadIds = favoritos.Where(f => f.ClienteId == clientId).Select(f => f.PropiedadId).ToList();

        var propiedades = await _propiedadRepository.GetAllWithIncludeAsync();
        // Solo trae las favoritas que pertenezcan al cliente y esten disponibles
        var favoritasDisponibles = propiedades.Where(p => propiedadIds.Contains(p.Id) && p.Estado == "Disponible").OrderByDescending(p => p.FechaCreacion).ToList();

        return _mapper.Map<List<PropiedadViewModel>>(favoritasDisponibles);
    }

    public async Task<List<int>> GetFavoritePropertyIdsByClientIdAsync(int clientId)
    {
        var favoritos = await _favoritoRepository.GetAllAsync();
        return favoritos.Where(f => f.ClienteId == clientId).Select(f => f.PropiedadId).ToList();
    }
}