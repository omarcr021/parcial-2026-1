using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parcial_2026_1.Data;
using parcial_2026_1.Models;
using parcial_2026_1.Models.ViewModels;

namespace parcial_2026_1.Controllers;

[Authorize]
public class SolicitudesController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
{
    public async Task<IActionResult> Index(MisSolicitudesViewModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var cliente = await context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == user.Id);
        if (cliente == null)
        {
            model.Solicitudes = new List<SolicitudCredito>();
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var query = context.SolicitudesCredito
            .Include(s => s.Cliente)
            .Where(s => s.ClienteId == cliente.Id)
            .AsQueryable();

        if (model.Estado.HasValue)
        {
            query = query.Where(s => s.Estado == model.Estado.Value);
        }

        if (model.MontoMinimo.HasValue)
        {
            query = query.Where(s => s.MontoSolicitado >= model.MontoMinimo.Value);
        }

        if (model.MontoMaximo.HasValue)
        {
            query = query.Where(s => s.MontoSolicitado <= model.MontoMaximo.Value);
        }

        if (model.FechaInicio.HasValue)
        {
            query = query.Where(s => s.FechaSolicitud.Date >= model.FechaInicio.Value.Date);
        }

        if (model.FechaFin.HasValue)
        {
            query = query.Where(s => s.FechaSolicitud.Date <= model.FechaFin.Value.Date);
        }

        model.Solicitudes = await query.OrderByDescending(s => s.FechaSolicitud).ToListAsync();

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var solicitud = await context.SolicitudesCredito
            .Include(s => s.Cliente)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (solicitud == null) return NotFound();

        if (solicitud.Cliente?.UsuarioId != user.Id)
        {
            return Forbid();
        }

        return View(solicitud);
    }
}
