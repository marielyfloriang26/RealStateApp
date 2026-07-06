namespace RealStateApp.Domain.Entities;

public class PropiedadFavorita
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int PropiedadId { get; set; }

    public Usuario? Cliente { get; set; }
    public Propiedad? Propiedad { get; set; }
}
