# Covenant.Api — .NET 8 Backend

## Code Navigation

```
Controllers:     Covenant.Api/Controllers/Sigook/                       (root: Catalog, File, Location)
                 Covenant.Api/Controllers/Sigook/Agency/                (Agency, AgencyLocation)
                 Covenant.Api/Controllers/Sigook/Agency/Accounting/     (Invoices, PayStubs, Reports)
Module controllers: Covenant.Api/{Module}Module/                        (AccountingModule, AgencyModule, CompanyModule, WorkerModule, ManagerModule)
Services:        Covenant.Core.BL/Services/                             (PayStubService, RequestService, WorkerService, etc.)
                 Covenant.Core.BL/Services/Invoices/                    (CanadaInvoiceService, UsaInvoiceService)
Entities:        Covenant.Common/Entities/{Domain}/                     (Accounting/, Agency/, Company/, Request/, Worker/, Candidate/)
Models/DTOs:     Covenant.Common/Models/{Domain}/                       (mirrors Entities structure)
Repo interfaces: Covenant.Common/Repositories/{Domain}/
Repo impls:      Covenant.Infrastructure/Repositories/{Domain}/
EF configs:      Covenant.Infrastructure/Configurations/{Domain}/
Migrations:      Covenant.Infrastructure/Migrations/
DI registration: Covenant.Api/Configuration/ApiServicesConfiguration.cs
Tests:           Covenant.Tests/
```

## Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Entity | `{Name}.cs` | `PayStub.cs`, `Invoice.cs` |
| Child entity | `{Parent}{Child}.cs` | `PayStubItem.cs`, `InvoiceDiscount.cs` |
| Service interface | `I{Name}Service.cs` | `IPayStubService.cs` |
| Service impl | `{Name}Service.cs` | `PayStubService.cs` |
| Repository interface | `I{Name}Repository.cs` | `IPayStubRepository.cs` |
| Repository impl | `{Name}Repository.cs` | `PayStubRepository.cs` |
| EF configuration | `{Entity}Configuration.cs` | `PayStubHistoryConfiguration.cs` |
| Create model | `Create{Name}Model.cs` | `CreatePayStubModel.cs` |
| Detail model | `{Name}DetailModel.cs` | `PayStubDetailModel.cs` |
| List model | `{Name}ListModel.cs` | `InvoiceListModel.cs` |
| Filter model | `Get{Name}Filter.cs` | `GetPayStubsFilter.cs` |
| Versioned controller | `{Module}{Resource}V{N}Controller.cs` | `AccountingPayStubV4Controller.cs` |

## Patterns

- All services/repos registered as `AddScoped<>` in `ApiServicesConfiguration.cs`
- Repository pattern with interfaces in `Covenant.Common`, implementations in `Covenant.Infrastructure`
- Services in `Covenant.Core.BL` depend only on repository interfaces
- EF Core configurations in `Covenant.Infrastructure/Configurations/`

## Commands

```bash
# Build
dotnet build Covenant.Api/Covenant.Api.csproj

# Run tests
dotnet test Covenant.Tests/Covenant.Tests.csproj

# Add migration
dotnet ef migrations add MigrationName -p Covenant.Infrastructure -s Covenant.Api
```
