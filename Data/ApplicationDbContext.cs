using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portal_inmobiliario.Models;

namespace Portal_inmobiliario.Data;

public class ApplicationDbContext : IdentityDbContext

{
    // --- DBSETS ---
    public DbSet<Inmueble> Inmuebles { get; set; }
    public DbSet<Visita> Visitas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // --- CONFIGURACIÓN DEL MODELO Y SEMILLA DE DATOS ---
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // MUY IMPORTANTE: Esta línea debe ir primero.

        // --- Configuración de Inmueble ---
        builder.Entity<Inmueble>(entity =>
        {
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.ToTable(tb => tb.HasCheckConstraint("CK_Inmueble_Precio", "Precio > 0"));
            entity.ToTable(tb => tb.HasCheckConstraint("CK_Inmueble_MetrosCuadrados", "MetrosCuadrados > 0"));
        });

        // --- Configuración de Visita ---
        builder.Entity<Visita>(entity =>
        {
            entity.ToTable(tb => tb.HasCheckConstraint("CK_Visita_Fechas", "FechaFin > FechaInicio"));
        });

       
        builder.Entity<Inmueble>().HasData(
            new Inmueble
            {
                Id = 1,
                Codigo = "PISO-MAD-001",
                Titulo = "Luminoso Piso en el Barrio de Salamanca",
                Tipo = TipoInmueble.Departamento, // "Piso" en España
                Ciudad = "Madrid",
                Direccion = "Calle de Goya, 75",
                Dormitorios = 3,
                Banos = 2,
                MetrosCuadrados = 110.5,
                Precio = 650000m,
                Activo = true,
                Imagen = "/images/piso_salamanca.jpg"
            },
            new Inmueble
            {
                Id = 2,
                Codigo = "CHALET-BCN-001",
                Titulo = "Espectacular Chalet con Vistas al Mar en Sitges",
                Tipo = TipoInmueble.Casa, // "Chalet"
                Ciudad = "Barcelona",
                Direccion = "Avinguda del Port, 22, Sitges",
                Dormitorios = 5,
                Banos = 4,
                MetrosCuadrados = 320,
                Precio = 1250000m,
                Activo = true,
                Imagen = "/images/chalet_sitges.jpg"
            },
            new Inmueble
            {
                Id = 3,
                Codigo = "OFI-VLC-001",
                Titulo = "Moderna Oficina en el Centro Financiero",
                Tipo = TipoInmueble.Oficina,
                Ciudad = "Valencia",
                Direccion = "Avenida de las Cortes Valencianas, 58",
                Dormitorios = 0,
                Banos = 2,
                MetrosCuadrados = 150,
                Precio = 850000m,
                Activo = true,
                Imagen = "/images/oficina_valencia.jpg"
            },
            new Inmueble
            {
                Id = 4,
                Codigo = "LOFT-SEV-001",
                Titulo = "Loft con Encanto en el Barrio de Triana",
                Tipo = TipoInmueble.Departamento, // "Loft" es un tipo de departamento
                Ciudad = "Sevilla",
                Direccion = "Calle Betis, 41",
                Dormitorios = 1,
                Banos = 1,
                MetrosCuadrados = 65,
                Precio = 280000m,
                Activo = false, // Inmueble inactivo para pruebas de filtro
                Imagen = "/images/loft_triana.jpg"
            },
            new Inmueble
            {
                Id = 5,
                Codigo = "LOCAL-MAD-001",
                Titulo = "Local Comercial en Zona Peatonal de Malasaña",
                Tipo = TipoInmueble.Local,
                Ciudad = "Madrid",
                Direccion = "Calle del Pez, 15",
                Dormitorios = 0,
                Banos = 1,
                MetrosCuadrados = 80,
                Precio = 450000m,
                Activo = true,
                Imagen = "/images/local_malasana.jpg"
            }
        );
    }

}
