using System.Collections.Generic;

namespace RealStateApp.Application.ViewModels.Propiedad;

public class PropiedadViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public int TipoPropiedadId { get; set; }
    public string TipoPropiedadNombre { get; set; } = null!;
    public int TipoVentaId { get; set; }
    public string TipoVentaNombre { get; set; } = null!;
    public int AgenteId { get; set; }
    public string AgenteNombre { get; set; } = null!;
    public string AgenteApellido { get; set; } = null!;
    public string? AgenteTelefono { get; set; }
    public string? AgenteFotoUrl { get; set; }
    public string? AgenteCorreo { get; set; }
    public decimal Precio { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public decimal TamanoMetros { get; set; }
    public string Descripcion { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public List<string> ImagenesUrl { get; set; } = new();
    public List<string> Mejoras { get; set; } = new();
}