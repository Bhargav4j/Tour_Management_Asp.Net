# Tour Management System - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Architecture

The solution follows clean architecture with four main layers:

- **TourManagement.Domain**: Contains entities, interfaces, and domain logic
- **TourManagement.Application**: Contains business logic services
- **TourManagement.Infrastructure**: Contains data access with Entity Framework Core 8.0
- **TourManagement.Web**: Contains the web UI built with Razor Pages

## Features

- User registration and authentication with BCrypt password hashing
- Tour management (Create, Read, Update, Delete)
- Booking system for tours
- Responsive UI with Bootstrap 5
- Structured logging with Serilog
- Entity Framework Core 8.0 with SQL Server
- Cookie-based authentication
- Clean architecture with dependency injection

## Prerequisites

- .NET 8 SDK
- SQL Server or SQL Server LocalDB

## Setup Instructions

1. Update the connection string in src/TourManagement.Web/appsettings.json
2. Run Entity Framework migrations to create the database
3. Run the application with dotnet run

## Build Verification

✅ Build Status: SUCCESS
- All projects compile without errors
- NuGet packages restored successfully
- .NET 8 compatibility verified

## Migration Completed

This application has been fully migrated from ASP.NET Web Forms to .NET 8.
