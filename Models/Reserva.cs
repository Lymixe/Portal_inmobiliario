using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal_inmobiliario.Models;

public class Reserva
{
    public int Id { get; set; }

    [Required]
    public int InmuebleId { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime FechaExpiracion { get; set; }

    // Propiedades de navegación
    [ForeignKey("InmuebleId")]
    public virtual Inmueble? Inmueble { get; set; }

    [ForeignKey("UsuarioId")]
    public virtual IdentityUser? Usuario { get; set; }
}