using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.Mejoras;

public class SaveMejoraViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string Descripcion { get; set; } = null!;
    
    public bool HasError { get; set; }
    public string? Error { get; set; }
}
