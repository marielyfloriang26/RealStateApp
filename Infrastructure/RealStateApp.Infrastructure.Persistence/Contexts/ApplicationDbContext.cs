using RealStateApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RealStateApp.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TipoPropiedad> TiposPropiedad { get; set; }
    public DbSet<TipoVenta> TiposVenta { get; set; }
    public DbSet<Mejora> Mejoras { get; set; }
    public DbSet<Propiedad> Propiedades { get; set; }
    public DbSet<PropiedadMejora> PropiedadesMejoras { get; set; }
    public DbSet<ImagenPropiedad> ImagenesPropiedad { get; set; }
    public DbSet<PropiedadFavorita> PropiedadesFavoritas { get; set; }
    public DbSet<Oferta> Ofertas { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Nombres de tablas
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        modelBuilder.Entity<TipoPropiedad>().ToTable("TiposPropiedad");
        modelBuilder.Entity<TipoVenta>().ToTable("TiposVenta");
        modelBuilder.Entity<Mejora>().ToTable("Mejoras");
        modelBuilder.Entity<Propiedad>().ToTable("Propiedades");
        modelBuilder.Entity<PropiedadMejora>().ToTable("PropiedadesMejoras");
        modelBuilder.Entity<ImagenPropiedad>().ToTable("ImagenesPropiedad");
        modelBuilder.Entity<PropiedadFavorita>().ToTable("PropiedadesFavoritas");
        modelBuilder.Entity<Oferta>().ToTable("Ofertas");
        modelBuilder.Entity<Mensaje>().ToTable("Mensajes");

        // PK Compuesta para PropiedadMejora
        modelBuilder.Entity<PropiedadMejora>()
            .HasKey(pm => new { pm.PropiedadId, pm.MejoraId });

        // Relaciones
        modelBuilder.Entity<Propiedad>()
            .HasOne(p => p.Agente)
            .WithMany(a => a.Propiedades)
            .HasForeignKey(p => p.AgenteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Cliente)
            .WithMany(c => c.MensajesEnviados)
            .HasForeignKey(m => m.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Agente)
            .WithMany(a => a.MensajesRecibidos)
            .HasForeignKey(m => m.AgenteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PropiedadFavorita>()
            .HasOne(pf => pf.Cliente)
            .WithMany(c => c.PropiedadesFavoritas)
            .HasForeignKey(pf => pf.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Oferta>()
            .HasOne(o => o.Cliente)
            .WithMany(c => c.Ofertas)
            .HasForeignKey(o => o.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
