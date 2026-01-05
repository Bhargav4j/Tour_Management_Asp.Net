# Tour Management System - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Project Structure

```
TMS20/
├── src/
│   ├── TourManagement.Domain/          # Domain layer (entities, interfaces)
│   ├── TourManagement.Application/     # Application layer (services, DTOs)
│   ├── TourManagement.Infrastructure/  # Infrastructure layer (data access, repositories)
│   └── TourManagement.Web/            # Presentation layer (Razor Pages)
├── tests/                              # Test projects (placeholder)
└── TourManagement.sln                 # Solution file
```

## Technologies Used

- **.NET 8.0** - Target framework
- **ASP.NET Core Razor Pages** - UI framework
- **Entity Framework Core 8.0** - ORM for database access
- **SQL Server** - Database
- **AutoMapper 12.0.1** - Object mapping
- **Serilog 8.0.0** - Logging framework
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **Bootstrap 5.3** - UI framework

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server (LocalDB or full instance)

### Configuration

1. Update the connection string in `src/TourManagement.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Your connection string here"
     }
   }
   ```

### Running the Application

1. Navigate to the project root directory:
   ```bash
   cd /modernize-data/studio-data/TNT1001/APP2730/transformed-code/866/studio-workspace/TMS20
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

5. Open your browser and navigate to `https://localhost:5001` (or the URL displayed in the console)

## Features

- **User Management**: Register, login, and profile management
- **Tour Catalog**: Browse available tours with details
- **Booking System**: Book tours and manage reservations
- **Clean Architecture**: Separation of concerns across layers
- **Secure Authentication**: BCrypt password hashing
- **Logging**: Comprehensive logging with Serilog
- **Responsive Design**: Bootstrap 5 responsive UI

## Migration Notes

### What Changed

- Migrated from Web Forms to Razor Pages
- Replaced System.Web with ASP.NET Core equivalents
- Migrated from ADO.NET to Entity Framework Core
- Implemented proper password hashing with BCrypt
- Added dependency injection throughout
- Implemented clean architecture patterns
- Added comprehensive logging with Serilog
- Migrated configuration from Web.config to appsettings.json

### Security Improvements

- **Password Hashing**: Plain text passwords replaced with BCrypt hashing
- **SQL Injection Prevention**: Parameterized queries via EF Core
- **CSRF Protection**: Built-in ASP.NET Core CSRF protection
- **Secure Session Management**: ASP.NET Core session with secure cookies

## Build Verification

✅ **Build Status**: SUCCESS
- All projects compile without errors
- All dependencies resolved
- Target framework: net8.0

## License

All rights reserved.
