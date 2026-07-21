using System.Collections.Generic;

namespace RealStateApp.Presentation.WebApi.DTOs;

public class PropiedadDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string TipoPropiedad { get; set; } = null!;
    public string TipoVenta { get; set; } = null!;
    public decimal Precio { get; set; }
    public decimal TamanoMetros { get; set; } 
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public string Descripcion { get; set; } = null!;
    public List<string> Mejoras { get; set; } = new();
    public string Agente { get; set; } = null!; // Nombre completo del Agente 
    public int AgenteId { get; set; }
    public string Estado { get; set; } = null!; // Disponible o Vendida
}