using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class ImagenPropiedadRepository : RepositoryAsync<ImagenPropiedad>, IImagenPropiedadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ImagenPropiedadRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
