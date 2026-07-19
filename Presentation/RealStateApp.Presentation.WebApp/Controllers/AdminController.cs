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



    // =================================
    // MANTENIMIENTO DE ADMINISTRADORES
    // =================================

    public IActionResult CrearAdministrador()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAdministrador(SaveAdministradorViewModel model)
    {
        if (string.IsNullOrEmpty(model.Password)) 
            ModelState.AddModelError("Password", "La contraseña es obligatoria.");

        if (!ModelState.IsValid) return View(model);

        // Crear el objeto usuario de Identity
        var user = new Usuario 
        { 
            UserName = model.UserName, 
            Email = model.Email, 
            Nombre = model.Nombre, 
            Apellido = model.Apellido, 
            Cedula = model.Cedula,
            EsActivo = true, // Por defecto lo activamos al crearlo
            TipoUsuario = "Administrador"
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Administrador");
            TempData["Success"] = "Administrador creado exitosamente.";
            return RedirectToAction("ListarAdministradores");
        }

        // Si hubo errores (ej: usuario duplicado, contraseña débil)
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> CambiarEstadoAdmin(string id)
    {
        var adminLogueadoId = _userManager.GetUserId(User);
        if (id == adminLogueadoId)
        {
            TempData["Error"] = "No puede inactivar a su propio usuario.";
            return RedirectToAction("ListarAdministradores");
        }

        var user = await _userManager.FindByIdAsync(id);
        
        // Validación: No dejar el sistema sin administradores activos
        var administradoresActivos = (await _userManager.GetUsersInRoleAsync("Administrador"))
                                      .Count(a => a.EsActivo);

        if (user.EsActivo && administradoresActivos <= 1)
        {
            TempData["Error"] = "Debe existir al menos un administrador activo en el sistema.";
            return RedirectToAction("ListarAdministradores");
        }

        user.EsActivo = !user.EsActivo;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = $"El administrador fue {(user.EsActivo ? "activado" : "inactivado")} correctamente.";
        
        return RedirectToAction("ListarAdministradores");
    }

    [HttpGet]
    public async Task<IActionResult> EditarAdministrador(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var model = new EditarAdministradorViewModel
        {
            Id = user.Id.ToString(),
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Cedula = user.Cedula,
            Email = user.Email,
            UserName = user.UserName
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditarAdministrador(EditarAdministradorViewModel model)
    {
        // 1. Validar que no edite su propio usuario
        var adminLogueadoId = _userManager.GetUserId(User);
        if (model.Id == adminLogueadoId)
        {
            TempData["Error"] = "No puede editar su propio usuario desde este mantenimiento.";
            return RedirectToAction("ListarAdministradores");
        }

        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        // 2. Validaciones de datos únicos (cédula, correo, username)
        // Excluyendo al usuario actual del chequeo para no dar error de duplicado consigo mismo
        if (_userManager.Users.Any(u => u.Cedula == model.Cedula && u.Id.ToString() != model.Id.ToString()))
            ModelState.AddModelError("Cedula", "Ya existe un usuario registrado con esta cédula.");
        
        if (_userManager.Users.Any(u => u.Email == model.Email && u.Id.ToString() != model.Id.ToString()))
            ModelState.AddModelError("Email", "Ya existe un usuario registrado con este correo.");

        if (!ModelState.IsValid) return View(model);

        // 3. Actualizar datos básicos
        user.Nombre = model.Nombre;
        user.Apellido = model.Apellido;
        user.Cedula = model.Cedula;
        user.Email = model.Email;
        user.UserName = model.UserName;

        // 4. Lógica de nueva contraseña (opcional)
        if (!string.IsNullOrEmpty(model.Password))
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "La contraseña y la confirmación no coinciden.");
                return View(model);
            }
            
            // Eliminar hash actual y establecer nuevo
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, model.Password);
        }

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "El administrador fue actualizado correctamente.";
        return RedirectToAction("ListarAdministradores");
    }
    public async Task<IActionResult> ListarAdministradores()
    {
        // Obtener todos los usuarios con rol Administrador
        var admins = await _userManager.GetUsersInRoleAsync("Administrador");
        
        // Convertir a un ViewModel adecuado para la vista
        var adminViewModels = admins.Select(u => new AdministradorViewModel
        {
            Id = u.Id.ToString(),
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            UserName = u.UserName,
            Cedula = u.Cedula,
            Email = u.Email,
            EsActivo = u.EsActivo
        }).ToList();

        return View(adminViewModels);
    }


}