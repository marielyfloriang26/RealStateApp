namespace RealStateApp.Presentation.WebApi.DTOs;

public class AgenteDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public int CantidadPropiedades { get; set; }
    public string Correo { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public bool Estado { get; set; } // true = Activo, false = Inactivo
}

public class ChangeAgentStatusRequest
{
    public bool Estado { get; set; }
}