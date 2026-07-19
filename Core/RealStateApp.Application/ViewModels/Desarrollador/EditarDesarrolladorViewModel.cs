using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.Desarrollador;

public class EditarDesarrolladorViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "El campo es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El campo es obligatorio.")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "El campo es obligatorio.")]
    public string Cedula { get; set; }

    [Required(ErrorMessage = "El campo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El campo es obligatorio.")]
    public string UserName { get; set; }

    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}