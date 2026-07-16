using Microsoft.AspNetCore.Identity;
using RealStateApp.Domain.Entities;

public static class DefaultIdentitySeed
{
    public static async Task SeedRolesAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Crear los roles si no existen
        await roleManager.CreateAsync(new IdentityRole("Administrador"));
        await roleManager.CreateAsync(new IdentityRole("Agente"));
        await roleManager.CreateAsync(new IdentityRole("Cliente"));
        await roleManager.CreateAsync(new IdentityRole("Desarrollador"));

        // 2. Crear el Administrador por defecto
        var defaultAdmin = new Usuario
        {
            UserName = "Admin",
            Email = "admin@realestateapp.com",
            Nombre = "Administrador",
            Apellido = "Sistema",
            TipoUsuario = "Administrador",
            EmailConfirmed = true,
            EsActivo = true
        };

        if (userManager.Users.All(u => u.UserName != defaultAdmin.UserName))
        {
            var user = await userManager.FindByEmailAsync(defaultAdmin.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultAdmin, "Admin123!"); // ¡contraseña!
                await userManager.AddToRoleAsync(defaultAdmin, "Administrador");
            }
        }
    }
}