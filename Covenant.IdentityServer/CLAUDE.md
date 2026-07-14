# Covenant.IdentityServer — .NET 6 Authentication Server

IdentityServer4-based authentication and authorization server for the Covenant platform. Handles user login, registration, password reset, email confirmation, OAuth2/OpenID Connect flows, and client management.

## Code Navigation

```
Controllers:        Covenant.IdentityServer/Controllers/Account/        (Login, Registration, Password reset)
                    Covenant.IdentityServer/Controllers/Clients/        (OAuth client management)
                    Covenant.IdentityServer/Controllers/Consent/        (OAuth consent flow)
                    Covenant.IdentityServer/Controllers/Configuration/  (Runtime configuration)
                    Covenant.IdentityServer/Controllers/Diagnostics/
                    Covenant.IdentityServer/Controllers/Grants/
                    Covenant.IdentityServer/Controllers/Home/
Services:           Covenant.IdentityServer/Services/                   (Interfaces: IClientService, IEmailService, etc.)
                    Covenant.IdentityServer/Services/Impl/              (Implementations)
Entities:           Covenant.IdentityServer/Entities/                   (CovenantRole, InactiveUser)
Models:             Covenant.IdentityServer/Models/
Data/DbContext:     Covenant.IdentityServer/Data/                       (CovenantContext, MyKeysContext)
Migrations:         Covenant.IdentityServer/Migrations/
                    Covenant.IdentityServer/Data/Migrations/IdentityServer/
Configuration:      Covenant.IdentityServer/Configuration/              (Environment-specific configs, OAuth clients)
Views (Razor):      Covenant.IdentityServer/Views/
Email templates:    Covenant.IdentityServer/Templates/Email/
Background jobs:    Covenant.IdentityServer/BackgroundServices/
```

## Gotchas

- **Pinned to .NET 6.** Upgrading to .NET 8 deadlocks (IdentityServer4 + AutoMapper incompatibility). Do not bump the TargetFramework.
- **No reference to `Covenant.Common`** (neither project nor NuGet). IdentityServer vendors its own copies of shared types (`Entities/CovenantUser.cs`, `Enums/UserType.cs`, etc.) — changes to Covenant.Common do not flow here automatically; keep vendored copies in sync manually. The pipeline still passes a `PatSigookPackages` build-arg that the Dockerfile no longer consumes (leftover).

## Commands

```bash
# Build
dotnet build Covenant.IdentityServer/Covenant.IdentityServer.csproj

# Run
dotnet run --project Covenant.IdentityServer/Covenant.IdentityServer.csproj
```
