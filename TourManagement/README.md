# Tour Management System - .NET 8 Migration

This is a fully migrated Tour Management application from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Architecture

The solution follows clean architecture with four main layers:

- **Domain Layer**: Core entities and interfaces
- **Application Layer**: Business logic and services
- **Infrastructure Layer**: Data access with Entity Framework Core 8
- **Web Layer**: ASP.NET Core Razor Pages

## Prerequisites

- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code

## Getting Started

1. **Update Connection String**

   Edit `src/TourManagement.Web/appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your connection string here"
   }
   ```

2. **Run Database Migrations**

   ```bash
   cd src/TourManagement.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../TourManagement.Web
   dotnet ef database update --startup-project ../TourManagement.Web
   ```

3. **Run the Application**

   ```bash
   cd src/TourManagement.Web
   dotnet run
   ```

4. **Access the Application**

   Navigate to `https://localhost:5001` in your browser.

## Key Features

- User registration and authentication with secure password hashing (BCrypt)
- Tour browsing and details
- Booking management
- Clean architecture with dependency injection
- Entity Framework Core 8 with SQL Server
- Serilog logging
- Cookie-based authentication
- Responsive Bootstrap UI

## Migration Changes

### What Was Migrated

- **Pages**: All .aspx pages converted to Razor Pages
- **Data Access**: ADO.NET replaced with Entity Framework Core 8
- **Configuration**: Web.config replaced with appsettings.json
- **Authentication**: Forms authentication replaced with ASP.NET Core Identity cookie authentication
- **Server Controls**: Replaced with HTML helpers and Tag Helpers
- **ViewState**: Removed, using proper state management patterns

### Security Improvements

- SQL injection vulnerabilities fixed (parameterized queries via EF Core)
- Plain text passwords replaced with BCrypt hashing
- CSRF protection enabled by default in Razor Pages
- Input validation using Data Annotations
- Proper error handling and logging

## Project Structure

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/        # Entities and interfaces
│   ├── TourManagement.Application/   # Services and business logic
│   ├── TourManagement.Infrastructure/# Data access and repositories
│   └── TourManagement.Web/           # Razor Pages UI
├── tests/
│   ├── TourManagement.UnitTests/
│   └── TourManagement.IntegrationTests/
└── docs/
```

## Build Verification

The solution has been verified to build successfully with:
- .NET 8.0 SDK
- All projects compile without errors
- All dependencies resolved successfully

## Known Issues

None at this time.

## Future Improvements

- Add unit and integration tests
- Implement API endpoints for mobile apps
- Add admin panel for tour and user management
- Implement advanced search and filtering
- Add payment integration
- Implement email notifications

## License

All rights reserved.
