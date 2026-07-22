using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.ViewModels.Admin;
using RealStateApp.Application.ViewModels.Desarrollador;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence.Contexts;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class DesarrolladorController : Controller
{
    private readonly UserManager<Usuario> _userManager;

    public DesarrolladorController(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> ListarDesarrolladores()
    {
        var devs = await _userManager.GetUsersInRoleAsync("Desarrollador");
        return View(devs);
    }
    [HttpGet]
    public IActionResult CrearDesarrollador()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearDesarrollador(SaveDesarrolladorViewModel model)
    {
        // Validaciones de requeridos 
        if (!ModelState.IsValid) 
        {
            ModelState.AddModelError("", "Debe completar todos los campos requeridos.");
            return View(model);
        }

        // Validaciones adicionales de negocio
        if (_userManager.Users.Any(u => u.Cedula == model.Cedula))
            ModelState.AddModelError("Cedula", "Ya existe un usuario registrado con esta cédula.");
        
        if (_userManager.Users.Any(u => u.Email == model.Email))
            ModelState.AddModelError("Email", "Ya existe un usuario registrado con este correo electrónico.");
            
        if (_userManager.Users.Any(u => u.UserName == model.UserName))
            ModelState.AddModelError("UserName", "Ya existe un usuario registrado con este nombre de usuario.");

        if (model.Password != model.ConfirmPassword)
            ModelState.AddModelError("Password", "La contraseña y la confirmación de contraseña no coinciden.");

        if (!ModelState.IsValid) return View(model);

        // Creación del usuario
        var user = new Usuario 
        { 
            UserName = model.UserName, 
            Email = model.Email, 
            Nombre = model.Nombre, 
            Apellido = model.Apellido, 
            Cedula = model.Cedula,
            EsActivo = true, // Activo por defecto
            TipoUsuario = "Desarrollador" 
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Desarrollador");
            TempData["Success"] = "El desarrollador fue creado correctamente.";
            return RedirectToAction("ListarDesarrolladores");
        }

        // Manejo de errores
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditarDesarrollador(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || !await _userManager.IsInRoleAsync(user, "Desarrollador")) return NotFound();

        var model = new EditarDesarrolladorViewModel
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
    public async Task<IActionResult> EditarDesarrollador(EditarDesarrolladorViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        if (!await _userManager.IsInRoleAsync(user, "Desarrollador"))
        {
            return Forbid(); // Bloquea si no es un desarrollador
        }

        // Validaciones de unicidad
        if (_userManager.Users.Any(u => u.Cedula == model.Cedula && u.Id.ToString() != model.Id.ToString()))
            ModelState.AddModelError("Cedula", "Ya existe un usuario registrado con esta cédula.");
        
        if (_userManager.Users.Any(u => u.Email == model.Email && u.Id.ToString() != model.Id.ToString()))
            ModelState.AddModelError("Email", "Ya existe un usuario registrado con este correo.");

        if (!ModelState.IsValid) return View(model);

        user.Nombre = model.Nombre;
        user.Apellido = model.Apellido;
        user.Cedula = model.Cedula;
        user.Email = model.Email;
        user.UserName = model.UserName;

        if (!string.IsNullOrEmpty(model.Password))
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "La contraseña y la confirmación no coinciden.");
                return View(model);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, model.Password);
        }

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "El desarrollador fue actualizado correctamente.";
        return RedirectToAction("ListarDesarrolladores");
    }

    [HttpGet]
    public async Task<IActionResult> CambiarEstado(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || !await _userManager.IsInRoleAsync(user, "Desarrollador"))
        {
            TempData["Error"] = "El desarrollador seleccionado no existe.";
            return RedirectToAction("ListarDesarrolladores");
        }

        return View(user); 
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarCambiarEstado(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || !await _userManager.IsInRoleAsync(user, "Desarrollador")) return NotFound();

        user.EsActivo = !user.EsActivo;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = $"El desarrollador fue {(user.EsActivo ? "activado" : "inactivado")} correctamente.";
        return RedirectToAction("ListarDesarrolladores");
    }
}