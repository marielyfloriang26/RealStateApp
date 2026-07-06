using System;
namespace RealStateApp.Domain.Entities;

public class Mensaje
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int AgenteId { get; set; }
    public int PropiedadId { get; set; }
    public string Remitente { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    public Usuario? Cliente { get; set; }
    public Usuario? Agente { get; set; }
    public Propiedad? Propiedad { get; set; }
}
