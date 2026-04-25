namespace parcial_2026_1.Models.ViewModels;

public class EvaluarSolicitudViewModel
{
    public int SolicitudId { get; set; }
    public string Accion { get; set; } = string.Empty; // "Aprobar" or "Rechazar"
    public string? MotivoRechazo { get; set; }
}
