using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.DTOs.Account;

public class RegisterRequest
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es requerido.")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "La cédula es requerida.")]
    public string Cedula { get; set; }

    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "La confirmación de contraseña es requerida.")]
    public string ConfirmPassword { get; set; }
}