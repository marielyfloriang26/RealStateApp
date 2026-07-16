using System.Collections.Generic;
using System.Threading.Tasks;
using RealStateApp.Application.ViewModels.TipoPropiedades;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Interfaces.Services;

public interface ITipoPropiedadService : IGenericService<SaveTipoPropiedadViewModel, TipoPropiedadViewModel, TipoPropiedad>
{
    Task<List<TipoPropiedadViewModel>> GetAllAsync();
}
