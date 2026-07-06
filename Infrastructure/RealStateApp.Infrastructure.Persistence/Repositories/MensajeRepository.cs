using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class MensajeRepository : RepositoryAsync<Mensaje>, IMensajeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MensajeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
