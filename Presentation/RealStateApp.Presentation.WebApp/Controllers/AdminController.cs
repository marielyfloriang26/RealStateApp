using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Admin;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IPropiedadService _propertyService; // Asegúrate de tener este servicio

    public AdminController(UserManager<Usuario> userManager, IPropiedadService propertyService)
    {
        _userManager = userManager;
        _propertyService = propertyService;
    }

    public async Task<IActionResult> Index()
    {
        // Obtener usuarios
        var users = _userManager.Users.ToList();
        
        // Indicadores de Usuarios
        var viewModel = new AdminDashboardViewModel
        {
            AgentesActivos = users.Count(u => u.TipoUsuario == "Agente" && u.EsActivo),
            AgentesInactivos = users.Count(u => u.TipoUsuario == "Agente" && !u.EsActivo),
            ClientesActivos = users.Count(u => u.TipoUsuario == "Cliente" && u.EsActivo),
            ClientesInactivos = users.Count(u => u.TipoUsuario == "Cliente" && !u.EsActivo),
            DesarrolladoresActivos = users.Count(u => u.TipoUsuario == "Desarrollador" && u.EsActivo),
            DesarrolladoresInactivos = users.Count(u => u.TipoUsuario == "Desarrollador" && !u.EsActivo),
            
            // Indicadores de Propiedades (Asumiendo que tienes un servicio de propiedades)
            PropiedadesDisponibles = await _propertyService.CountByStatus("Disponible"),
            PropiedadesVendidas = await _propertyService.CountByStatus("Vendida")
        };

        return View(viewModel);
    }
}