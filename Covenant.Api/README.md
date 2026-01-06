# Covenant API

A comprehensive .NET 6.0 web API for staffing and recruitment management, handling agencies, companies, workers, accounting, payroll, and Canadian tax calculations.

## Overview

The Covenant API (also known as "Sigook") is a modular enterprise-level application that manages the complete lifecycle of staffing operations including:

- **Agency Management** - Personnel, candidates, requests, and worker profiles
- **Company Management** - Company profiles, locations, job positions, and requests
- **Worker Management** - Worker profiles, applications, timesheets, and history
- **Accounting & Payroll** - Invoices, pay stubs, deductions, and complex Canadian tax calculations
- **Document Generation** - Excel reports, PDF invoices, and pay stubs

## Architecture

The solution consists of 15+ projects following domain-driven design:

### Core Projects
- **Covenant.Api** - Main web API with modular controllers
- **Covenant.Common** - Shared entities, models, interfaces, enums
- **Covenant.Infrastructure** - Data access, repositories, EF Core context
- **Covenant.Core.BL** - Business logic services and adapters

### Business Domain Projects
- **Covenant.Billing** - Invoice creation and billing logic
- **Covenant.PayStubs** - Payroll and pay stub generation
- **Covenant.Deductions** - Canadian tax calculations and deductions
- **Covenant.TimeSheetTotal** - Timesheet and overtime calculations
- **Covenant.Documents** - Document generation and reports
- **Covenant.Notifications** - Email and Teams notifications

## Prerequisites

- **.NET 6.0 SDK** (version 6.0.400 or later)
- **PostgreSQL** database server
- **Azure Storage Account** (for file storage)
- **Azure Service Bus** (for messaging)
- **Docker** (optional, for containerized deployment)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Covenant.Api
```

### 2. Set Up Environment Variables

#### Required Environment Variables

Create environment variables or use user secrets for:

```bash
# Database Connection
ConnectionStrings__DefaultConnection="Host=localhost;Database=covenant_db;Username=your_user;Password=your_password"

# Azure Storage (required for file operations)
CUSTOMCONNSTR_AccountingStorageConnection="DefaultEndpointsProtocol=https;AccountName=your_account;AccountKey=your_key;EndpointSuffix=core.windows.net"
CUSTOMCONNSTR_FileStorageConnection="DefaultEndpointsProtocol=https;AccountName=your_account;AccountKey=your_key;EndpointSuffix=core.windows.net"

# Azure Service Bus (required for background processing)
CUSTOMCONNSTR_ServiceBusConnection="Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your_key"
```

### 3. Database Configuration

**Note: Database setup is not required for development.** The project uses a shared cloud-based PostgreSQL database that is already configured and available for all developers.

The database connection is automatically configured through environment variables. No manual database setup, migrations, or Entity Framework CLI tools installation is needed for development work.

#### Optional: Database Management (For Advanced Use Cases)

If you need to work with database migrations or schema changes:

```bash
# Install Entity Framework CLI Tool (only if needed for migrations)
dotnet tool install --global dotnet-ef

# Add new migration (only when modifying entity models)
dotnet ef migrations add MigrationName --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api
```

### 4. Build the Solution

```bash
# Build all projects from the solution root
dotnet build Covenant.Api/Covenant.Api.sln
```

### 5. Run the Application

#### Development Mode

```bash
# Option 1: Standard execution
# Runs the application once - requires manual restart to see changes
dotnet run --project Covenant.Api/Covenant.Api

# Option 2: Watch mode (Recommended for development)
# Automatically restarts the application when it detects file changes
dotnet watch run --project Covenant.Api/Covenant.Api

# Option 3: Debug mode
# Runs the application in debug configuration with detailed logging
dotnet run --project Covenant.Api/Covenant.Api --configuration Debug

# Option 4: Debug + Watch mode (Best for active debugging)
# Combines debug configuration with auto-restart on file changes
dotnet watch run --project Covenant.Api/Covenant.Api --configuration Debug
```

**Which one to use?**
- **`dotnet run`**: For normal execution in production or when you don't need frequent restarts
- **`dotnet watch run`**: For active development - detects changes in `.cs`, `.json`, `.razor` files and automatically restarts
- **`dotnet run --configuration Debug`**: For debugging with enhanced logging and debug symbols
- **`dotnet watch run --configuration Debug`**: For active debugging - combines auto-restart with debug logging (Recommended for debugging)

The API will be available at:
- **HTTPS**: https://localhost:44307
- **HTTP**: http://localhost:5000
- **Swagger UI**: https://localhost:44307 (in Development environment)


## Docker Deployment

### Build Docker Image

```bash
# From the solution root directory
docker build -f Covenant.Api/Dockerfile -t covenant-api .
```

### Run with Docker

```bash
# Run the container with environment variables
docker run -d \
  --name covenant-api \
  -p 80:80 \
  -e ConnectionStrings__DefaultConnection="your_postgresql_connection" \
  -e CUSTOMCONNSTR_FileStorageConnection="your_azure_storage_connection" \
  -e CUSTOMCONNSTR_ServiceBusConnection="your_service_bus_connection" \
  covenant-api
```


## Development

### Running Tests

```bash
# Run unit tests
dotnet test **/Covenant.Tests.csproj

# Run integration tests
dotnet test **/Covenant.Integration.Tests.csproj

# Run all tests
dotnet test
```

### Database Migrations

```bash
# Add a new migration (from repository root)
dotnet ef migrations add YourMigrationName --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api

# Update database
dotnet ef database update --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api

# Generate SQL script
dotnet ef migrations script --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api
```

### Code Structure

The API follows a modular structure with controllers organized by business domain:

```
Covenant.Api/
├── AccountingModule/     # Invoice, PayStub, Deduction controllers
├── AgencyModule/         # Agency, Candidate, Request controllers
├── CompanyModule/        # Company, Profile, Request controllers
├── WorkerModule/         # Worker, Profile, Request controllers
├── ManagerModule/        # Administrative controllers
└── Security/             # Authentication and user management
```

## Configuration

### Key Configuration Sections

The application uses several configuration sections in `appsettings.json`:

- **Rates** - Tax rates, overtime, holiday pay calculations
- **TimeLimits** - Maximum work hours per day/week
- **GeocodeGoogleConfiguration** - Google Maps API integration
- **TeamsWebhookConfiguration** - Microsoft Teams notifications
- **SendGridConfiguration** - Email service settings

### Health Checks & API Documentation

- **Health Checks**: Comprehensive monitoring endpoints - see [Health Checks Documentation](docs/health-checks.md)
- **Swagger UI**: Available at https://localhost:44307 (Development mode only)

## Features

### Role-Based Data Access Architecture
- Single database with role-based data segregation
- Agency and Company level data filtering via authorization filters
- Role-based authorization ensuring users only access their organization's data

### Key Capabilities
- **Payroll Processing**: Canadian tax calculations, CPP/EI deductions, overtime/holiday pay
- **Document Generation**: Excel reports, PDF invoices and pay stubs, automated email delivery
- **Background Processing**: Azure Service Bus integration, email/Teams notifications
- **File Management**: Azure Blob Storage, multiple document format support

## Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Ensure PostgreSQL is running
   - Verify connection string format
   - Check firewall settings

2. **Azure Storage Issues**
   - Verify storage account access keys
   - Ensure proper connection string format
   - Check storage account permissions

3. **Migration Issues**
   - Ensure EF Core tools are installed: `dotnet tool install --global dotnet-ef`
   - Run migrations from the correct directory
   - Check that the startup project is specified correctly

4. **Docker Issues**
   - Ensure all required environment variables are set
   - Check that external services (database, storage) are accessible from container

### Debugging Setup

The repository includes pre-configured debugging settings in `.vscode/`:

#### For Visual Studio Code (Official Microsoft version):
1. **F5** or **Run > Start Debugging** - Launches with debugger attached
2. **Ctrl+F5** or **Run > Start Without Debugging** - Runs without debugger
3. **Debug Panel** - Use "Launch Covenant API" configuration

#### For Cursor or other VS Code forks:
**Note**: .NET debugging only works in official Microsoft VS Code due to licensing restrictions.

**Alternative debugging approaches:**
1. **Console logging**:
   ```csharp
   Console.WriteLine($"Debug: {variableName}");
   ```

2. **Logger framework**:
   ```csharp
   _logger.LogInformation("Debug info: {Value}", myVariable);
   ```

3. **Use official VS Code** for debugging sessions, Cursor for regular development

4. **Visual Studio Community** (free) for full debugging experience

#### Available Debug Configurations (VS Code only):
- **Launch Covenant API** - Standard debug launch  
- **Launch Covenant API (Simple)** - Simplified debug configuration
- **Attach to dotnet process** - Attach debugger to running process

### Development Tips

- Run `dotnet restore` if you encounter build issues
- Check Azure portal for storage and service bus connection strings
- Monitor application logs for detailed error information
- Use **F5** instead of `dotnet run` commands when debugging

## Contributing

1. Follow the existing code structure and patterns
2. Run tests before submitting changes: `dotnet test`
3. Add database migrations for entity changes
4. Update documentation for new features
5. Use the existing service/repository patterns for new functionality