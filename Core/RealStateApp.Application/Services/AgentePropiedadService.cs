using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Agente;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Services;

public class AgentePropiedadService : IAgentePropiedadService
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IMensajeRepository _mensajeRepository;
    private readonly IOfertaRepository _ofertaRepository;
    private readonly IRepositoryAsync<Usuario> _usuarioRepository;

    public AgentePropiedadService(
        IPropiedadRepository propiedadRepository,
        IMensajeRepository mensajeRepository,
        IOfertaRepository ofertaRepository,
        IRepositoryAsync<Usuario> usuarioRepository)
    {
        _propiedadRepository = propiedadRepository;
        _mensajeRepository = mensajeRepository;
        _ofertaRepository = ofertaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AgentPropiedadDetalleViewModel?> GetPropiedadDetalleAsync(int propiedadId, int agenteId)
    {
        var propiedades = await _propiedadRepository.GetAllWithIncludeAsync(new List<string> { "TipoPropiedad", "TipoVenta", "Imagenes", "PropiedadMejoras.Mejora" });
        var propiedad = propiedades.FirstOrDefault(p => p.Id == propiedadId && p.AgenteId == agenteId);

        if (propiedad == null) return null;

        return new AgentPropiedadDetalleViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            TipoPropiedad = propiedad.TipoPropiedad?.Nombre ?? "",
            TipoVenta = propiedad.TipoVenta?.Nombre ?? "",
            Precio = propiedad.Precio,
            CantidadHabitaciones = propiedad.CantidadHabitaciones,
            CantidadBanos = propiedad.CantidadBanos,
            TamanoMetros = propiedad.TamanoMetros,
            Descripcion = propiedad.Descripcion,
            Estado = propiedad.Estado,
            Imagenes = propiedad.Imagenes?.Select(i => i.ImagenUrl).ToList() ?? new List<string>(),
            Mejoras = propiedad.PropiedadMejoras?.Select(pm => pm.Mejora?.Nombre ?? "").ToList() ?? new List<string>()
        };
    }

    public async Task<List<AgentClienteConversacionViewModel>> GetClientesConversacionAsync(int propiedadId, int agenteId)
    {
        var mensajes = await _mensajeRepository.GetAllWithIncludeAsync(new List<string> { "Cliente" });
        var mensajesPropiedad = mensajes.Where(m => m.PropiedadId == propiedadId && m.AgenteId == agenteId).ToList();

        var clientes = mensajesPropiedad
            .GroupBy(m => m.ClienteId)
            .Select(g => new AgentClienteConversacionViewModel
            {
                ClienteId = g.Key,
                NombreCliente = g.First().Cliente!.Nombre + " " + g.First().Cliente!.Apellido,
                UltimoMensaje = g.OrderByDescending(m => m.FechaEnvio).First().Contenido,
                FechaUltimoMensaje = g.OrderByDescending(m => m.FechaEnvio).First().FechaEnvio
            })
            .ToList();

        return clientes;
    }

    public async Task<AgentConversacionViewModel?> GetConversacionCompletaAsync(int propiedadId, int clienteId, int agenteId)
    {
        var propiedad = await _propiedadRepository.GetByIdAsync(propiedadId);
        if (propiedad == null || propiedad.AgenteId != agenteId) return null;

        var cliente = await _usuarioRepository.GetByIdAsync(clienteId);
        if (cliente == null) return null;

        var todosMensajes = await _mensajeRepository.GetAllWithIncludeAsync(new List<string> { "Cliente", "Agente" });
        var mensajesFiltrados = todosMensajes
            .Where(m => m.PropiedadId == propiedadId && m.ClienteId == clienteId && m.AgenteId == agenteId)
            .OrderBy(m => m.FechaEnvio)
            .Select(m => new AgentMensajeViewModel
            {
                Remitente = m.Remitente == "Agente" ? (m.Agente?.Nombre ?? "Agente") : (m.Cliente?.Nombre ?? "Cliente"),
                Contenido = m.Contenido,
                FechaEnvio = m.FechaEnvio
            })
            .ToList();

        return new AgentConversacionViewModel
        {
            PropiedadId = propiedadId,
            ClienteId = clienteId,
            NombreCliente = cliente.Nombre + " " + cliente.Apellido,
            Mensajes = mensajesFiltrados,
            NuevoMensaje = ""
        };
    }

    public async Task<bool> EnviarMensajeAsync(int propiedadId, int clienteId, int agenteId, string contenido)
    {
        var propiedad = await _propiedadRepository.GetByIdAsync(propiedadId);
        if (propiedad == null || propiedad.AgenteId != agenteId) return false;

        var mensaje = new Mensaje
        {
            PropiedadId = propiedadId,
            ClienteId = clienteId,
            AgenteId = agenteId,
            Remitente = "Agente",
            Contenido = contenido,
            FechaEnvio = DateTime.UtcNow
        };

        await _mensajeRepository.AddAsync(mensaje);
        return true;
    }

    public async Task<List<AgentClienteOfertasViewModel>> GetClientesConOfertasAsync(int propiedadId, int agenteId)
    {
        var propiedad = await _propiedadRepository.GetByIdAsync(propiedadId);
        if (propiedad == null || propiedad.AgenteId != agenteId) return new List<AgentClienteOfertasViewModel>();

        var ofertas = await _ofertaRepository.GetAllWithIncludeAsync(new List<string> { "Cliente" });
        var ofertasPropiedad = ofertas.Where(o => o.PropiedadId == propiedadId).ToList();

        var resultado = ofertasPropiedad
            .GroupBy(o => o.ClienteId)
            .Select(g => new AgentClienteOfertasViewModel
            {
                ClienteId = g.Key,
                NombreCliente = g.First().Cliente!.Nombre + " " + g.First().Cliente!.Apellido,
                CantidadOfertas = g.Count(),
                UltimaOfertaMonto = g.OrderByDescending(o => o.FechaOferta).First().Monto,
                EstadoUltimaOferta = g.OrderByDescending(o => o.FechaOferta).First().Estado
            })
            .ToList();

        return resultado;
    }

    public async Task<AgentOfertasPorClienteViewModel?> GetOfertasPorClienteAsync(int propiedadId, int clienteId, int agenteId)
    {
        var propiedad = await _propiedadRepository.GetByIdAsync(propiedadId);
        if (propiedad == null || propiedad.AgenteId != agenteId) return null;

        var cliente = await _usuarioRepository.GetByIdAsync(clienteId);
        if (cliente == null) return null;

        var ofertas = await _ofertaRepository.GetAllWithIncludeAsync(new List<string>());
        var ofertasFiltradas = ofertas
            .Where(o => o.PropiedadId == propiedadId && o.ClienteId == clienteId)
            .OrderByDescending(o => o.FechaOferta)
            .Select(o => new AgentOfertaDetalleViewModel
            {
                OfertaId = o.Id,
                FechaOferta = o.FechaOferta,
                MontoOfertado = o.Monto,
                Estado = o.Estado
            })
            .ToList();

        return new AgentOfertasPorClienteViewModel
        {
            PropiedadId = propiedadId,
            ClienteId = clienteId,
            NombreCliente = cliente.Nombre + " " + cliente.Apellido,
            Ofertas = ofertasFiltradas
        };
    }

    public async Task<bool> ResponderOfertaAsync(int ofertaId, int agenteId, string nuevaRespuesta)
    {
        var ofertas = await _ofertaRepository.GetAllWithIncludeAsync(new List<string> { "Propiedad" });
        var oferta = ofertas.FirstOrDefault(o => o.Id == ofertaId);

        if (oferta == null || oferta.Propiedad?.AgenteId != agenteId) return false;
        
        // Reglas de negocio
        if (oferta.Propiedad.Estado == "Vendida") return false;
        if (oferta.Estado != "Pendiente") return false;

        if (nuevaRespuesta == "Rechazada")
        {
            oferta.Estado = "Rechazada";
            await _ofertaRepository.UpdateAsync(oferta);
            return true;
        }

        if (nuevaRespuesta == "Aceptada")
        {
            oferta.Estado = "Aceptada";
            await _ofertaRepository.UpdateAsync(oferta);

            // Rechazar las demás ofertas pendientes de esa propiedad
            var otrasOfertas = ofertas.Where(o => o.PropiedadId == oferta.PropiedadId && o.Id != ofertaId && o.Estado == "Pendiente");
            foreach (var o in otrasOfertas)
            {
                o.Estado = "Rechazada";
                await _ofertaRepository.UpdateAsync(o);
            }

            // Cambiar estado propiedad a Vendida
            oferta.Propiedad.Estado = "Vendida";
            await _propiedadRepository.UpdateAsync(oferta.Propiedad);

            return true;
        }

        return false;
    }
}
