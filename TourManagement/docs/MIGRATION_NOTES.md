# ASP.NET Web Forms to .NET 8 Migration Notes

## Migration Summary

This document details the migration of the Tour Management application from ASP.NET Web Forms 4.7.2 to .NET 8 with Razor Pages.

## Architecture Changes

### Old Architecture (Web Forms)
- Monolithic Web Forms application
- Code-behind pattern with ViewState
- ADO.NET for data access
- Web.config for configuration
- Forms authentication
- System.Web dependencies

### New Architecture (.NET 8)
- Clean architecture with four layers
- Razor Pages with page models
- Entity Framework Core 8
- appsettings.json for configuration
- Cookie authentication with ASP.NET Core
- Modern dependency injection

## File Mappings

### Pages
- `userlogin.aspx` → `Pages/Account/Login.cshtml`
- `SignUpForm.aspx` → `Pages/Account/Register.cshtml`
- `DisplayTours.aspx` → `Pages/Tours/Index.cshtml`
- `AddTour.aspx` → Admin functionality (future implementation)
- `Order.aspx` → `Pages/Bookings/Create.cshtml`
- `mybooking.aspx` → `Pages/Bookings/Index.cshtml`

### Data Access
- ADO.NET SqlConnection/SqlCommand → Entity Framework Core DbContext
- Inline SQL queries → LINQ queries and repository pattern
- ConfigurationManager → IConfiguration with Options pattern

### Configuration
- `Web.config` → `appsettings.json` and `appsettings.Development.json`
- Connection strings moved to appsettings.json
- App settings moved to appsettings.json

## Critical Changes

### 1. Authentication & Security

**Old (Web Forms)**:
```csharp
string checkPasswordQuery = "select password from Userinfo where password='" + txtPassword.Text + "'";
```

**New (.NET 8)**:
```csharp
var user = await _userService.AuthenticateUserAsync(Email, Password);
// BCrypt password hashing
// Parameterized queries via EF Core
```

### 2. Data Access

**Old (Web Forms)**:
```csharp
SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
conn.Open();
SqlCommand com = new SqlCommand(insertQuery, conn);
com.ExecuteNonQuery();
```

**New (.NET 8)**:
```csharp
await _tourRepository.AddAsync(tour, cancellationToken);
// Using EF Core with async/await
// Proper using statements and disposal
```

### 3. Dependency Injection

**Old (Web Forms)**:
```csharp
// Direct instantiation in code-behind
SqlConnection conn = new SqlConnection(...);
```

**New (.NET 8)**:
```csharp
// Constructor injection
public LoginModel(IUserService userService, ILogger<LoginModel> logger)
{
    _userService = userService;
    _logger = logger;
}
```

### 4. State Management

**Old (Web Forms)**:
- ViewState for page state
- Session for user data
- Response.Write for output

**New (.NET 8)**:
- Model binding with [BindProperty]
- Claims-based authentication
- TempData for temporary data
- Return Page() or RedirectToPage()

## Breaking Changes

1. **No System.Web**: All System.Web references removed
2. **No Server Controls**: Replaced with HTML and Tag Helpers
3. **No Global.asax**: Replaced with Program.cs startup
4. **No ViewState**: Use proper state management
5. **No Page Lifecycle Events**: Use OnGet/OnPost handlers
6. **Async Required**: All I/O operations are async

## Database Schema

The existing database schema is preserved with minor enhancements:
- Added audit fields (CreatedDate, ModifiedDate, IsActive)
- Added proper foreign key relationships
- Using EF Core conventions for mapping

### Tables
- **Tour**: Tour information (mapped from existing schema)
- **UserInfo**: User accounts (mapped from existing schema)
- **Booking**: Tour bookings (new table for booking functionality)

## Security Improvements

1. **SQL Injection**: Fixed by using EF Core parameterized queries
2. **Password Storage**: Implemented BCrypt hashing (no more plain text)
3. **Input Validation**: Data Annotations on all input models
4. **CSRF Protection**: Enabled by default in Razor Pages
5. **Authentication**: Proper cookie-based authentication with claims
6. **Logging**: Structured logging with Serilog

## Performance Improvements

1. **Async/Await**: All data access operations are async
2. **Connection Pooling**: Built into EF Core
3. **Query Optimization**: Using AsNoTracking for read-only queries
4. **Retry Logic**: Implemented for database operations

## Testing Strategy

Future work includes:
- Unit tests for services
- Integration tests for repositories
- End-to-end tests for pages
- Performance testing

## Known Limitations

1. Admin functionality not yet migrated (AddTour, TourCrud, etc.)
2. Chart controls not migrated (consider using Chart.js or similar)
3. File upload for tour images needs enhancement
4. Email notifications not implemented

## Deployment Considerations

1. Update connection strings for production
2. Configure logging sinks appropriately
3. Set up database migrations pipeline
4. Configure authentication cookies (domain, secure, etc.)
5. Enable HTTPS in production
6. Consider using distributed cache for sessions
7. Set up monitoring and health checks

## References

- [Migrate from ASP.NET to ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/migration/proper-to-2x/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/)
