using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealStateApp.Application.ViewModels.Propiedades;

public class PropiedadViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string TipoPropiedadNombre { get; set; } = null!;
    public string TipoVentaNombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public decimal TamanoMetros { get; set; }
    public string ImagenPrincipalUrl { get; set; } = null!;
    public string Estado { get; set; } = null!;
}

public class SavePropiedadViewModel
{
    public int Id { get; set; }
    
    public string Codigo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de propiedad es requerido.")]
    [Display(Name = "Tipo de Propiedad")]
    public int TipoPropiedadId { get; set; }

    [Required(ErrorMessage = "El tipo de venta es requerido.")]
    [Display(Name = "Tipo de Venta")]
    public int TipoVentaId { get; set; }

    [Required(ErrorMessage = "El precio es requerido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor numérico mayor que cero.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "El tamaño de la propiedad es requerido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El tamaño debe ser un valor numérico mayor que cero.")]
    [Display(Name = "Tamaño de la propiedad (Metros)")]
    public decimal TamanoMetros { get; set; }

    [Required(ErrorMessage = "La cantidad de habitaciones es requerida.")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero.")]
    [Display(Name = "Cantidad de Habitaciones")]
    public int CantidadHabitaciones { get; set; }

    [Required(ErrorMessage = "La cantidad de baños es requerida.")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero.")]
    [Display(Name = "Cantidad de Baños")]
    public int CantidadBanos { get; set; }

    [Required(ErrorMessage = "Debe seleccionarse al menos una mejora.")]
    public List<int> MejorasIds { get; set; } = new();

    public List<IFormFile>? ImagenesFiles { get; set; }
    
    // For edition: keeping existing image URLs
    public List<string> ImagenesActuales { get; set; } = new();
}

public class TipoPropiedadViewModel { public int Id { get; set; } public string Nombre { get; set; } = null!; }
public class TipoVentaViewModel { public int Id { get; set; } public string Nombre { get; set; } = null!; }
public class MejoraViewModel { public int Id { get; set; } public string Nombre { get; set; } = null!; }
