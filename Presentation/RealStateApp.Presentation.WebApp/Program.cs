using Microsoft.AspNetCore.Identity;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence;
using RealStateApp.Infrastructure.Persistence.Contexts;
using RealStateApp.Infrastructure.Shared;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // O true si quieres activación por correo

        // Configuración de políticas de seguridad
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true; // Esto es lo que pide el carácter especial
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>() // Ajusta esto según el nombre de tu contexto
    .AddDefaultTokenProviders();
    

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

app.Run();

