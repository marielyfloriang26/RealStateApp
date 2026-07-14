using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Application.ViewModels.Agente;

public class AgentPropiedadDetalleViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string TipoPropiedad { get; set; } = null!;
    public string TipoVenta { get; set; } = null!;
    public decimal Precio { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public decimal TamanoMetros { get; set; }
    public string Descripcion { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public List<string> Imagenes { get; set; } = new();
    public List<string> Mejoras { get; set; } = new();
}

public class AgentClienteConversacionViewModel
{
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = null!;
    public string UltimoMensaje { get; set; } = null!;
    public DateTime FechaUltimoMensaje { get; set; }
}

public class AgentMensajeViewModel
{
    public string Remitente { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
}

public class AgentConversacionViewModel
{
    public int PropiedadId { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = null!;
    public List<AgentMensajeViewModel> Mensajes { get; set; } = new();
    
    [Required(ErrorMessage = "Debe escribir un mensaje antes de enviarlo.")]
    public string NuevoMensaje { get; set; } = null!;
}

public class AgentClienteOfertasViewModel
{
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = null!;
    public int CantidadOfertas { get; set; }
    public decimal UltimaOfertaMonto { get; set; }
    public string EstadoUltimaOferta { get; set; } = null!;
}

public class AgentOfertaDetalleViewModel
{
    public int OfertaId { get; set; }
    public DateTime FechaOferta { get; set; }
    public decimal MontoOfertado { get; set; }
    public string Estado { get; set; } = null!;
}

public class AgentOfertasPorClienteViewModel
{
    public int PropiedadId { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = null!;
    public List<AgentOfertaDetalleViewModel> Ofertas { get; set; } = new();
}
