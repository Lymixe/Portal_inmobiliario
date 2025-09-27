using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Portal_inmobiliario.ViewModels
{
    public class AgendarVisitaViewModel
    {
        public int InmuebleId { get; set; }

    public string? InmuebleTitulo { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Inicio de la Visita")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    [Display(Name = "Fin de la Visita")]
    public DateTime FechaFin { get; set; }

    [StringLength(500)]
    public string? Notas { get; set; }
    }
}