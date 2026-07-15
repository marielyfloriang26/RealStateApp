using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealStateApp.Application.ViewModels.User;
public class RegisterViewModel
{
    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public IFormFile Foto { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]

    public string Email { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [RegularExpression(@"^.*[^a-zA-Z0-9].*$", ErrorMessage = "La contraseña debe incluir al menos una mayúscula, un número y un carácter especial. (ej: !, @, #, $).")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string TipoUsuario { get; set; } // "Cliente" o "Agente"
}