using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IOfertaService
{
    Task<List<OfertaViewModel>> GetOffersByClientAndPropertyAsync(int clientId, int propiedadId);
    Task<bool> HasPendingOfferAsync(int clientId, int propiedadId);
    Task<bool> HasAcceptedOfferAsync(int propiedadId);
    Task MakeOfferAsync(int clientId, int propiedadId, decimal amount);
}