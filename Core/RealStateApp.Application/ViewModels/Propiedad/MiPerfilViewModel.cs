using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.Propiedad;

public class MiPerfilViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido.")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es requerido.")]
    public string Teléfono { get; set; } = null!;

    public string? FotoUrl { get; set; }

    public IFormFile? FotoFile { get; set; }
}