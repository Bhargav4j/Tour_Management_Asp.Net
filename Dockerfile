# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy solution file
COPY TourManagement.sln ./

# Copy project files for dependency caching
COPY src/TourManagement.Web/TourManagement.Web.csproj src/TourManagement.Web/
COPY src/TourManagement.Application/TourManagement.Application.csproj src/TourManagement.Application/
COPY src/TourManagement.Domain/TourManagement.Domain.csproj src/TourManagement.Domain/
COPY src/TourManagement.Infrastructure/TourManagement.Infrastructure.csproj src/TourManagement.Infrastructure/
COPY Tests/TourManagement.Application.Tests/TourManagement.Application.Tests.csproj Tests/TourManagement.Application.Tests/
COPY Tests/TourManagement.Domain.Tests/TourManagement.Domain.Tests.csproj Tests/TourManagement.Domain.Tests/
COPY Tests/TourManagement.Infrastructure.Tests/TourManagement.Infrastructure.Tests.csproj Tests/TourManagement.Infrastructure.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/src/TourManagement.Web
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish -c Release -o /app/publish --no-build

# Runtime stage with explicit base image
FROM docker.io/library/nginx:latest

WORKDIR /app

# Copy published application
COPY --from=builder /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=UTC

# Expose application port
EXPOSE 8080

# Run the application
CMD ["dotnet", "TourManagement.Web.dll"]