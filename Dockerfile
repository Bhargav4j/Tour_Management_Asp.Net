# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy solution and project files for dependency restoration
COPY TourManagement.sln ./
COPY src/TourManagement.Web/TourManagement.Web.csproj ./src/TourManagement.Web/
COPY src/TourManagement.Application/TourManagement.Application.csproj ./src/TourManagement.Application/
COPY src/TourManagement.Domain/TourManagement.Domain.csproj ./src/TourManagement.Domain/
COPY src/TourManagement.Infrastructure/TourManagement.Infrastructure.csproj ./src/TourManagement.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY src/ ./src/

# Build the application
WORKDIR /src/src/TourManagement.Web
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish -c Release --no-build -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application from builder stage
COPY --from=builder /app/publish .

# Create logs directory with proper permissions
RUN mkdir -p /app/logs && chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=UTC

# Expose application port
EXPOSE 8080

# Configure graceful shutdown
ENTRYPOINT ["dotnet", "TourManagement.Web.dll"]