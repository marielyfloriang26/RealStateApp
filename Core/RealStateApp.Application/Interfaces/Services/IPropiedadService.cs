using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IPropiedadService
{
    Task<List<PropiedadViewModel>> GetAllWithIncludeAsync();
    Task<PropiedadViewModel?> GetByIdWithIncludeAsync(int id);
    Task<PropiedadViewModel?> GetByCodeWithIncludeAsync(string code);
    Task<List<PropiedadViewModel>> GetAllFilteredAsync(FiltroPropiedadViewModel filter);
    Task<List<PropiedadViewModel>> GetPropertiesByAgentIdAsync(int agentId);
}