# PerFitty Backend

ASP.NET Core API using Clean Architecture.

## Projects

- `Hnanut.PerFitty.Api`: HTTP host, middleware, controllers, OpenAPI.
- `Hnanut.PerFitty.Application`: use-case abstractions and application services.
- `Hnanut.PerFitty.Domain`: domain modules and business rules.
- `Hnanut.PerFitty.Infrastructure`: EF Core, SQL Server, external integrations.
- `Hnanut.PerFitty.SharedKernel`: shared response, paging, and base domain types.

## Commands

```powershell
Copy-Item .\.env.example .\.env
docker compose up -d
dotnet restore .\Hnanut.PerFitty.slnx
dotnet build .\Hnanut.PerFitty.slnx
dotnet run --project .\src\Hnanut.PerFitty.Api
```

MinIO console runs on `http://localhost:9001`.

## Endpoints

- `GET /health`
- `GET /api/health`
- `GET /api/system/version`
- `GET /swagger`
