using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.User;

public class LoginViewModel
{
    [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario y contraseña.")]
    public string EmailOrUserName { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario y contraseña.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}