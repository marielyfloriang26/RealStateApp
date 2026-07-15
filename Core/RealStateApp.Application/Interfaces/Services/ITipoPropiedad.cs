using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface ITipoPropiedadService
{
    Task<List<TipoPropiedadViewModel>> GetAllAsync();
}