using System;
namespace RealStateApp.Domain.Entities;

public class Oferta
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int PropiedadId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaOferta { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "Pendiente";

    public Usuario? Cliente { get; set; }
    public Propiedad? Propiedad { get; set; }
}
