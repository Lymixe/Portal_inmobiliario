using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal_inmobiliario.Data;
using Portal_inmobiliario.Models;
using Portal_inmobiliario.ViewModels;
using System.Security.Claims;

namespace Portal_inmobiliario.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InmueblesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Catalogo");
        }

        public async Task<IActionResult> Catalogo([FromQuery] CatalogoViewModel viewModel, int pagina = 1)
        {
            IQueryable<Inmueble> query = _context.Inmuebles.Where(i => i.Activo);

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

            if (viewModel.PrecioMin.HasValue && viewModel.PrecioMax.HasValue && viewModel.PrecioMin > viewModel.PrecioMax)
            {
                ModelState.AddModelError(nameof(viewModel.PrecioMin), "El precio mínimo no puede ser mayor que el precio máximo.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.Inmuebles = new List<Inmueble>();
                return View(viewModel);
            }

            const int tamanoPagina = 6;
            var totalInmuebles = await query.CountAsync();

            viewModel.TotalPaginas = (int)Math.Ceiling(totalInmuebles / (double)tamanoPagina);
            viewModel.PaginaActual = pagina;

            viewModel.Inmuebles = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> Detalle(int? id)
        {
            if (id == null) return NotFound();
            
            var inmueble = await _context.Inmuebles.FirstOrDefaultAsync(m => m.Id == id);
            
            if (inmueble == null) return NotFound();

            ViewBag.TieneReservaActiva = await _context.Reservas
                .AnyAsync(r => r.InmuebleId == id && r.FechaExpiracion > DateTime.UtcNow);

            var visitaViewModel = new AgendarVisitaViewModel
            {
                InmuebleId = inmueble.Id,
                InmuebleTitulo = inmueble.Titulo,
                FechaInicio = DateTime.Now.Date.AddHours(9),
                FechaFin = DateTime.Now.Date.AddHours(10)
            };
            ViewBag.VisitaViewModel = visitaViewModel;

            return View(inmueble);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgendarVisita(AgendarVisitaViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // --- CAMBIO 1: CARGAR Y VALIDAR EL INMUEBLE AL PRINCIPIO ---
            var inmueble = await _context.Inmuebles.FindAsync(model.InmuebleId);
            if (inmueble == null)
            {
                TempData["MensajeError"] = "El inmueble que intentaba visitar ya no existe.";
                return RedirectToAction("Catalogo");
            }

            if (model.FechaInicio >= model.FechaFin)
            {
                ModelState.AddModelError(nameof(model.FechaInicio), "La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            var horaInicio = model.FechaInicio.TimeOfDay;
            var horaFin = model.FechaFin.TimeOfDay;
            if (horaInicio < new TimeSpan(8, 0, 0) || horaFin > new TimeSpan(19, 0, 0))
            {
                ModelState.AddModelError(nameof(model.FechaInicio), "Las visitas solo pueden ser entre las 08:00 y las 19:00.");
            }
            
            var haySolapamiento = await _context.Visitas
                .AnyAsync(v => v.InmuebleId == model.InmuebleId &&
                               v.Estado != EstadoVisita.Cancelada &&
                               v.FechaInicio < model.FechaFin &&
                               v.FechaFin > model.FechaInicio);

            if (haySolapamiento)
            {
                ModelState.AddModelError("", "El horario seleccionado ya no está disponible. Por favor, elija otro.");
            }

            // --- CAMBIO 2: EL BLOQUE DE ERROR AHORA ES MÁS SEGURO ---
            if (!ModelState.IsValid)
            {
                TempData["MensajeError"] = "No se pudo agendar la visita. Por favor, revise los errores.";
                ViewBag.TieneReservaActiva = await _context.Reservas.AnyAsync(r => r.InmuebleId == model.InmuebleId && r.FechaExpiracion > DateTime.UtcNow);
                ViewBag.VisitaViewModel = model; 
                // Reutilizamos la variable 'inmueble' que ya sabemos que no es null.
                return View("Detalle", inmueble);
            }

            var visita = new Visita
            {
                InmuebleId = model.InmuebleId,
                UsuarioId = userId,
                FechaInicio = model.FechaInicio,
                FechaFin = model.FechaFin,
                Notas = model.Notas,
                Estado = EstadoVisita.Solicitada
            };

            _context.Add(visita);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "¡Visita agendada correctamente! Un asesor la confirmará pronto.";
            return RedirectToAction("Detalle", new { id = model.InmuebleId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int inmuebleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var hayReservaActiva = await _context.Reservas
                .AnyAsync(r => r.InmuebleId == inmuebleId && r.FechaExpiracion > DateTime.UtcNow);
            
            if (hayReservaActiva)
            {
                TempData["MensajeError"] = "Este inmueble ya tiene una reserva activa.";
                return RedirectToAction("Detalle", new { id = inmuebleId });
            }
            
            var reserva = new Reserva
            {
                InmuebleId = inmuebleId,
                UsuarioId = userId,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddHours(48)
            };

            _context.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "¡Inmueble reservado por 48 horas! Por favor, contacte a un asesor para continuar.";
            return RedirectToAction("Detalle", new { id = inmuebleId });
        }
    }
}