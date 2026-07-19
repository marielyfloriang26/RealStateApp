using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.User;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Presentation.WebApp.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IUploadService _uploadService;
    private readonly IEmailService _emailService;
    private readonly SignInManager<Usuario> _signInManager;

    public AccountController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IUploadService uploadService, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _uploadService = uploadService;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // 1. Buscar usuario por nombre o email
        var user = await _userManager.FindByNameAsync(vm.EmailOrUserName) 
                   ?? await _userManager.FindByEmailAsync(vm.EmailOrUserName);

        if (user == null)
        {
            ModelState.AddModelError("", "Los datos de acceso son inválidos.");
            return View(vm);
        }

        // 2. Validar estado (Solo usuarios activos)
        if (!user.EsActivo)
        {
            // Si el usuario es Agente, mostramos el mensaje personalizado
            if (await _userManager.IsInRoleAsync(user, "Agente"))
            {
                ModelState.AddModelError("", "Su cuenta de agente aún no ha sido activada por un administrador.");
            }
            else if (await _userManager.IsInRoleAsync(user, "Desarrollador"))
            {
                ModelState.AddModelError("", "Su cuenta de desarrollador se encuentra inactiva y no tiene acceso a la API.");
            }
            else
            {
                ModelState.AddModelError("", "El usuario se encuentra inactivo y no puede iniciar sesión.");
            }
            return View(vm);
        }

        // 3. Validar credenciales

        await _signInManager.SignOutAsync();

        var result = await _signInManager.PasswordSignInAsync(user.UserName, vm.Password, false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Los datos de acceso son inválidos.");
            return View(vm);
        }

        // 4. Redirección basada en roles
        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any())
        {
            ModelState.AddModelError("", "El usuario no tiene un rol válido asignado. Póngase en contacto con un administrador.");
            return View(vm);
        }

        var role = roles.First();
        return role switch
        {     // ASIGNAR REDIRECCIONES CORRECTAS
            "Administrador" => RedirectToAction("Index", "Home"),
            "Agente" => RedirectToAction("Index","Home"),
            "Cliente" => RedirectToAction("Index", "Home"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View(); // Esto buscará Views/Account/AccessDenied.cshtml
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // 1. Validar Tipo de Usuario (Seguridad)
        if (vm.TipoUsuario != "Cliente" && vm.TipoUsuario != "Agente")
        {
            ModelState.AddModelError("", "Tipo de usuario no válido.");
            return View(vm);
        }

        // 2. Validar Unicidad (Reglas de negocio)
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

        // 3. Crear usuario
        var user = new Usuario
        {
            UserName = vm.UserName,
            Email = vm.Email,
            Nombre = vm.Nombre,
            Apellido = vm.Apellido,
            TipoUsuario = vm.TipoUsuario,
            EsActivo = false // Requerimiento: Siempre inactivo
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(vm);
        }

        // 4. Subida de Foto
        try
        {
            user.FotoUrl = _uploadService.UploadFile(vm.Foto, user.Id);
            await _userManager.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            await _userManager.DeleteAsync(user); // Rollback
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }

        // 5. Asignar Rol
        await _userManager.AddToRoleAsync(user, vm.TipoUsuario);

        if (vm.TipoUsuario == "Cliente")
        {
            // Generamos un token para la activación
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmarEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

            string mensaje = $@"
                <h2>Hola {user.Nombre},</h2>
                <p>Su cuenta ha sido registrada correctamente en RealEstateApp.</p>
                <p>Para activar su usuario, utilice el siguiente enlace:</p>
                <a href='{callbackUrl}'>Activar Cuenta</a>";

            await _emailService.SendEmailAsync(user.Email, "Activación de cuenta en RealEstateApp", mensaje);
            
            TempData["Message"] = "Su cuenta ha sido creada correctamente. Revise su correo electrónico para activar su usuario.";
        }
        else
        {
            TempData["Message"] = "Su cuenta de agente ha sido creada correctamente. Un administrador debe activar su usuario antes de que pueda iniciar sesión.";
        }

        return RedirectToAction("Login", "Account");
        
    }
    
    [HttpGet]
    public async Task<IActionResult> ConfirmarEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction("Login", "Account");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            // Activamos el usuario en nuestra base de datos
            user.EmailConfirmed = true;
            user.EsActivo = true;
            await _userManager.UpdateAsync(user);
            
            TempData["Message"] = "¡Cuenta activada correctamente! Ya puede iniciar sesión.";
        }
        else
        {
            TempData["Message"] = "Error al activar la cuenta. Intente nuevamente.";
        }

        return RedirectToAction("Login", "Account");
    }

    
}