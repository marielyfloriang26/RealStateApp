using System;

namespace RealStateApp.Application.ViewModels.Propiedad;

public class MensajeViewModel
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int AgenteId { get; set; }
    public int PropiedadId { get; set; }
    public string Remitente { get; set; } = null!; // Cliente o Agente
    public string Contenido { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
}