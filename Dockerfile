# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy solution and project files for dependency restoration
COPY ["src/TourManagement.Web/TourManagement.Web.csproj", "src/TourManagement.Web/"]
COPY ["src/TourManagement.Application/TourManagement.Application.csproj", "src/TourManagement.Application/"]
COPY ["src/TourManagement.Domain/TourManagement.Domain.csproj", "src/TourManagement.Domain/"]
COPY ["src/TourManagement.Infrastructure/TourManagement.Infrastructure.csproj", "src/TourManagement.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/TourManagement.Web/TourManagement.Web.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /src/src/TourManagement.Web
RUN dotnet build "TourManagement.Web.csproj" -c Release -o /app/build --no-restore

# Publish the application
RUN dotnet publish "TourManagement.Web.csproj" -c Release -o /app/publish --no-restore --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Create a non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application from build stage
COPY --from=builder /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=UTC

# Change ownership of application files
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose application port
EXPOSE 8080

# Configure graceful shutdown
STOPSIGNAL SIGTERM

# Run the application
ENTRYPOINT ["dotnet", "TourManagement.Web.dll"]