using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal_inmobiliario.Data; // Asegúrate que el namespace sea el correcto de tu proyecto
using Portal_inmobiliario.Models;
using Portal_inmobiliario.ViewModels; // Asegúrate que el namespace sea el correcto de tu proyecto
using System.Linq;
using System.Threading.Tasks;

namespace Portal_inmobiliario.Controllers // Asegúrate que el namespace sea el correcto de tu proyecto
{
    public class InmueblesController : Controller
    {
        private readonly ApplicationDbContext _context;

        // El DbContext se inyecta a través del constructor (Inyección de Dependencias)
        public InmueblesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inmuebles
        // Redirige a la acción principal que es el catálogo
        public IActionResult Index()
        {
            return RedirectToAction("Catalogo");
        }

        // GET: Inmuebles/Catalogo?Ciudad=Madrid&Tipo=Casa...
        // Esta es la acción principal que filtra, valida y pagina los resultados
        public async Task<IActionResult> Catalogo([FromQuery] CatalogoViewModel viewModel, int pagina = 1)
        {
            // --- 1. Lógica de Consulta Dinámica ---
            // Empezamos con una consulta base IQueryable. Esto es muy eficiente porque
            // no se ejecuta en la BD hasta el final.
            IQueryable<Inmueble> query = _context.Inmuebles.Where(i => i.Activo);

            // Aplicamos los filtros solo si tienen un valor
            if (!string.IsNullOrEmpty(viewModel.Ciudad))
            {
                query = query.Where(i => i.Ciudad.Contains(viewModel.Ciudad));
            }

            if (viewModel.Tipo.HasValue)
            {
                query = query.Where(i => i.Tipo == viewModel.Tipo.Value);
            }

            if (viewModel.PrecioMin.HasValue)
            {
                query = query.Where(i => i.Precio >= viewModel.PrecioMin.Value);
            }

            if (viewModel.PrecioMax.HasValue)
            {
                query = query.Where(i => i.Precio <= viewModel.PrecioMax.Value);
            }

            if (viewModel.Dormitorios.HasValue)
            {
                query = query.Where(i => i.Dormitorios >= viewModel.Dormitorios.Value);
            }

            // --- 2. Validación Server-Side ---
            // Las validaciones de [Range] en el ViewModel se verifican automáticamente.
            // Aquí añadimos nuestra validación personalizada.
            if (viewModel.PrecioMin.HasValue && viewModel.PrecioMax.HasValue && viewModel.PrecioMin > viewModel.PrecioMax)
            {
                ModelState.AddModelError(nameof(viewModel.PrecioMin), "El precio mínimo no puede ser mayor que el precio máximo.");
            }

            // Si el modelo no es válido (por los [Range] o por nuestra regla personalizada)...
            if (!ModelState.IsValid)
            {
                // Devolvemos la vista con los datos que el usuario ingresó y los mensajes de error.
                // Es importante poner una lista de inmuebles vacía para no mostrar resultados de una búsqueda inválida.
                viewModel.Inmuebles = new List<Inmueble>();
                return View(viewModel);
            }

            // --- 3. Lógica de Paginación ---
            const int tamanoPagina = 6; // Mostramos 6 inmuebles por página
            var totalInmuebles = await query.CountAsync();

            viewModel.TotalPaginas = (int)Math.Ceiling(totalInmuebles / (double)tamanoPagina);
            viewModel.PaginaActual = pagina;

            // Ahora sí, ejecutamos la consulta en la base de datos aplicando paginación
            viewModel.Inmuebles = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            // Devolvemos la vista con el ViewModel completo (filtros + resultados paginados)
            return View(viewModel);
        }
        
        // GET: Inmuebles/Detalle/5
        // Muestra la página de detalles de un solo inmueble
        public async Task<IActionResult> Detalle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles.FirstOrDefaultAsync(m => m.Id == id);
            
            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }
    }
}