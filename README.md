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

## Variables de Entorno (Producción - Render)

Para desplegar en Render como Web Service, debes configurar las siguientes variables de entorno:

- `ASPNETCORE_ENVIRONMENT`: `Production`
- `ASPNETCORE_URLS`: `http://0.0.0.0:${PORT}`
- `ConnectionStrings__DefaultConnection`: `DataSource=app.db;Cache=Shared` (o cadena de conexión si se usa otro motor de BD).
- `Redis__ConnectionString`: Cadena de conexión provista por tu servidor Redis gestionado (ej. el Internal URL de Redis en Render).

## URL de Render



## Usuarios Iniciales (Seeding)

Al iniciar el proyecto, se crean los siguientes datos de prueba (contraseña para todos: `Password123!`):
- **Analista**: `analista@banco.com` (Rol: Analista)
- **Cliente 1**: `cliente1@correo.com` (con 1 solicitud Pendiente)
- **Cliente 2**: `cliente2@correo.com` (con 1 solicitud Aprobada)