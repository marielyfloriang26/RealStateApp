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
    private readonly IPropiedadService _propiedadService; // Asegúrate de tener este servicio

    public AdminController(UserManager<Usuario> userManager, IPropiedadService propertyService)
    {
        _userManager = userManager;
        _propiedadService = propertyService;
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
            PropiedadesDisponibles = await _propiedadService.CountByStatus("Disponible"),
            PropiedadesVendidas = await _propiedadService.CountByStatus("Vendida")
        };

        return View(viewModel);
    }

    public async Task<IActionResult> ListarAgentes()
    {
        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        var model = new List<AgenteViewModel>();

        foreach (var user in agentes)
        {
            model.Add(new AgenteViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                EsActivo = user.EsActivo,
                CantidadPropiedades = await _propiedadService.CountByAgenteId(user.Id)
            });
        }
        return View(model);
    }

    // 2. Activar/Inactivar
    [HttpPost] // ESTO ES OBLIGATORIO
    public async Task<IActionResult> CambiarEstadoAgente(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        // Cambiamos el estado
        user.EsActivo = !user.EsActivo;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = $"El agente fue {(user.EsActivo ? "activado" : "inactivado")} correctamente.";
        return RedirectToAction("ListarAgentes");
    }

    // 3. Eliminar (GET para vista de confirmación)
    [HttpGet]
    public async Task<IActionResult> EliminarAgente(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) 
        {
            TempData["Error"] = "El agente seleccionado no existe.";
            return RedirectToAction("ListarAgentes");
        }
        return View(user); // Renderiza la vista que acabamos de crear
    }

    // 4. Eliminar (POST para ejecutar)
    [HttpPost] // DEBE tener este atributo
    public async Task<IActionResult> ConfirmarEliminarAgente(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        await _propiedadService.DeleteAllByAgenteId(id);
        await _userManager.DeleteAsync(user);

        TempData["Success"] = "El agente fue eliminado correctamente.";
        return RedirectToAction("ListarAgentes");
    }
}