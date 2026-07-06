using System.Collections.Generic;
namespace RealStateApp.Domain.Entities;

public class Mejora
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public ICollection<PropiedadMejora>? PropiedadMejoras { get; set; }
}
