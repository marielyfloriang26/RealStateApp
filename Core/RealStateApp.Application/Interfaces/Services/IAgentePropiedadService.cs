using System.Collections.Generic;
using System.Threading.Tasks;
using RealStateApp.Application.ViewModels.Agente;

namespace RealStateApp.Application.Interfaces.Services;

public interface IAgentePropiedadService
{
    Task<AgentPropiedadDetalleViewModel?> GetPropiedadDetalleAsync(int propiedadId, int agenteId);
    Task<List<AgentClienteConversacionViewModel>> GetClientesConversacionAsync(int propiedadId, int agenteId);
    Task<AgentConversacionViewModel?> GetConversacionCompletaAsync(int propiedadId, int clienteId, int agenteId);
    Task<bool> EnviarMensajeAsync(int propiedadId, int clienteId, int agenteId, string contenido);
    
    Task<List<AgentClienteOfertasViewModel>> GetClientesConOfertasAsync(int propiedadId, int agenteId);
    Task<AgentOfertasPorClienteViewModel?> GetOfertasPorClienteAsync(int propiedadId, int clienteId, int agenteId);
    Task<bool> ResponderOfertaAsync(int ofertaId, int agenteId, string nuevaRespuesta); // nuevaRespuesta: "Aceptada" o "Rechazada"
}
