namespace RealStateApp.Domain.Entities;

public class ImagenPropiedad
{
    public int Id { get; set; }
    public int PropiedadId { get; set; }
    public string ImagenUrl { get; set; } = null!;
    public Propiedad? Propiedad { get; set; }
}
