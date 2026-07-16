using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Agente;
using RealStateApp.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApp.Controllers;


[Authorize(Roles = "Agente")] // Protege el controlador para que solo entren Agentes //AGENTE, TEMPORAL

public class AgenteController : Controller
{
    private readonly IAgentePropiedadService _agentePropiedadService;
    private readonly IPropiedadService _propiedadService;
    private readonly UserManager<Usuario> _userManager;

    public AgenteController(IAgentePropiedadService agentePropiedadService, IPropiedadService propiedadService, UserManager<Usuario> userManager)
    {
        _agentePropiedadService = agentePropiedadService;
        _propiedadService = propiedadService;
        _userManager = userManager;
    }

    private int GetAgenteId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    public async Task<IActionResult> Index()
    {
        var agentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(agentIdClaim))
        {
            return RedirectToAction("Login", "Home");
        }

        int agentId = int.Parse(agentIdClaim);
        
        var propiedades = await _propiedadService.GetPropertiesByAgentIdAsync(agentId);

        return View(propiedades);
    }

    // --- Detalle Propiedad ---
    public async Task<IActionResult> PropiedadDetalle(int id)
    {
        var agenteId = GetAgenteId();
        var vm = await _agentePropiedadService.GetPropiedadDetalleAsync(id, agenteId);
        if (vm == null) return RedirectToAction("Index", "Home");
        return View(vm);
    }

    // --- Conversaciones ---
    public async Task<IActionResult> ClientesConversacion(int propiedadId)
    {
        var agenteId = GetAgenteId();
        var clientes = await _agentePropiedadService.GetClientesConversacionAsync(propiedadId, agenteId);
        ViewBag.PropiedadId = propiedadId;
        return View(clientes);
    }

    public async Task<IActionResult> Conversacion(int propiedadId, int clienteId)
    {
        var agenteId = GetAgenteId();
        var vm = await _agentePropiedadService.GetConversacionCompletaAsync(propiedadId, clienteId, agenteId);
        if (vm == null) return RedirectToAction("ClientesConversacion", new { propiedadId });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EnviarMensaje(AgentConversacionViewModel model)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.NuevoMensaje))
        {
            ModelState.AddModelError("NuevoMensaje", "Debe escribir un mensaje antes de enviarlo.");
            var agenteId = GetAgenteId();
            var vm = await _agentePropiedadService.GetConversacionCompletaAsync(model.PropiedadId, model.ClienteId, agenteId);
            if(vm != null) 
            {
                vm.NuevoMensaje = model.NuevoMensaje;
                return View("Conversacion", vm);
            }
            return RedirectToAction("Index", "Home");
        }

        var success = await _agentePropiedadService.EnviarMensajeAsync(model.PropiedadId, model.ClienteId, GetAgenteId(), model.NuevoMensaje);
        return RedirectToAction("Conversacion", new { propiedadId = model.PropiedadId, clienteId = model.ClienteId });
    }

    // --- Ofertas ---
    public async Task<IActionResult> ClientesOfertas(int propiedadId)
    {
        var agenteId = GetAgenteId();
        var clientes = await _agentePropiedadService.GetClientesConOfertasAsync(propiedadId, agenteId);
        ViewBag.PropiedadId = propiedadId;
        return View(clientes);
    }

    public async Task<IActionResult> OfertasPorCliente(int propiedadId, int clienteId)
    {
        var agenteId = GetAgenteId();
        var vm = await _agentePropiedadService.GetOfertasPorClienteAsync(propiedadId, clienteId, agenteId);
        if (vm == null) return RedirectToAction("ClientesOfertas", new { propiedadId });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> ResponderOferta(int ofertaId, int propiedadId, int clienteId, string respuesta)
    {
        var agenteId = GetAgenteId();
        var success = await _agentePropiedadService.ResponderOfertaAsync(ofertaId, agenteId, respuesta);
        if (success)
        {
            if (respuesta == "Aceptada")
                TempData["SuccessMessage"] = "La oferta fue aceptada correctamente y la propiedad fue marcada como vendida.";
            else if (respuesta == "Rechazada")
                TempData["SuccessMessage"] = "La oferta fue rechazada correctamente.";
        }
        else
        {
            var propiedadDetalle = await _agentePropiedadService.GetPropiedadDetalleAsync(propiedadId, agenteId);
            if (propiedadDetalle?.Estado == "Vendida")
                TempData["ErrorMessage"] = "No se puede aceptar una oferta para una propiedad que ya fue vendida.";
            else
                TempData["ErrorMessage"] = "Esta oferta ya fue respondida.";
        }

        return RedirectToAction("OfertasPorCliente", new { propiedadId, clienteId });
    }
}
