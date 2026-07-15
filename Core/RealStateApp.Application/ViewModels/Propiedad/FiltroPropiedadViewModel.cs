using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.Propiedad;

public class FiltroPropiedadViewModel
{
    public int? TipoPropiedadId { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser menor que cero.")]
    public decimal? PrecioMinimo { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "El precio máximo no puede ser menor que cero.")]
    public decimal? PrecioMaximo { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero.")]
    public int? CantidadHabitaciones { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero.")]
    public int? CantidadBanos { get; set; }
}