using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Portal_inmobiliario.Models;

namespace Portal_inmobiliario.ViewModels
{
    public class CatalogoViewModel
    {
        public string? Ciudad { get; set; }
    public TipoInmueble? Tipo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser negativo.")]
    public decimal? PrecioMin { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio máximo no puede ser negativo.")]
    public decimal? PrecioMax { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El número de dormitorios no puede ser negativo.")]
    public int? Dormitorios { get; set; }
    
    // --- Propiedades para los resultados y paginación ---
    public List<Inmueble> Inmuebles { get; set; } = new();
    public int PaginaActual { get; set; } = 1;
    public int TotalPaginas { get; set; }
    }
}