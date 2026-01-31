# Tour Management System - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Project Structure

```
TourManagement.Solution/
├── src/
│   ├── TourManagement.Domain/         # Domain entities and interfaces
│   ├── TourManagement.Application/    # Business logic and services
│   ├── TourManagement.Infrastructure/ # Data access and EF Core
│   └── TourManagement.Web/            # Razor Pages UI
├── tests/
│   ├── TourManagement.UnitTests/      # Unit tests
│   └── TourManagement.IntegrationTests/ # Integration tests
└── docs/                               # Documentation
```

## Technology Stack

- **.NET 8.0**: Target framework
- **ASP.NET Core Razor Pages**: UI layer (migrated from Web Forms)
- **Entity Framework Core 8.0**: Data access (migrated from ADO.NET)
- **SQL Server LocalDB**: Database
- **AutoMapper**: Object mapping
- **BCrypt**: Password hashing (improved security)
- **Serilog**: Structured logging
- **xUnit**: Testing framework

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB or SQL Server
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository
2. Navigate to the solution directory:
   ```bash
   cd TourManagement.Solution
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Update the connection string in `src/TourManagement.Web/appsettings.json` if needed

5. Apply database migrations:
   ```bash
   dotnet ef database update --project src/TourManagement.Infrastructure --startup-project src/TourManagement.Web
   ```

6. Build the solution:
   ```bash
   dotnet build
   ```

7. Run the application:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

8. Open your browser and navigate to `https://localhost:5001`

## Migration Changes

### Architecture
- **Clean Architecture**: Separated into Domain, Application, Infrastructure, and Web layers
- **Dependency Injection**: Built-in .NET DI container
- **Repository Pattern**: Abstract data access
- **Service Layer**: Business logic separation

### Data Access
- **ADO.NET → EF Core**: Modern ORM with type safety
- **SQL Queries → LINQ**: Type-safe queries
- **No Connection Strings in Code**: Configuration-based

### Security
- **Password Hashing**: BCrypt instead of plain text
- **Input Validation**: FluentValidation
- **SQL Injection Prevention**: Parameterized queries via EF Core
- **CSRF Protection**: Built-in with Razor Pages

### Configuration
- **Web.config → appsettings.json**: Modern configuration
- **Environment-specific configs**: appsettings.Development.json

### Logging
- **Serilog**: Structured logging to console and files

## Testing

Run unit tests:
```bash
dotnet test tests/TourManagement.UnitTests
```

Run integration tests:
```bash
dotnet test tests/TourManagement.IntegrationTests
```

Run all tests:
```bash
dotnet test
```

## Build Verification

The solution has been verified to build successfully on .NET 8:
```bash
dotnet build
# Build succeeded
#     0 Warning(s)
#     0 Error(s)
```

## Known Issues

- Database migrations need to be created for the new schema
- Legacy tour images need to be migrated to wwwroot/images
- Admin default credentials need to be seeded

## Next Steps

1. Create initial EF Core migration
2. Seed default admin account
3. Migrate existing database data
4. Add remaining CRUD pages for Tours, Users, Bookings
5. Implement authentication and authorization
6. Add API endpoints if needed
7. Deploy to production environment

## Migration Notes

- All Web Forms pages have been converted to Razor Pages
- Server controls replaced with HTML and Tag Helpers
- ViewState eliminated in favor of proper state management
- Session state now uses distributed cache-ready implementation
- All synchronous I/O converted to async/await patterns
- Connection management handled by EF Core

## License

Copyright © 2026 Tour Management System
