using Microsoft.AspNetCore.Identity;
using parcial_2026_1.Models;

namespace parcial_2026_1.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Crear Rol Analista
        if (!await roleManager.RoleExistsAsync("Analista"))
        {
            await roleManager.CreateAsync(new IdentityRole("Analista"));
        }

        // Crear Usuario Analista
        if (await userManager.FindByEmailAsync("analista@banco.com") == null)
        {
            var analista = new IdentityUser { UserName = "analista@banco.com", Email = "analista@banco.com", EmailConfirmed = true };
            await userManager.CreateAsync(analista, "Password123!");
            await userManager.AddToRoleAsync(analista, "Analista");
        }

        // Crear Usuarios Clientes
        var clienteUser1 = await userManager.FindByEmailAsync("cliente1@correo.com");
        if (clienteUser1 == null)
        {
            clienteUser1 = new IdentityUser { UserName = "cliente1@correo.com", Email = "cliente1@correo.com", EmailConfirmed = true };
            await userManager.CreateAsync(clienteUser1, "Password123!");
        }

        var clienteUser2 = await userManager.FindByEmailAsync("cliente2@correo.com");
        if (clienteUser2 == null)
        {
            clienteUser2 = new IdentityUser { UserName = "cliente2@correo.com", Email = "cliente2@correo.com", EmailConfirmed = true };
            await userManager.CreateAsync(clienteUser2, "Password123!");
        }

        // Crear Clientes y Solicitudes en BD si no existen
        if (!context.Clientes.Any())
        {
            var cliente1 = new Cliente { UsuarioId = clienteUser1.Id, IngresosMensuales = 5000m, Activo = true };
            var cliente2 = new Cliente { UsuarioId = clienteUser2.Id, IngresosMensuales = 3000m, Activo = true };
            
            context.Clientes.AddRange(cliente1, cliente2);
            await context.SaveChangesAsync();

            // 1 Pendiente, 1 Aprobada
            var solicitud1 = new SolicitudCredito
            {
                ClienteId = cliente1.Id,
                MontoSolicitado = 10000m,
                Estado = EstadoSolicitud.Pendiente,
                FechaSolicitud = DateTime.UtcNow
            };

            var solicitud2 = new SolicitudCredito
            {
                ClienteId = cliente2.Id,
                MontoSolicitado = 5000m,
                Estado = EstadoSolicitud.Aprobado,
                FechaSolicitud = DateTime.UtcNow.AddDays(-2)
            };

            context.SolicitudesCredito.AddRange(solicitud1, solicitud2);
            await context.SaveChangesAsync();
        }
    }
}
