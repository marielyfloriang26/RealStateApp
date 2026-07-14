namespace RealStateApp.Application.ViewModels.TipoVentas;

public class TipoVentaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public int CantidadPropiedadesAsociadas { get; set; }
}
