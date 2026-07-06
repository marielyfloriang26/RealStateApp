using System;
using System.Collections.Generic;
namespace RealStateApp.Domain.Entities;

public class Propiedad
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public int TipoPropiedadId { get; set; }
    public int TipoVentaId { get; set; }
    public int AgenteId { get; set; }
    public decimal Precio { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public decimal TamanoMetros { get; set; }
    public string Descripcion { get; set; } = null!;
    public string Estado { get; set; } = "Disponible";
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public TipoPropiedad? TipoPropiedad { get; set; }
    public TipoVenta? TipoVenta { get; set; }
    public Usuario? Agente { get; set; }
    public ICollection<PropiedadMejora>? PropiedadMejoras { get; set; }
    public ICollection<ImagenPropiedad>? Imagenes { get; set; }
    public ICollection<PropiedadFavorita>? PropiedadesFavoritas { get; set; }
    public ICollection<Oferta>? Ofertas { get; set; }
    public ICollection<Mensaje>? Mensajes { get; set; }
}
