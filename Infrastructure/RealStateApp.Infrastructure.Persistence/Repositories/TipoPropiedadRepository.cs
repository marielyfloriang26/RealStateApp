using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class TipoPropiedadRepository : RepositoryAsync<TipoPropiedad>, ITipoPropiedadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TipoPropiedadRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
