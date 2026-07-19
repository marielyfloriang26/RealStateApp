using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Agente;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApp.Controllers;


[Authorize(Roles = "Agente")] // solo entren Agentes 

public class AgenteController : Controller
{
    private readonly IAgentePropiedadService _agentePropiedadService;
    private readonly IPropiedadService _propiedadService;
    private readonly UserManager<Usuario> _userManager;
    private readonly IAgenteService _agenteService;
    private readonly IUploadService _uploadService;

    public AgenteController(IAgentePropiedadService agentePropiedadService, IPropiedadService propiedadService, UserManager<Usuario> userManager, IAgenteService agenteService, IUploadService uploadService)
    {
        _agentePropiedadService = agentePropiedadService;
        _propiedadService = propiedadService;
        _userManager = userManager;
        _agenteService = agenteService; 
        _uploadService = uploadService;
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
        if (vm == null) return RedirectToAction("Index");
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
                vm.NuevoMensaje = model.NuevoMensaje ?? "";
                return View("Conversacion", vm);
            }
            return RedirectToAction("Index");
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
        var (success, errorMessage) = await _agentePropiedadService.ResponderOfertaAsync(ofertaId, agenteId, respuesta);
        if (success)
        {
            if (respuesta == "Aceptada")
                TempData["SuccessMessage"] = "La oferta fue aceptada correctamente y la propiedad fue marcada como vendida.";
            else if (respuesta == "Rechazada")
                TempData["SuccessMessage"] = "La oferta fue rechazada correctamente.";
        }
        else
        {
            TempData["ErrorMessage"] = errorMessage;
        }

        return RedirectToAction("OfertasPorCliente", new { propiedadId, clienteId });
    }
    [HttpGet]
public async Task<IActionResult> Perfil()
{
    var agentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var agente = await _agenteService.GetByIdAsync(int.Parse(agentIdClaim!));

    if (agente == null) return RedirectToAction("Index");

    var vm = new MiPerfilViewModel
    {
        Id = agente.Id,
        Nombre = agente.Nombre,
        Apellido = agente.Apellido,
        Teléfono = agente.Telefono ?? "",
        FotoUrl = agente.FotoUrl
    };

    return View(vm);
}

[HttpPost]
public async Task<IActionResult> Perfil(MiPerfilViewModel vm)
{
    if (!ModelState.IsValid) return View(vm);

    var agentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    int agentId = int.Parse(agentIdClaim!);

    // Procesa subida de foto si se cargo una nueva
    if (vm.FotoFile != null)
    {
        var ext = System.IO.Path.GetExtension(vm.FotoFile.FileName).ToLower();
        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
        {
            ModelState.AddModelError("FotoFile", "El archivo seleccionado no tiene un formato de imagen válido (Solo .jpg, .jpeg, .png).");
            return View(vm);
        }

        // Sube la imagen y guarda la nueva URL
        vm.FotoUrl = _uploadService.UploadFile(vm.FotoFile, agentId);
    }

    await _agenteService.UpdateProfileAsync(agentId, vm);
    TempData["SuccessMessage"] = "Su perfil fue actualizado correctamente.";
    
    return RedirectToAction("Perfil");
}
}
