using System.ComponentModel.DataAnnotations;
using parcial_2026_1.Models;

namespace parcial_2026_1.Models.ViewModels;

public class MisSolicitudesViewModel : IValidatableObject
{
    public EstadoSolicitud? Estado { get; set; }

    [Display(Name = "Monto Mínimo")]
    [Range(0, double.MaxValue, ErrorMessage = "No se aceptan montos negativos.")]
    public decimal? MontoMinimo { get; set; }

    [Display(Name = "Monto Máximo")]
    [Range(0, double.MaxValue, ErrorMessage = "No se aceptan montos negativos.")]
    public decimal? MontoMaximo { get; set; }

    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicio { get; set; }

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime? FechaFin { get; set; }

    public List<SolicitudCredito> Solicitudes { get; set; } = new List<SolicitudCredito>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaInicio.HasValue && FechaFin.HasValue)
        {
            if (FechaInicio.Value > FechaFin.Value)
            {
                yield return new ValidationResult("La fecha de inicio no puede ser mayor a la fecha de fin.", new[] { nameof(FechaInicio), nameof(FechaFin) });
            }
        }
    }
}
