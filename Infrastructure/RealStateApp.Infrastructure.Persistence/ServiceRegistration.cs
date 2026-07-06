using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Infrastructure.Persistence.Contexts;
using RealStateApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RealStateApp.Infrastructure.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                m => m.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        #region Repositories
        services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
        services.AddTransient<ITipoPropiedadRepository, TipoPropiedadRepository>();
        services.AddTransient<ITipoVentaRepository, TipoVentaRepository>();
        services.AddTransient<IMejoraRepository, MejoraRepository>();
        services.AddTransient<IPropiedadRepository, PropiedadRepository>();
        services.AddTransient<IPropiedadMejoraRepository, PropiedadMejoraRepository>();
        services.AddTransient<IImagenPropiedadRepository, ImagenPropiedadRepository>();
        services.AddTransient<IPropiedadFavoritaRepository, PropiedadFavoritaRepository>();
        services.AddTransient<IOfertaRepository, OfertaRepository>();
        services.AddTransient<IMensajeRepository, MensajeRepository>();
        #endregion
    }
}
