namespace RealStateApp.Application.ViewModels.TipoPropiedades;

public class TipoPropiedadViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public int CantidadPropiedadesAsociadas { get; set; }
}
