using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IAgenteService
{
    Task<List<AgenteViewModel>> GetAllActiveAsync();
    Task<List<AgenteViewModel>> SearchByNameAsync(string name);
    Task<AgenteViewModel?> GetByIdAsync(int id);
}