# Backend Definition of Done

- Code follows Clean Architecture boundaries.
- `Api` only handles HTTP, middleware, and endpoint wiring.
- `Application` owns use-case contracts and orchestration.
- `Domain` owns module rules and business concepts.
- `Infrastructure` owns EF Core, SQL Server, cache, storage, and external adapters.
- Public endpoints return the standard response shape or ProblemDetails.
- New persistence changes include EF Core configuration and migration.
- Endpoint is visible in OpenAPI when public.
- Local restore/build succeeds.
- No template sample code is left behind.
