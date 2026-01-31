# Tour Management System - .NET 8

A modernized tour management application migrated from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Overview

This application manages tours, bookings, and users for a tourism business. It has been completely migrated from the legacy ASP.NET Web Forms framework to modern .NET 8 with ASP.NET Core Razor Pages.

## Architecture

The solution follows clean architecture with four main layers:

- **TourManagement.Domain**: Core business entities and repository interfaces
- **TourManagement.Application**: Business logic, services, DTOs, and AutoMapper profiles
- **TourManagement.Infrastructure**: Data access with Entity Framework Core 8 and repository implementations
- **TourManagement.Web**: ASP.NET Core Razor Pages UI layer

## Technologies Used

- .NET 8.0
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0
- SQL Server LocalDB
- AutoMapper 12.0.1
- BCrypt.Net for password hashing
- Serilog for logging
- Bootstrap 5 for UI
- xUnit for testing

## Prerequisites

- .NET 8 SDK
- SQL Server LocalDB (or SQL Server)

## Getting Started

### 1. Restore dependencies

```bash
dotnet restore TourManagement.sln
```

### 2. Update database connection string

Edit `TourManagement.Web/appsettings.json` and update the connection string if needed.

### 3. Run Entity Framework migrations

```bash
cd TourManagement.Web
dotnet ef migrations add InitialCreate --project ../TourManagement.Infrastructure
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run --project TourManagement.Web
```

The application will be available at https://localhost:5001

## Build Verification

✅ Build succeeded - all projects compile without errors
