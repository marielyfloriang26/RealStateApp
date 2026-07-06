using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class MejoraRepository : RepositoryAsync<Mejora>, IMejoraRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MejoraRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
