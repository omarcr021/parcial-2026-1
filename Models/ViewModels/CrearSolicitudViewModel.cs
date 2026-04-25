using System.ComponentModel.DataAnnotations;

namespace parcial_2026_1.Models.ViewModels;

public class CrearSolicitudViewModel
{
    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Display(Name = "Monto Solicitado")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto solicitado debe ser mayor a 0.")]
    public decimal MontoSolicitado { get; set; }
}
