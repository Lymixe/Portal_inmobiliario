using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal_inmobiliario.Models
{
    public enum EstadoVisita
{
    Solicitada,
    Confirmada,
    Cancelada
}

public class Visita
{
    public int Id { get; set; }

    [Required]
    public int InmuebleId { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    public EstadoVisita Estado { get; set; } = EstadoVisita.Solicitada;

    [StringLength(500)]
    public string? Notas { get; set; }

    // Propiedades de navegación
    [ForeignKey("InmuebleId")]
    public virtual Inmueble? Inmueble { get; set; }

    [ForeignKey("UsuarioId")]
    public virtual IdentityUser? Usuario { get; set; }
}

}