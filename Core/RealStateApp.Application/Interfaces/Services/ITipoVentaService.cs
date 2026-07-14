using RealStateApp.Application.ViewModels.TipoVentas;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Interfaces.Services;

public interface ITipoVentaService : IGenericService<SaveTipoVentaViewModel, TipoVentaViewModel, TipoVenta>
{
    
}
