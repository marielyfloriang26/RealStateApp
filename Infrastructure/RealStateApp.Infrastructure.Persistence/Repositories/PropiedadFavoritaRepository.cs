using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class PropiedadFavoritaRepository : RepositoryAsync<PropiedadFavorita>, IPropiedadFavoritaRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PropiedadFavoritaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
