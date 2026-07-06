using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class PropiedadMejoraRepository : RepositoryAsync<PropiedadMejora>, IPropiedadMejoraRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PropiedadMejoraRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
