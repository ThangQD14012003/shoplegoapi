# ShopLego Clean Architecture API

## Projects

- `ShopLego.Domain`: entities and domain constants; no outer-layer dependencies.
- `ShopLego.Application`: use-case contracts, request/response models and application errors.
- `ShopLego.Infrastructure`: EF Core PostgreSQL persistence, JWT and service implementations.
- `ShopLego.Api`: controllers, authentication, OpenAPI, health checks and error middleware.
- `ShopLego.UnitTests`: dependency-rule tests.

The EF mapping targets the existing Neon schema documented in `../DB_SCHEMA.md`; it does not recreate or migrate the database.

## Configuration

Set secrets through environment variables or a local user-secrets store:

```text
ConnectionStrings__MyDB
Jwt__Key
Jwt__Issuer=ShopLegoApi
Jwt__Audience=ShopLegoClient
```

## Run

```powershell
dotnet restore ShopLego.Clean.slnx
dotnet test ShopLego.Clean.slnx
dotnet run --project src/ShopLego.Api
```

Open `/swagger` for the API explorer or `/health` for a process health check.

## Security changes

- Product mutations and all admin routes require the `Admin` role.
- Cart and order routes enforce ownership from the JWT subject instead of trusting a request user id.
- Registration always creates a normal `User`; callers cannot self-assign the admin role.
- Refresh tokens are marked and validated as refresh tokens.
- Secrets are intentionally absent from committed settings.
