namespace RealStateApp.Application.ViewModels.Admin;

public class AgenteViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public int CantidadPropiedades { get; set; }
    public bool EsActivo { get; set; }
}