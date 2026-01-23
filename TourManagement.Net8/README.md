# Tour Management System - .NET 8

This is a migrated ASP.NET Web Forms application, now running on .NET 8 with clean architecture.

## Architecture

The solution follows clean architecture principles with the following layers:

- **TourManagement.Domain**: Core domain entities, interfaces, and exceptions
- **TourManagement.Application**: Business logic, services, DTOs, and mappings
- **TourManagement.Infrastructure**: Data access, repositories, and EF Core configurations
- **TourManagement.Web**: Razor Pages UI layer
- **TourManagement.UnitTests**: Unit tests for services
- **TourManagement.IntegrationTests**: Integration tests for repositories and pages

## Technologies

- .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0
- SQL Server LocalDB
- AutoMapper
- Serilog
- BCrypt.Net for password hashing
- xUnit, FluentAssertions, and Moq for testing

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB

### Running the Application

1. Navigate to the solution directory:
   ```bash
   cd TourManagement.Net8
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run database migrations (if needed):
   ```bash
   cd src/TourManagement.Web
   dotnet ef database update --project ../TourManagement.Infrastructure
   ```

5. Run the application:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

6. Open your browser and navigate to `https://localhost:5001`

## Features

- User registration and authentication
- Tour browsing and management
- Tour booking system
- Admin panel for tour CRUD operations
- File upload for tour images
- Responsive Bootstrap 5 UI

## Migration from Web Forms

This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8 with the following changes:

### Security Improvements
- Replaced plain text passwords with BCrypt hashing
- Added parameterized queries throughout (EF Core)
- Implemented proper input validation
- Added CSRF protection (built-in with Razor Pages)

### Architecture Improvements
- Separated concerns with clean architecture
- Added dependency injection
- Implemented repository pattern
- Added service layer with interfaces
- Introduced DTOs for data transfer

### Modern Patterns
- Async/await throughout
- Structured logging with Serilog
- AutoMapper for entity-DTO mapping
- Unit and integration tests

### Database
- Migrated from ADO.NET to Entity Framework Core
- Maintained original database schema for compatibility

## Configuration

Update `appsettings.json` to configure:
- Connection strings
- Logging levels
- File upload settings

## Testing

Run unit tests:
```bash
dotnet test tests/TourManagement.UnitTests
```

Run integration tests:
```bash
dotnet test tests/TourManagement.IntegrationTests
```

## License

This project is for educational purposes.
