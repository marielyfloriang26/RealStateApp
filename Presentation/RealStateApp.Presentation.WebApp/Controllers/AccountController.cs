using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Domain.Entities;
using RealStateApp.Presentation.WebApp.Models;

namespace RealStateApp.Presentation.WebApp.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IUploadService _uploadService; // Tu servicio de subida de archivos




    [HttpGet]
    public IActionResult Register()
    {
        return View(); // Esto renderiza la vista Register.cshtml
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // Validar unicidad (Requerimiento)
        if (await _userManager.FindByNameAsync(vm.UserName) != null)
        {
            ModelState.AddModelError("", "Ya existe un usuario registrado con este nombre de usuario.");
            return View(vm);
        }

        if (await _userManager.FindByEmailAsync(vm.Email) != null)
        {
            ModelState.AddModelError("", "Ya existe un usuario registrado con este correo electrónico.");
            return View(vm);
        }

        // Crear usuario
        var user = new Usuario {
            UserName = vm.UserName,
            Email = vm.Email,
            Nombre = vm.Nombre,
            Apellido = vm.Apellido,
            TipoUsuario = vm.TipoUsuario,
            EsActivo = false // Siempre Inactivo
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (result.Succeeded)
        {
            try
            {
                string fotoPath = _uploadService.UploadFile(vm.Foto, user.Id);
                user.FotoUrl = fotoPath;
                await _userManager.UpdateAsync(user); // Guardar la ruta en la DB
            }
            catch (Exception ex)
            {
                // Si falla la foto, eliminamos el usuario creado para mantener consistencia
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }

            // 4. Asignar rol
            await _userManager.AddToRoleAsync(user, vm.TipoUsuario);

            // 5. Flujo según tipo de usuario
            if (vm.TipoUsuario == "Cliente")
            {
                // TODO: Aquí invocarás tu servicio de Email para enviar el enlace de activación
                TempData["Message"] = "Su cuenta ha sido creada correctamente. Revise su correo electrónico para activar su usuario.";
            }
            else // Agente
            {
                TempData["Message"] = "Su cuenta de agente ha sido creada correctamente. Un administrador debe activar su usuario antes de que pueda iniciar sesión.";
            }

            return RedirectToAction("Login", "Account");
        }

        ModelState.AddModelError("", "No fue posible completar el registro. Intente nuevamente más tarde.");
        return View(vm);
    }
}