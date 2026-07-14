using RealStateApp.Application;
using RealStateApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddApplicationLayer();
builder.Services.AddTransient<RealStateApp.Application.Interfaces.Services.IAgentePropiedadService, RealStateApp.Application.Services.AgentePropiedadService>();
builder.Services.AddTransient<RealStateApp.Application.Interfaces.Services.IMantenimientoPropiedadService, RealStateApp.Application.Services.MantenimientoPropiedadService>();

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
