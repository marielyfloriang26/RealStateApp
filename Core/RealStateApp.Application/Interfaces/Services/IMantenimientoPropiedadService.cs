using System.Collections.Generic;
using System.Threading.Tasks;
using RealStateApp.Application.ViewModels.Propiedades;

namespace RealStateApp.Application.Interfaces.Services;

public interface IMantenimientoPropiedadService
{
    Task<List<PropiedadViewModel>> GetAllViewModelWithFilters(int agenteId);
    Task<SavePropiedadViewModel> Add(SavePropiedadViewModel vm, int agenteId);
    Task Update(SavePropiedadViewModel vm, int agenteId);
    Task Delete(int id, int agenteId);
    Task<SavePropiedadViewModel?> GetByIdSaveViewModel(int id, int agenteId);
    
    Task<List<TipoPropiedadViewModel>> GetTiposPropiedad();
    Task<List<TipoVentaViewModel>> GetTiposVenta();
    Task<List<MejoraViewModel>> GetMejoras();
}
