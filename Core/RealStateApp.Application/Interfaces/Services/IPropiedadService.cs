using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Domain.Entities;
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


    Task<List<PropiedadViewModel>> GetAllApiAsync();
    Task<PropiedadViewModel?> GetByIdApiAsync(int id);
    Task<PropiedadViewModel?> GetByCodeApiAsync(string code);

    Task<int> CountByStatus(string status);
    Task<int> CountByAgenteId(int agenteId);
    Task DeleteAllByAgenteId(int agenteId);

}