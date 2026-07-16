using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Application.Services;

public class MensajeService : IMensajeService
{
    private readonly IMensajeRepository _mensajeRepository;
    private readonly IMapper _mapper;

    public MensajeService(IMensajeRepository mensajeRepository, IMapper mapper)
    {
        _mensajeRepository = mensajeRepository;
        _mapper = mapper;
    }

    public async Task<List<MensajeViewModel>> GetChatHistoryAsync(int clientId, int propiedadId)
    {
        var mensajes = await _mensajeRepository.GetAllAsync();
        // El cliente solo ve su conv sobre esta propiedad
        var chat = mensajes.Where(m => m.ClienteId == clientId && m.PropiedadId == propiedadId).OrderBy(m => m.FechaEnvio).ToList();
        return _mapper.Map<List<MensajeViewModel>>(chat);
    }

    public async Task SendMessageAsync(int clientId, int agenteId, int propiedadId, string content, string senderRole)
    {
        var mensaje = new Mensaje
        {
            ClienteId = clientId,
            AgenteId = agenteId,
            PropiedadId = propiedadId,
            Contenido = content,
            Remitente = senderRole, // "Cliente" o "Agente"
            FechaEnvio = DateTime.UtcNow
        };
        await _mensajeRepository.AddAsync(mensaje);
    }
}