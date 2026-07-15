using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.Services;
using System.Reflection;

namespace RealStateApp.Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        #region Services
        services.AddTransient<IPropiedadService, PropiedadService>();
        services.AddTransient<ITipoPropiedadService, TipoPropiedadService>();
        #endregion
    }
}