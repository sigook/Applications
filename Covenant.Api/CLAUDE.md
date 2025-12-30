# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

The Covenant API (also known as "Sigook") is a comprehensive .NET 6.0 staffing/recruitment management system with modular architecture spanning 15+ projects. It manages agencies, companies, workers, accounting, payroll, and complex Canadian tax calculations.

## Architecture

### Project Structure
The solution follows domain-driven design with these key projects:
- **Covenant.Api** - Main web API with modular controllers (Agency/Company/Worker/Accounting modules)
- **Covenant.Common** - Shared entities, enums, interfaces, models
- **Covenant.Infrastructure** - EF Core context, repositories, external service integrations
- **Covenant.Core.BL** - Business logic services and adapters
- **Domain-specific projects** - Billing, PayStubs, Deductions, TimeSheetTotal, Documents, etc.

### Key Patterns
- Repository pattern with interface-based design
- Service layer with dependency injection
- CQRS using MediatR for complex operations
- Event-driven architecture with Azure Service Bus consumers
- Single database with role-based data segregation via Agency/Company authorization filters

## Development Commands

### Build & Run
```bash
# Build entire solution (from repository root)
dotnet build Covenant.Api/Covenant.Api.sln

# Run API (from solution root)
dotnet run --project Covenant.Api/Covenant.Api

# Run with watch mode for development (auto-restart on file changes)
dotnet watch run --project Covenant.Api/Covenant.Api

# Run in debug mode with enhanced logging
dotnet run --project Covenant.Api/Covenant.Api --configuration Debug

# Run in debug mode with watch (auto-restart + debug logging)
dotnet watch run --project Covenant.Api/Covenant.Api --configuration Debug

# Run with specific environment
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Covenant.Api/Covenant.Api
```

### Testing
```bash
# Run unit tests
dotnet test **/Covenant.Tests.csproj

# Run integration tests
dotnet test **/Covenant.Integration.Tests.csproj

# Run all tests
dotnet test
```

### Database Management

**Note: The project uses a shared cloud-based PostgreSQL database. No database setup is required for development.**

For advanced database operations (only when modifying entity models):

```bash
# Add migration (from repository root) - only when changing entity models
dotnet ef migrations add MigrationName --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api

# Generate migration script (for review purposes)
dotnet ef migrations script --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api

# Note: Database updates are handled automatically - manual database update is typically not needed
```

## Configuration Requirements

### Essential Environment Variables
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `CUSTOMCONNSTR_AccountingStorageConnection` - Azure Storage for accounting documents
- `CUSTOMCONNSTR_FileStorageConnection` - Azure Storage for files
- `CUSTOMCONNSTR_ServiceBusConnection` - Azure Service Bus for messaging

### Key Configuration Sections
- **Rates** - Tax rates, overtime, holiday pay calculations
- **TimeLimits** - Maximum work hours per day/week
- **TeamsWebhookConfiguration** - Microsoft Teams notifications
- **SendGridConfiguration** - Email service settings
- **GeocodeGoogleConfiguration** - Google Maps API integration

## Technology Stack

- **.NET 6.0** with ASP.NET Core Web API
- **PostgreSQL** with Entity Framework Core
- **Azure Service Bus** for messaging
- **Azure Storage** for file management
- **AutoMapper** for object mapping
- **FluentValidation** for input validation
- **SendGrid** for email services
- **Microsoft Teams** webhooks for notifications

## Development Notes

### Module Structure
Each business domain (Agency, Company, Worker, Accounting) has its own controller hierarchy within the API project. Controllers follow a consistent pattern with base classes for shared functionality.

### Complex Business Logic
- **Canadian Payroll** - Complex tax calculations with federal/provincial deductions, CPP, EI
- **Time Sheet Processing** - Overtime, night shift, holiday pay calculations
- **Multi-currency Invoicing** - CAD/USD support with different tax rules
- **Document Generation** - Excel reports and PDF generation for invoices/pay stubs

### Background Processing
The system uses Azure Service Bus for async processing of:
- Email notifications
- Teams webhook notifications  
- Invoice processing
- Document generation

### Multi-language Support
Built-in localization for English/Spanish with resource files in `Covenant.Common/Resources/`.

## Testing Strategy

- **Unit Tests** in `Covenant.Tests` - Business logic and utility functions
- **Integration Tests** in `Covenant.Integration.Tests` - Full API endpoints with test database
- **Custom Test Utilities** in `Covenant.Test.Utils` - Shared test infrastructure and fake data generators

## CI/CD Pipeline

Azure DevOps pipeline with:
- Automated testing on pull requests
- Docker image building
- Deployment to staging (`sigook-api-staging`) and production (`sigook-api`) Azure App Services
- NuGet package publishing for `Covenant.Common`

## Code Style and Language Requirements

**IMPORTANT: All code, comments, documentation, and variable names must be written in English.**

This project targets the American market, therefore:
- All code comments must be in English
- All variable names, method names, and class names must be in English
- All documentation and README files must be in English
- All user-facing messages should use English localization resources
- All commit messages must be in English

## Common Development Tasks

When working on this codebase:
1. Always run tests before committing: `dotnet test`
2. Add database migrations when changing entities: `dotnet ef migrations add MigrationName --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api`
3. Use the existing service/repository patterns for new features
4. Follow the modular controller structure within appropriate business domains
5. Update localization resources when adding user-facing messages
6. Write all code and comments in English only