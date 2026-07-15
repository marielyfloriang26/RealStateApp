using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.User;

public class LoginViewModel
{
    [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario.")]
    public string EmailOrUserName { get; set; }

    [Required(ErrorMessage = "Debe ingresar su contraseña.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}