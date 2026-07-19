using Microsoft.AspNetCore.Identity;
using RealStateApp.Domain.Entities;

public static class DefaultIdentitySeed
{
    public static async Task SeedRolesAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        // 1. Crear los roles
        string[] roles = { "Administrador", "Agente", "Cliente", "Desarrollador" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        // 2. Crear Usuarios (Administrador, Cliente y Agente)
        var usersToSeed = new[]
        {
            new { User = "Admin", Email = "admin@realestateapp.com", Role = "Administrador", Nombre = "Administrador", Apellido = "Sistema", Pass = "Admin123!" },
            new { User = "Cliente1", Email = "cliente@realestateapp.com", Role = "Cliente", Nombre = "Juan", Apellido = "Cliente", Pass = "Cliente123!" },
            new { User = "Agente1", Email = "agente@realestateapp.com", Role = "Agente", Nombre = "Maria", Apellido = "Agente", Pass = "Agente123!" }
        };

        foreach (var data in usersToSeed)
        {
            var userExists = await userManager.FindByEmailAsync(data.Email);
            if (userExists == null)
            {
                var newUser = new Usuario
                {
                    UserName = data.User,
                    Email = data.Email,
                    Nombre = data.Nombre,
                    Apellido = data.Apellido,
                    TipoUsuario = data.Role,
                    EmailConfirmed = true,
                    EsActivo = true
                };

                var result = await userManager.CreateAsync(newUser, data.Pass);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, data.Role);
                }
            }
        }
    }
}