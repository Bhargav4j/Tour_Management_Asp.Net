# Tour Management System - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Architecture

The solution follows clean architecture with four main layers:

- **Domain**: Core entities, interfaces, and business rules
- **Application**: Business logic, services, and DTOs
- **Infrastructure**: Data access, repositories, and external services
- **Web**: ASP.NET Core Razor Pages UI layer

## Projects

- `TourManagement.Domain`: Domain entities and interfaces
- `TourManagement.Application`: Business logic and services
- `TourManagement.Infrastructure`: Data access with EF Core
- `TourManagement.Web`: Razor Pages web application
- `TourManagement.UnitTests`: Unit tests
- `TourManagement.IntegrationTests`: Integration tests

## Technology Stack

- .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0
- SQL Server (LocalDB)
- Serilog for logging
- AutoMapper for object mapping
- xUnit for testing

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB or SQL Server

### Setup

1. Clone the repository
2. Update connection string in `appsettings.json` if needed
3. Run migrations:
   ```bash
   dotnet ef database update --project src/TourManagement.Infrastructure --startup-project src/TourManagement.Web
   ```
4. Run the application:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

### Default Admin Credentials

- Email: admin@gmail.com
- Password: admin

## Database

The application uses Entity Framework Core with SQL Server. The database schema includes:

- Tour: Tour packages
- UserInfo: User accounts
- Booking: Tour bookings

## Features

- Browse available tours
- User registration and authentication
- Admin authentication
- Tour management (CRUD operations for admins)
- Tour booking system
- Responsive design with Bootstrap 5

## Migration Notes

This application was migrated from ASP.NET Web Forms to .NET 8. Key changes:

- Replaced Web Forms pages with Razor Pages
- Migrated from ADO.NET to Entity Framework Core
- Implemented clean architecture
- Added dependency injection
- Replaced session-based authentication with modern session management
- Fixed SQL injection vulnerabilities
- Added proper error handling and logging
- Implemented async/await patterns

## Build Status

Build succeeded with 0 errors and 0 warnings.
