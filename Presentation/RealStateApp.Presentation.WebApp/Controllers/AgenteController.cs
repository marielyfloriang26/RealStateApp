using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Cliente")] // Protege el controlador para que solo entren Agentes
public class AgenteController : Controller
{
    private readonly IPropiedadService _propiedadService;
    private readonly UserManager<Usuario> _userManager;

    public AgenteController(IPropiedadService propiedadService, UserManager<Usuario> userManager)
    {
        _propiedadService = propiedadService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Obtiene el ID del Agente que tiene la sesion iniciada
        var agentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(agentIdClaim))
        {
            return RedirectToAction("Login", "Home");
        }

        int agentId = int.Parse(agentIdClaim);
        
        // Buscamos sus propiedades
        var propiedades = await _propiedadService.GetPropertiesByAgentIdAsync(agentId);

        return View(propiedades);
    }
}