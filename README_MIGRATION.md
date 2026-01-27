# Tour Management System - .NET 8

This is a modern Tour Management System built with ASP.NET Core 8.0, migrated from ASP.NET Web Forms.

## Architecture

The application follows Clean Architecture principles with four main layers:

- **Domain Layer**: Core business entities, interfaces, and domain exceptions
- **Application Layer**: Business logic implementation, services, and DTOs
- **Infrastructure Layer**: Data access, repositories, and EF Core configurations
- **Web Layer**: Razor Pages UI with proper separation of concerns

## Technology Stack

- .NET 8.0
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0
- SQL Server (LocalDB for development)
- Serilog for logging
- Bootstrap 5 for UI

## Project Structure

```
Tour2701Cmp/
├── src/
│   ├── TourManagement.Domain/        # Domain entities and interfaces
│   ├── TourManagement.Application/   # Business logic and services
│   ├── TourManagement.Infrastructure/ # Data access and repositories
│   └── TourManagement.Web/           # Web UI (Razor Pages)
├── docs/                             # Documentation
├── TourManagement.sln                # Solution file
└── README.md
```

## Features

### Tours Management
- View all tours
- Create new tour packages
- Edit existing tours
- Delete tours
- Upload tour images

### User Management
- User registration with validation
- Secure login with password hashing (SHA256)
- User profile management
- Session-based authentication

### Bookings Management
- Book tours
- View all bookings (admin)
- View user's own bookings
- Cancel bookings

## Database Schema

### Tour Table
- TourId (Primary Key)
- TourName, Place, Days, Price
- Locations, TourInfo
- PicFileName (image)
- Audit fields (CreatedDate, ModifiedDate, IsActive, etc.)

### UserInfo Table
- Email (Primary Key)
- FirstName, LastName, Gender
- Password (hashed)
- DateOfBirth
- Address (Street, City, State)
- Audit fields

### Booking Table
- BookingId (Primary Key)
- TourId (Foreign Key)
- Email (Foreign Key)
- TourName, Place, FirstName
- BookingDate
- Audit fields

## Setup Instructions

1. **Prerequisites**
   - .NET 8 SDK installed
   - SQL Server or LocalDB installed

2. **Database Setup**
   - Update connection string in appsettings.json
   - Run migrations: dotnet ef database update --project src/TourManagement.Web
   - Or use the Database.txt file to create tables manually

3. **Build and Run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project src/TourManagement.Web
   ```

4. **Access Application**
   - Navigate to https://localhost:5001

## Migration Notes

This application was migrated from ASP.NET Web Forms 4.7.2 to ASP.NET Core 8.0.

### Key Changes

1. **Architecture**: Moved from code-behind pattern to Clean Architecture
2. **Data Access**: Replaced ADO.NET with Entity Framework Core
3. **Configuration**: Migrated Web.config to appsettings.json
4. **Authentication**: Replaced Forms Authentication with session-based auth
5. **UI**: Converted aspx pages to Razor Pages
6. **Security**: Implemented password hashing
7. **Async**: All I/O operations are now async
8. **Logging**: Replaced Response.Write with proper logging using Serilog

### Breaking Changes Fixed

- System.Web dependencies removed
- ConfigurationManager replaced with IConfiguration
- Server.MapPath replaced with IWebHostEnvironment
- Response.Write replaced with logging and TempData
- SQL injection vulnerabilities fixed with EF Core
- FileUpload control replaced with IFormFile
- ViewState and Session replaced with modern state management
- No hardcoded credentials

## Security Improvements

- Password hashing with SHA256
- SQL injection protection via EF Core
- CSRF protection enabled by default in Razor Pages
- HTTPS enforcement
- Input validation on all forms
- Parameterized database queries
- Secure file upload handling

## Build Verification

Build Status: SUCCESS
- All projects compile without errors
- 1 warning fixed
- .NET 8 compatibility verified
- No deprecated API usage

## License

Internal use only.
