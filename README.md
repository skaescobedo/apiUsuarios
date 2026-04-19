# API REST - Gestión de Usuarios

API REST desarrollada con **ASP.NET Core 8 + Entity Framework Core + MySQL** para gestionar usuarios en una organización con múltiples sucursales y roles.

## Tecnologías

- C# / .NET 8 (ASP.NET Core Web API)
- Entity Framework Core 8
- Pomelo.EntityFrameworkCore.MySql
- Swagger / OpenAPI

## Arquitectura

Separación por capas:

- `Controllers`: endpoints HTTP
- `Services`: lógica de negocio
- `Data`: `AppDbContext` y configuración EF Core
- `DTOs`: contratos de entrada/salida
- `Models`: entidades de dominio

## Requisitos

- .NET SDK 8
- MySQL Server 8+
- Docker Desktop (opcional para ejecución con contenedores)

## Configuración

Editar `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=api_usuarios_db;user=YOUR_USER;password=YOUR_PASSWORD;"
}
```

## Ejecución

1. Restaurar paquetes:
   ```bash
   dotnet restore
   ```
2. Aplicar migraciones:
   ```bash
   dotnet ef database update
   ```
3. Ejecutar API:
   ```bash
   dotnet run
   ```

Swagger disponible en:

- `https://localhost:{puerto}/swagger`
- `http://localhost:{puerto}/swagger`

## Docker (configuración lista)

Levantar API + MySQL con Docker Compose:

```bash
docker compose up -d --build
```

Accesos:

- API: `http://localhost:5086`
- Swagger: `http://localhost:5086/swagger`
- MySQL (host): `localhost:3307`
- MySQL (red interna Docker): `mysql:3306`

Notas Docker:

- El contenedor MySQL ejecuta automáticamente `docker/mysql-init/01-init.sql` en el primer arranque del volumen.
- Ese script deja estructura + datos seed listos para probar endpoints.
- Si quieres re-inicializar desde cero, usa:
  ```bash
  docker compose down -v
  docker compose up -d --build
  ```

Detener contenedores:

```bash
docker compose down
```

Detener y eliminar volumen de datos:

```bash
docker compose down -v
```

## Endpoints

### Users

- `GET /api/users`
  - Filtros opcionales: `roleId`, `branchId`
  - Búsqueda parcial: `search` (nombre, apellido, correo, teléfono)
- `GET /api/users/{id}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

### Roles

- `GET /api/roles`
- `GET /api/roles/{id}`
- `POST /api/roles`
- `PUT /api/roles/{id}`
- `DELETE /api/roles/{id}`

### Branches

- `GET /api/branches`
- `GET /api/branches/{id}`
- `POST /api/branches`
- `PUT /api/branches/{id}`
- `DELETE /api/branches/{id}`

## Ejemplos de request

### Crear role

```json
{
  "name": "Administrador",
  "description": "Acceso completo"
}
```

### Crear branch

```json
{
  "name": "Sucursal Centro",
  "street": "Av. Principal",
  "exteriorNumber": "100",
  "interiorNumber": "2B",
  "neighborhood": "Centro",
  "city": "Ciudad de México",
  "state": "CDMX",
  "postalCode": "06000",
  "country": "México"
}
```

### Crear user

```json
{
  "firstName": "Juan",
  "lastName": "Pérez",
  "secondLastName": "López",
  "email": "juan.perez@example.com",
  "phone": "5551234567",
  "isActive": true,
  "roleId": 1,
  "branchId": 1
}
```

## Modelo de datos (resumen)

| Entidad | Campos clave |
|---|---|
| `Role` | `Id`, `Name` (único), `Description` |
| `Branch` | `Id`, `Name` (único), dirección (`Street`, `ExteriorNumber`, `City`, `State`, `PostalCode`, `Country`) |
| `User` | `Id`, `FirstName`, `LastName`, `SecondLastName`, `Email` (único), `Phone`, `IsActive`, `RoleId`, `BranchId` |

Relaciones:

- `User` -> `Role` (muchos a uno)
- `User` -> `Branch` (muchos a uno)

## Validaciones y errores

- Respuestas consistentes para errores con:
  - `message`
  - `code`
  - `errors` (cuando aplica por validación de modelo)
- Casos cubiertos:
  - recurso no encontrado (`404`)
  - datos inválidos (`400`)
  - duplicados de `Email`, `Role.Name`, `Branch.Name` (`400`)
  - `RoleId` / `BranchId` inexistente al crear/editar usuario (`400`)

## Artefactos de entrega incluidos

- Backup SQL: `backup_api_usuarios.sql`
- Colección Postman: `apiUsuarios.postman_collection.json`
- Docker: `Dockerfile`, `docker-compose.yml`, `.dockerignore`
- Seed automático Docker: `docker/mysql-init/01-init.sql`
- Diagrama: `DiagramaER.pdf`
