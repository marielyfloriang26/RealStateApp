namespace RealStateApp.Application.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int AgentesActivos { get; set; }
    public int AgentesInactivos { get; set; }
    public int ClientesActivos { get; set; }
    public int ClientesInactivos { get; set; }
    public int DesarrolladoresActivos { get; set; }
    public int DesarrolladoresInactivos { get; set; }
    public int PropiedadesDisponibles { get; set; }
    public int PropiedadesVendidas { get; set; }
}