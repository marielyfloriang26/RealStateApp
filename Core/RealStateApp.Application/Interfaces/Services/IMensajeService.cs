using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IMensajeService
{
    Task<List<MensajeViewModel>> GetChatHistoryAsync(int clientId, int propiedadId);
    Task SendMessageAsync(int clientId, int agenteId, int propiedadId, string content, string senderRole);
}