using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IPropiedadFavoritaService
{
    Task AddFavoriteAsync(int clientId, int propertyId);
    Task RemoveFavoriteAsync(int clientId, int propertyId);
    Task<List<PropiedadViewModel>> GetFavoritesByClientIdAsync(int clientId);
    Task<List<int>> GetFavoritePropertyIdsByClientIdAsync(int clientId);
}