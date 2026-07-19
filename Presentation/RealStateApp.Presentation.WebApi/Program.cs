using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using RealStateApp.Infrastructure.Persistence.Contexts;
using RealStateApp.Domain.Entities;
using RealStateApp.Infrastructure.Persistence; // Asegúrate de tener este using

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceInfrastructure(builder.Configuration);

// 1. CONFIGURACIÓN DE IDENTIDAD Y BASE DE DATOS
builder.Services.AddIdentity<Usuario, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 2. CONFIGURACIÓN DE JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
    
    // Personalización para manejar 403 Forbidden específicamente
    options.Events = new JwtBearerEvents
    {
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\": \"Acceso denegado. No tiene permisos para realizar esta acción.\"}");
        }
    };
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 3. PIPELINE DE SEGURIDAD
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

// Ejecución del Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        await DefaultIdentitySeed.SeedRolesAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al ejecutar el seed: {ex.Message}");
    }
}

app.Run();