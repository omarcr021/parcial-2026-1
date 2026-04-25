using System.ComponentModel.DataAnnotations;

namespace parcial_2026_1.Models;

public enum EstadoSolicitud
{
    Pendiente,
    Aprobado,
    Rechazado
}

public class SolicitudCredito : IValidatableObject
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El monto solicitado debe ser mayor a 0.")]
    public decimal MontoSolicitado { get; set; }

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    public string? MotivoRechazo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Estado == EstadoSolicitud.Aprobado && Cliente != null)
        {
            if (MontoSolicitado > Cliente.IngresosMensuales * 5)
            {
                yield return new ValidationResult("No se puede aprobar una solicitud si el monto solicitado es mayor a 5 veces los ingresos mensuales.", new[] { nameof(MontoSolicitado), nameof(Estado) });
            }
        }
    }
}
