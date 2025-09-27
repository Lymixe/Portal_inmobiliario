using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal_inmobiliario.Data;

namespace Portal_inmobiliario.Controllers
{
   
public class InmueblesController : Controller
{
    private readonly ApplicationDbContext _context;

    public InmueblesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {

        return RedirectToAction("Catalogo");
    }

    public async Task<IActionResult> Catalogo()
    {
        var inmueblesActivos = await _context.Inmuebles
            .Where(i => i.Activo)
            .ToListAsync();

        return View(inmueblesActivos);
    }
}
}