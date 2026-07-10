using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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
    public string Password { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    [Compare("Password", ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Debe completar todos los campos requeridos.")]
    public string TipoUsuario { get; set; } // "Cliente" o "Agente"
}