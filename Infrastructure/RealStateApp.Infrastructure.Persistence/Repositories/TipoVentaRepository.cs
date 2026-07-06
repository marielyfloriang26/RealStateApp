using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class TipoVentaRepository : RepositoryAsync<TipoVenta>, ITipoVentaRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TipoVentaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
