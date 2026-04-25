using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using parcial_2026_1.Models;

namespace parcial_2026_1.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<SolicitudCredito> SolicitudesCredito { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Un cliente solo puede tener una solicitud en estado Pendiente
        builder.Entity<SolicitudCredito>()
            .HasIndex(s => s.ClienteId)
            .IsUnique()
            .HasFilter("[Estado] = 0");
    }
}
