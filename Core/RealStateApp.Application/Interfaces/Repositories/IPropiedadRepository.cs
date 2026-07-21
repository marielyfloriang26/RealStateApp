using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Interfaces.Repositories;

public interface IPropiedadRepository : IRepositoryAsync<Propiedad>
{
    Task<IReadOnlyList<Propiedad>> GetAllWithIncludeAsync();
    Task<Propiedad?> GetByIdWithIncludeAsync(int id);
    Task DeleteAllRelatedToAgente(int agenteId);
}
