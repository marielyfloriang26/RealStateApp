using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Contadores
        var data = new
        {
            PropiedadesDisponibles = await _context.Propiedades.CountAsync(p => p.Estado == "Disponible"),
            PropiedadesVendidas = await _context.Propiedades.CountAsync(p => p.Estado == "Vendida"),
            AgentesActivos = await _context.Users.CountAsync(u => u.TipoUsuario == "Agente" && u.EsActivo),
            AgentesInactivos = await _context.Users.CountAsync(u => u.TipoUsuario == "Agente" && !u.EsActivo),
            ClientesActivos = await _context.Users.CountAsync(u => u.TipoUsuario == "Cliente" && u.EsActivo),
            ClientesInactivos = await _context.Users.CountAsync(u => u.TipoUsuario == "Cliente" && !u.EsActivo),
            DesarrolladoresActivos = await _context.Users.CountAsync(u => u.TipoUsuario == "Desarrollador" && u.EsActivo),
            DesarrolladoresInactivos = await _context.Users.CountAsync(u => u.TipoUsuario == "Desarrollador" && !u.EsActivo)
        };

        ViewBag.DashboardData = data;
        return View();
    }
}