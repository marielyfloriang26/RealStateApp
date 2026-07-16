using System;

namespace RealStateApp.Application.ViewModels.Propiedad;

public class OfertaViewModel
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int PropiedadId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaOferta { get; set; }
    public string Estado { get; set; } = null!; // Pendiente, Aceptada, Rechazada
}