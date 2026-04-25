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

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var cliente = await context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == user.Id);
        if (cliente == null)
        {
            TempData["ErrorMessage"] = "No se encontró un perfil de cliente asociado a su usuario.";
            return RedirectToAction(nameof(Index));
        }

        return View(new CrearSolicitudViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearSolicitudViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var cliente = await context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == user.Id);
        if (cliente == null)
        {
            ModelState.AddModelError("", "No se encontró el perfil de cliente.");
            return View(model);
        }

        // Validación: Cliente debe estar activo
        if (!cliente.Activo)
        {
            ModelState.AddModelError("", "No puede solicitar un crédito porque su perfil de cliente no está activo.");
            return View(model);
        }

        // Validación: No permitir más de una solicitud Pendiente por cliente
        var tienePendiente = await context.SolicitudesCredito
            .AnyAsync(s => s.ClienteId == cliente.Id && s.Estado == EstadoSolicitud.Pendiente);

        if (tienePendiente)
        {
            ModelState.AddModelError("", "Ya tienes una solicitud de crédito en estado Pendiente. Debes esperar a que sea evaluada.");
            return View(model);
        }

        // Validación: El monto solicitado no puede superar 10 veces los ingresos mensuales
        if (model.MontoSolicitado > cliente.IngresosMensuales * 10)
        {
            ModelState.AddModelError(nameof(model.MontoSolicitado), $"El monto solicitado no puede superar 10 veces sus ingresos mensuales (Máx: {cliente.IngresosMensuales * 10:C}).");
            return View(model);
        }

        var nuevaSolicitud = new SolicitudCredito
        {
            ClienteId = cliente.Id,
            MontoSolicitado = model.MontoSolicitado,
            FechaSolicitud = DateTime.UtcNow,
            Estado = EstadoSolicitud.Pendiente
        };

        context.SolicitudesCredito.Add(nuevaSolicitud);
        await context.SaveChangesAsync();

        ViewBag.SuccessMessage = "La solicitud de crédito ha sido registrada exitosamente y se encuentra en estado Pendiente.";
        ModelState.Clear(); // Limpiamos para el form vacío

        return View(new CrearSolicitudViewModel());
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
