# Tour Management System - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms (.NET Framework 4.7.2) to .NET 8 using clean architecture principles.

## Project Structure

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/          # Domain entities and interfaces
│   ├── TourManagement.Application/     # Business logic and services
│   ├── TourManagement.Infrastructure/  # Data access and repositories
│   └── TourManagement.Web/             # Razor Pages UI
├── tests/
│   ├── TourManagement.UnitTests/       # Unit tests
│   └── TourManagement.IntegrationTests/# Integration tests
└── docs/                                # Documentation
```

## Technologies Used

- **.NET 8** - Target framework
- **ASP.NET Core Razor Pages** - UI framework (migrated from Web Forms)
- **Entity Framework Core 8.0** - ORM for data access
- **SQL Server** - Database
- **Serilog** - Logging framework
- **BCrypt.Net** - Password hashing
- **xUnit** - Testing framework
- **Moq** - Mocking framework for tests
- **FluentAssertions** - Assertion library

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

2. **Update database connection string**
   Edit `src/TourManagement.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TourManagementDb;Trusted_Connection=True;"
     }
   }
   ```

3. **Create the database**
   Run the SQL scripts from Database.txt to create tables.

4. **Run the application**
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

5. **Access the application**
   Open your browser and navigate to: `https://localhost:5001`

## Features

- **Tour Management**: Create, view, edit, and delete tours
- **User Registration & Login**: Secure user authentication with BCrypt password hashing
- **Booking System**: Book tours and manage bookings
- **Admin Portal**: Admin login for tour and user management
- **Search Functionality**: Search tours by name, place, or location
- **Image Upload**: Upload tour images with validation
- **Session Management**: User sessions for authentication state
- **Logging**: Comprehensive logging with Serilog

## Default Credentials

### Admin Login
- Username: `admin`
- Password: `admin123`

## Migration Details

### What Was Migrated

- ✅ **Web Forms pages** → Razor Pages
- ✅ **ADO.NET** → Entity Framework Core
- ✅ **Web.config** → appsettings.json
- ✅ **System.Web** → ASP.NET Core equivalents
- ✅ **Plaintext passwords** → BCrypt hashed passwords
- ✅ **SQL injection vulnerabilities** → Parameterized queries (EF Core)
- ✅ **Server controls** → HTML helpers and Tag Helpers
- ✅ **ViewState** → Modern state management (session, TempData)

### Architecture Improvements

- **Clean Architecture**: Separation into Domain, Application, Infrastructure, and Web layers
- **Dependency Injection**: Built-in DI container for all services
- **Async/Await**: All I/O operations are async
- **Error Handling**: Comprehensive try-catch blocks with logging
- **Validation**: Model validation with Data Annotations
- **Security**: BCrypt password hashing, SQL injection prevention
- **Logging**: Structured logging with Serilog
- **Testing**: Unit and integration tests included
