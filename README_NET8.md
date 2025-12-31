# Tour Management System - .NET 8

A modern tour management web application built with ASP.NET Core 8.0 and Razor Pages, following clean architecture principles.

## Architecture

This application follows clean architecture with four main layers:

- **Domain**: Core business entities and interfaces
- **Application**: Business logic and services
- **Infrastructure**: Data access and external integrations
- **Web**: Presentation layer with Razor Pages

## Features

- User authentication and authorization
- Tour browsing and searching
- Tour booking management
- Admin tour management
- Secure password hashing with BCrypt
- Entity Framework Core with SQL Server
- Structured logging with Serilog

## Prerequisites

- .NET 8.0 SDK
- SQL Server or LocalDB
- Visual Studio 2022 or VS Code (optional)

## Getting Started

1. **Update connection string** in `src/TourManagement.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TourManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

2. **Run database migrations**:
   ```bash
   cd src/TourManagement.Web
   dotnet ef database update --project ../TourManagement.Infrastructure
   ```

3. **Run the application**:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

4. **Access the application**:
   Navigate to `https://localhost:5001` in your browser.

## Database Setup

The database schema includes three main tables:
- **UserInfo**: User accounts with hashed passwords
- **Tour**: Tour packages with details and pricing
- **Booking**: Tour bookings linking users to tours

Run the migrations to create the database automatically.

## Project Structure

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/         # Entities and interfaces
│   ├── TourManagement.Application/    # Business logic services
│   ├── TourManagement.Infrastructure/ # Data access and repositories
│   └── TourManagement.Web/            # Razor Pages UI
├── tests/
│   ├── TourManagement.UnitTests/
│   └── TourManagement.IntegrationTests/
└── docs/
```

## Migration Notes

This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8. Key changes:

- Replaced Web Forms pages with Razor Pages
- Migrated from ADO.NET to Entity Framework Core
- Implemented proper authentication with cookie-based auth
- Added password hashing (BCrypt) for security
- Implemented clean architecture with dependency injection
- Added structured logging with Serilog
- Replaced Web.config with appsettings.json

## Security Improvements

- Passwords are hashed using BCrypt
- SQL injection protection via EF Core parameterized queries
- CSRF protection enabled by default in Razor Pages
- Authentication and authorization implemented
- Input validation on all forms

## Build Verification

✅ Build Status: **SUCCESS**
- All projects compile without errors
- NuGet packages restored successfully
- Entity Framework Core 8.0.0 configured
- .NET 8 compatible packages used

## License

Proprietary - All rights reserved
