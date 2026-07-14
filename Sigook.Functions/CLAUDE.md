# Sigook.Functions — Azure Functions (.NET 8)

Azure Functions project for background and scheduled jobs in the Covenant platform.

Current functions (both Timer triggers, in `Sigook.Functions/Functions/ScheduleTasks.cs`): `NotificationSinExpiration`, `WarnLicensesExpiration`.
## Code Navigation

```
Functions:          Sigook.Functions/Functions/          (Azure Function triggers)
Services:           Sigook.Functions/Services/           (Business logic)
Models:             Sigook.Functions/Models/
Utils:              Sigook.Functions/Utils/
Entry point:        Sigook.Functions/Program.cs
Configuration:      Sigook.Functions/host.json
```

## Commands

```bash
# Build
dotnet build Sigook.Functions/Sigook.Functions.csproj

# Run locally
func start --project Sigook.Functions/
```
