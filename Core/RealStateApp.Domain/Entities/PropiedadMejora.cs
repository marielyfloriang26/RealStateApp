namespace RealStateApp.Domain.Entities;

public class PropiedadMejora
{
    public int PropiedadId { get; set; }
    public Propiedad? Propiedad { get; set; }
    public int MejoraId { get; set; }
    public Mejora? Mejora { get; set; }
}
