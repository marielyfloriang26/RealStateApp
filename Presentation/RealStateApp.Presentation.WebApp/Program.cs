using Microsoft.AspNetCore.Identity;
using RealStateApp.Domain.Entities;
using RealStateApp.Application;
using RealStateApp.Infrastructure.Persistence;
using RealStateApp.Infrastructure.Persistence.Contexts;
using RealStateApp.Infrastructure.Shared;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddTransient<RealStateApp.Application.Interfaces.Services.IAgentePropiedadService, RealStateApp.Application.Services.AgentePropiedadService>();
builder.Services.AddTransient<RealStateApp.Application.Interfaces.Services.IMantenimientoPropiedadService, RealStateApp.Application.Services.MantenimientoPropiedadService>();
builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // O true si quieres activación por correo

        // Configuración de políticas de seguridad
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false; // Esto es lo que pide el carácter especial
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>() // Ajusta esto según el nombre de tu contexto
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Define a dónde enviar cuando no está logueado
    options.LoginPath = "/Account/Login";
    
    // Define a dónde enviar cuando el usuario está logueado pero no tiene el rol
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddApplicationLayer();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    string[] roles = { "Cliente", "Agente" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }
}

app.Run();
