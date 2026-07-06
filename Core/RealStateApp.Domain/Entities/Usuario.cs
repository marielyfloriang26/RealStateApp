using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace RealStateApp.Domain.Entities;

public class Usuario : IdentityUser<int>
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? FotoUrl { get; set; }
    public string? Cedula { get; set; }
    public string TipoUsuario { get; set; } = null!;
    public bool EsActivo { get; set; } = false;

    // Navigation properties
    public ICollection<Propiedad>? Propiedades { get; set; }
    public ICollection<PropiedadFavorita>? PropiedadesFavoritas { get; set; }
    public ICollection<Oferta>? Ofertas { get; set; }
    public ICollection<Mensaje>? MensajesEnviados { get; set; }
    public ICollection<Mensaje>? MensajesRecibidos { get; set; }
}
