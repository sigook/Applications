# Covenant.IdentityServer — .NET 6 Authentication Server

IdentityServer4-based authentication and authorization server for the Covenant platform. Handles user login, registration, password reset, email confirmation, OAuth2/OpenID Connect flows, and client management.

## Code Navigation

```
Controllers:        Covenant.IdentityServer/Controllers/Account/        (Login, Registration, Password reset, PasswordController = JSON forgot/reset by 6-digit code)
                    Covenant.IdentityServer/Controllers/Clients/        (OAuth client management)
                    Covenant.IdentityServer/Controllers/Consent/        (OAuth consent flow)
                    Covenant.IdentityServer/Controllers/Configuration/  (Runtime configuration)
                    Covenant.IdentityServer/Controllers/Diagnostics/
                    Covenant.IdentityServer/Controllers/Grants/
                    Covenant.IdentityServer/Controllers/Home/
Services:           Covenant.IdentityServer/Services/                   (Interfaces: IClientService, IEmailService, etc.)
                    Covenant.IdentityServer/Services/Impl/              (Implementations)
Security:           Covenant.IdentityServer/Security/                   (RoleConstants, CovenantResourceOwnerPasswordValidator = password grant rules)
Entities:           Covenant.IdentityServer/Entities/                   (CovenantRole, InactiveUser, PasswordResetCode)
Models:             Covenant.IdentityServer/Models/
Data/DbContext:     Covenant.IdentityServer/Data/                       (CovenantContext, MyKeysContext)
Migrations:         Covenant.IdentityServer/Migrations/                 (CovenantContext, generated with dotnet ef)
                    Covenant.IdentityServer/Data/Migrations/IdentityServer/  (IdS4 stores; client data changes are hand-written SQL migrations + copied Designer)
Configuration:      Covenant.IdentityServer/Configuration/              (Microsoft 365 OIDC, Key Vault, EmailSettings — OAuth clients live only in the ConfigurationDb)
Views (Razor):      Covenant.IdentityServer/Views/                      (login/consent pages + Views/Notifications = email templates)
Background jobs:    Covenant.IdentityServer/BackgroundServices/
```

## Gotchas

- **Pinned to .NET 6.** Upgrading to .NET 8 deadlocks (IdentityServer4 + AutoMapper incompatibility). Do not bump the TargetFramework.
- **Dates:** `Program.cs` enables `Npgsql.EnableLegacyTimestampBehavior` (must stay before `CreateBuilder`). Npgsql then hands `timestamptz` back with an unpredictable `DateTime.Kind`, so compare instants with `DateTimeOffset` (see `PasswordResetCode`) rather than `DateTime.UtcNow`/`Now`.
- **Client grant changes are migrations, not UI:** the admin `Client/Edit` page never updates grant types / PKCE / offline access. Follow `Data/Migrations/IdentityServer/ConfigurationDb/20260820000000_AddPasswordGrantForNativeLogin.cs`.
- **No reference to `Covenant.Common`** (neither project nor NuGet). IdentityServer vendors its own copies of shared types (`Entities/CovenantUser.cs`, `Enums/UserType.cs`, etc.) — changes to Covenant.Common do not flow here automatically; keep vendored copies in sync manually. The pipeline still passes a `PatSigookPackages` build-arg that the Dockerfile no longer consumes (leftover).

## Commands

```bash
# Build
dotnet build Covenant.IdentityServer/Covenant.IdentityServer.csproj

# Run
dotnet run --project Covenant.IdentityServer/Covenant.IdentityServer.csproj
```
