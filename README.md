# Tour Management System - .NET 8

A modern tour booking and management system migrated from ASP.NET Web Forms to .NET 8 with clean architecture.

## Architecture

This application follows clean architecture principles with four main layers:

### Domain Layer (TourManagement.Domain)
- **Entities**: Core business entities (Tour, Booking, User)
- **Interfaces**: Repository and service interfaces
- **No Dependencies**: Pure domain logic

### Application Layer (TourManagement.Application)
- **Services**: Business logic implementation
- **Dependencies**: Domain layer, Microsoft.Extensions.Logging

### Infrastructure Layer (TourManagement.Infrastructure)
- **Data**: Entity Framework Core DbContext and configurations
- **Repositories**: Data access implementations
- **Dependencies**: Domain, Application, EF Core 8.0

### Web Layer (TourManagement.Web)
- **Razor Pages**: Modern ASP.NET Core UI
- **Dependencies**: Infrastructure, Application

## Technologies

- **.NET 8.0**: Target framework
- **Entity Framework Core 8.0**: ORM and data access
- **SQL Server**: Database (LocalDB for development)
- **Razor Pages**: Web UI framework
- **Serilog**: Structured logging
- **BCrypt.Net**: Password hashing

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server or LocalDB

### Setup

1. Update connection string in appsettings.json
2. Create database: dotnet ef database update --project src/TourManagement.Infrastructure
3. Run: dotnet run --project src/TourManagement.Web

## Migration from Web Forms

Key changes:
- Web.config → appsettings.json
- ADO.NET → Entity Framework Core
- ASPX pages → Razor Pages
- Plain text passwords → BCrypt hashed
- SQL concatenation → Parameterized queries
