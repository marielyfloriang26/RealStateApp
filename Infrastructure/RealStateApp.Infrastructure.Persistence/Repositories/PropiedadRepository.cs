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
    public async Task DeleteAllRelatedToAgente(int agenteId)
    {
        // Buscamos todas las propiedades del agente
        var propiedades = _dbContext.Propiedades.Where(p => p.AgenteId == agenteId).ToList();

        foreach (var propiedad in propiedades)
        {
            // Eliminamos hijos (cumpliendo la integridad de la base de datos)
            _dbContext.Mensajes.RemoveRange(_dbContext.Mensajes.Where(m => m.PropiedadId == propiedad.Id));
            _dbContext.Ofertas.RemoveRange(_dbContext.Ofertas.Where(o => o.PropiedadId == propiedad.Id));
            _dbContext.PropiedadesFavoritas.RemoveRange(_dbContext.PropiedadesFavoritas.Where(pf => pf.PropiedadId == propiedad.Id));
            _dbContext.ImagenesPropiedad.RemoveRange(_dbContext.ImagenesPropiedad.Where(i => i.PropiedadId == propiedad.Id));
            _dbContext.PropiedadesMejoras.RemoveRange(_dbContext.PropiedadesMejoras.Where(pm => pm.PropiedadId == propiedad.Id));
            
            // Eliminamos la propiedad
            _dbContext.Propiedades.Remove(propiedad);
        }

        // Persistimos cambios
        await _dbContext.SaveChangesAsync();
    }
}
