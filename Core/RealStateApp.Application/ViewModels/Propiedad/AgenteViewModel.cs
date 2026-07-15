namespace RealStateApp.Application.ViewModels.Propiedad;

public class AgenteViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? FotoUrl { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public int CantidadPropiedades { get; set; }
    public bool EsActivo { get; set; }
}