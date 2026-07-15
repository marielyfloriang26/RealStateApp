using Microsoft.EntityFrameworkCore;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class PropiedadRepository : RepositoryAsync<Propiedad>, IPropiedadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PropiedadRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IReadOnlyList<Propiedad>> GetAllWithIncludeAsync()
    {
        return await _dbContext.Propiedades
            .Include(p => p.TipoPropiedad)
            .Include(p => p.TipoVenta)
            .Include(p => p.Agente)
            .Include(p => p.Imagenes)
            .Include(p => p.PropiedadMejoras!)
            .ThenInclude(pm => pm.Mejora)
            .ToListAsync();
    }
    public async Task<Propiedad?> GetByIdWithIncludeAsync(int id)
    {
        return await _dbContext.Propiedades
            .Include(p => p.TipoPropiedad)
            .Include(p => p.TipoVenta)
            .Include(p => p.Agente)
            .Include(p => p.Imagenes)
            .Include(p => p.PropiedadMejoras!)
            .ThenInclude(pm => pm.Mejora)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
