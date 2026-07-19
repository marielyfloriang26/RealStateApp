using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.DTOs.Account;
using RealStateApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IConfiguration _config;

    public AccountController(UserManager<Usuario> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName) ?? await _userManager.FindByEmailAsync(model.UserName);
        
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized("Los datos de acceso son inválidos.");

        if (!user.EsActivo)
            return Unauthorized("El usuario se encuentra inactivo y no puede autenticarse.");

        var roles = await _userManager.GetRolesAsync(user);
        
        // Ahora usamos el DTO tipado
        var response = new AuthenticationResponse
        {
            Token = GenerateJwtToken(user, roles),
            Usuario = user.UserName!,
            Roles = roles.ToList(),
            Expiracion = DateTime.UtcNow.AddHours(2)
        };

        return Ok(response);
    }

    [HttpPost("register/admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest model) 
        => await RegisterUser(model, "Administrador");

    [HttpPost("register/desarrollador")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegisterDesarrollador([FromBody] RegisterRequest model) 
        => await RegisterUser(model, "Desarrollador");

    private async Task<IActionResult> RegisterUser(RegisterRequest model, string role)
    {
        if (!ModelState.IsValid) return BadRequest("Los datos enviados no son válidos.");
        
        if (model.Password != model.ConfirmPassword) 
            return BadRequest("La contraseña y la confirmación de contraseña no coinciden.");
        
        if (await _userManager.FindByEmailAsync(model.Email) != null)
            return BadRequest("Ya existe un usuario registrado con este correo electrónico.");

        if (await _userManager.FindByNameAsync(model.UserName) != null)
            return BadRequest("Ya existe un usuario registrado con este nombre de usuario.");

        if (await _userManager.Users.AnyAsync(u => u.Cedula == model.Cedula))
            return BadRequest("Ya existe un usuario registrado con esta cédula.");

        var user = new Usuario { 
            Nombre = model.Nombre, 
            Apellido = model.Apellido, 
            Cedula = model.Cedula, 
            Email = model.Email, 
            UserName = model.UserName, 
            EsActivo = true, 
            EmailConfirmed = true 
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) return BadRequest("Los datos enviados no son válidos.");
        
        await _userManager.AddToRoleAsync(user, role);
        return StatusCode(201, "Usuario creado correctamente.");
    }

    private string GenerateJwtToken(Usuario user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}