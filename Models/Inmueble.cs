using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal_inmobiliario.Models;

    public enum TipoInmueble
    {
    Departamento,
    Casa,
    Oficina,
    Local
    }   

    public class Inmueble
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        public string? Imagen { get; set; }

        [Required]
        public TipoInmueble Tipo { get; set; }

        [Required]
        [StringLength(50)]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Dormitorios { get; set; }

        [Range(1, int.MaxValue)]
        public int Banos { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Los metros cuadrados deben ser un valor positivo.")]
        public double MetrosCuadrados { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }

        public bool Activo { get; set; } = true;

        
        public virtual ICollection<Visita> Visitas { get; set; } = new List<Visita>();
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();


    }

