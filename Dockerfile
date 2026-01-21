# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy solution and project files for dependency caching
COPY TourManagement.sln ./
COPY src/TourManagement.Web/TourManagement.Web.csproj ./src/TourManagement.Web/
COPY src/TourManagement.Application/TourManagement.Application.csproj ./src/TourManagement.Application/
COPY src/TourManagement.Domain/TourManagement.Domain.csproj ./src/TourManagement.Domain/
COPY src/TourManagement.Infrastructure/TourManagement.Infrastructure.csproj ./src/TourManagement.Infrastructure/
COPY tests/TourManagement.UnitTests/TourManagement.UnitTests.csproj ./tests/TourManagement.UnitTests/
COPY tests/TourManagement.IntegrationTests/TourManagement.IntegrationTests.csproj ./tests/TourManagement.IntegrationTests/
COPY Tests/Tests.csproj ./Tests/

# Restore dependencies
RUN dotnet restore

# Copy entire source code
COPY . .

# Build the application
WORKDIR /src/src/TourManagement.Web
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application from builder stage
COPY --from=builder /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Configure ASP.NET Core environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

# Expose application port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "TourManagement.Web.dll"]