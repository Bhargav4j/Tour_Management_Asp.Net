# Dockerfile for .NET Framework 4.7.2 ASP.NET Web Forms Application
# Build Stage
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS builder

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR /src

# Install NuGet
RUN Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile C:\nuget.exe

# Copy project files and restore dependencies
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/packages.config ./packages.config
RUN C:\nuget.exe restore packages.config -PackagesDirectory ./packages

# Copy solution and project files
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management.sln ./
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/*.csproj ./Tour_Management/
RUN C:\nuget.exe restore Tour_Management.sln

# Copy remaining source code
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/ ./Tour_Management/

# Build the application
WORKDIR /src/Tour_Management
RUN msbuild Tour_Management.csproj /p:Configuration=Release /p:OutputPath=C:\inetpub\wwwroot /p:DeployOnBuild=true /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=True

# Runtime Stage
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Set environment variables
ENV ASPNET_ENV=Production

# Create application directory
WORKDIR /inetpub/wwwroot

# Copy published application from builder
COPY --from=builder /inetpub/wwwroot ./

# Create directories for application data
RUN New-Item -ItemType Directory -Force -Path C:\inetpub\wwwroot\pics; \
    New-Item -ItemType Directory -Force -Path C:\inetpub\wwwroot\Tour_pics; \
    New-Item -ItemType Directory -Force -Path C:\tmp\charts

# Configure IIS
RUN Import-Module WebAdministration; \
    Remove-Website -Name 'Default Web Site'; \
    New-Website -Name 'TourManagementApp' -Port 80 -PhysicalPath 'C:\inetpub\wwwroot' -ApplicationPool 'DefaultAppPool'; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool -Name processModel.identityType -Value 2

# Health check using native IIS endpoint
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD powershell -Command "try { $response = Invoke-WebRequest -Uri http://localhost/health.aspx -UseBasicParsing -TimeoutSec 5; if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }"

EXPOSE 80

# Start IIS
ENTRYPOINT ["powershell", "-Command", "Start-Service W3SVC; while ($true) { Start-Sleep -Seconds 3600 }"]
