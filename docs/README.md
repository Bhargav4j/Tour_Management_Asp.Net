# Tour Management System - .NET 8 Migration

## Overview
This is a modern .NET 8 web application migrated from ASP.NET Web Forms 4.7.2. The application follows clean architecture principles with complete separation of concerns.

## Architecture
- **Domain Layer**: Core entities and interfaces
- **Application Layer**: Business logic and services
- **Infrastructure Layer**: Data access with EF Core 8.0
- **Web Layer**: Razor Pages UI

## Features
- Tour management (CRUD operations)
- User registration and authentication
- Booking management
- Secure password hashing with BCrypt
- Session-based authentication
- File upload for tour images
- Responsive Bootstrap 5 UI

## Prerequisites
- .NET 8 SDK
- SQL Server or LocalDB

## Setup Instructions

### 1. Update Connection String
Edit `src/TourManagement.Web/appsettings.json` and update the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Your-Connection-String-Here"
}
```

### 2. Create Database
Run Entity Framework migrations to create the database:
```bash
cd src/TourManagement.Web
dotnet ef migrations add InitialCreate --project ../TourManagement.Infrastructure
dotnet ef database update
```

### 3. Build and Run
```bash
dotnet build
cd src/TourManagement.Web
dotnet run
```

The application will be available at `https://localhost:5001`

## Migration Summary
- Migrated from ASP.NET Web Forms to Razor Pages
- Replaced ADO.NET with Entity Framework Core 8.0
- Implemented proper authentication with BCrypt password hashing
- Added async/await patterns throughout
- Implemented repository pattern and dependency injection
- Added comprehensive error handling and logging with Serilog

## Security Improvements
- SQL injection vulnerabilities fixed (using EF Core parameterized queries)
- Password hashing implemented (BCrypt)
- File upload validation added
- HTTPS enforced
- Session-based authentication

## Project Structure
```
src/
├── TourManagement.Domain/          # Entities and interfaces
├── TourManagement.Application/     # Business logic services
├── TourManagement.Infrastructure/  # Data access and repositories
└── TourManagement.Web/            # Razor Pages UI
```

## Database Schema
- **Tour**: Tour packages with details and pricing
- **UserInfo**: User accounts with secure password storage
- **Booking**: Tour bookings linking users and tours
