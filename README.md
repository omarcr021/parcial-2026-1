# Sistema de Gestión de Solicitudes de Crédito

Plataforma web interna para gestionar solicitudes de crédito de clientes.

## Stack Tecnológico
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (SQLite en local)
- ASP.NET Core Identity

## Pasos Locales

1. Clonar el repositorio:
   ```bash
   git clone <repo_url>
   cd parcial-2026-1
   ```
2. Restaurar paquetes:
   ```bash
   dotnet restore
   ```
3. Ejecutar la aplicación (las migraciones y datos iniciales se aplicarán automáticamente en el primer inicio):
   ```bash
   dotnet run
   ```
4. Acceder en el navegador a la URL que aparezca en consola (ej. `https://localhost:5001`).

## Migraciones

Para agregar una nueva migración con EF Core Tools:
```bash
dotnet ef migrations add NombreMigracion
```
Para actualizar la base de datos manualmente:
```bash
dotnet ef database update
```

## Variables de Entorno

Para el desarrollo local se usa la cadena de conexión en `appsettings.Development.json`.
En producción (Render) se configurarán las siguientes variables de entorno:

- `ConnectionStrings__DefaultConnection`: Cadena de conexión para la base de datos.
- (Otras variables como `ConnectionStrings__RedisConnection` se agregarán en el futuro).

## URL de Render

*Pendiente de despliegue.*

## Usuarios Iniciales (Seeding)

Al iniciar el proyecto, se crean los siguientes datos de prueba (contraseña para todos: `Password123!`):
- **Analista**: `analista@banco.com` (Rol: Analista)
- **Cliente 1**: `cliente1@correo.com` (con 1 solicitud Pendiente)
- **Cliente 2**: `cliente2@correo.com` (con 1 solicitud Aprobada)