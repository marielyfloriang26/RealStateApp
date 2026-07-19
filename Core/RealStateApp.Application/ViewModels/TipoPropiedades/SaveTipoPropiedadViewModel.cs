using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.TipoPropiedades;

public class SaveTipoPropiedadViewModel
{
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string Nombre { get; set; } = null!;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string Descripcion { get; set; } = null!;

    public bool HasError { get; set; }
    public string? Error { get; set; }
}
