using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using parcial_2026_1.Data;
using parcial_2026_1.Models;
using parcial_2026_1.Models.ViewModels;

namespace parcial_2026_1.Controllers;

[Authorize(Roles = "Analista")]
public class AnalistaController(ApplicationDbContext context, IDistributedCache cache) : Controller
{
    public async Task<IActionResult> Index()
    {
        var solicitudes = await context.SolicitudesCredito
            .Include(s => s.Cliente)
                .ThenInclude(c => c!.Usuario)
            .Where(s => s.Estado == EstadoSolicitud.Pendiente)
            .OrderBy(s => s.FechaSolicitud)
            .ToListAsync();

        return View(solicitudes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Evaluar(EvaluarSolicitudViewModel model)
    {
        var solicitud = await context.SolicitudesCredito
            .Include(s => s.Cliente)
            .FirstOrDefaultAsync(s => s.Id == model.SolicitudId);

        if (solicitud == null)
        {
            TempData["ErrorMessage"] = "Solicitud no encontrada.";
            return RedirectToAction(nameof(Index));
        }

        if (solicitud.Estado != EstadoSolicitud.Pendiente)
        {
            TempData["ErrorMessage"] = "La solicitud ya ha sido procesada previamente.";
            return RedirectToAction(nameof(Index));
        }

        if (model.Accion == "Aprobar")
        {
            if (solicitud.Cliente != null && solicitud.MontoSolicitado > solicitud.Cliente.IngresosMensuales * 5)
            {
                TempData["ErrorMessage"] = $"No se puede aprobar. El monto ({solicitud.MontoSolicitado:C}) excede 5 veces los ingresos mensuales ({solicitud.Cliente.IngresosMensuales:C}).";
                return RedirectToAction(nameof(Index));
            }

            solicitud.Estado = EstadoSolicitud.Aprobado;
        }
        else if (model.Accion == "Rechazar")
        {
            if (string.IsNullOrWhiteSpace(model.MotivoRechazo))
            {
                TempData["ErrorMessage"] = "Debe proporcionar un motivo de rechazo obligatoriamente.";
                return RedirectToAction(nameof(Index));
            }

            solicitud.Estado = EstadoSolicitud.Rechazado;
            solicitud.MotivoRechazo = model.MotivoRechazo;
        }
        else
        {
            return BadRequest();
        }

        await context.SaveChangesAsync();

        if (solicitud.Cliente?.UsuarioId != null)
        {
            await cache.RemoveAsync($"solicitudes_usuario_{solicitud.Cliente.UsuarioId}");
        }

        TempData["SuccessMessage"] = $"La solicitud #{solicitud.Id} fue {(solicitud.Estado == EstadoSolicitud.Aprobado ? "aprobada" : "rechazada")} con éxito.";
        return RedirectToAction(nameof(Index));
    }
}
