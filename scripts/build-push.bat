@echo off
setlocal enabledelayedexpansion

echo ============================================
echo TourManagement Docker Build and Push Script
echo ============================================
echo.

set PROJECT_NAME=TourManagement

REM Sanitize image name using PowerShell
for /f "delims=" %%i in ('powershell -Command "'%PROJECT_NAME%'.ToLower() -replace '[^a-z0-9]+','-' -replace '^-+','' -replace '-+$',''"') do set IMAGE_NAME=%%i

echo Project: %PROJECT_NAME%
echo Image name: %IMAGE_NAME%
echo.

echo Select container registry:
echo 1. AWS ECR (Elastic Container Registry)
echo 2. Docker Hub
set /p REGISTRY_CHOICE="Enter your choice (1 or 2): "
echo.

if "!REGISTRY_CHOICE!"=="1" (
    echo === AWS ECR Configuration ===
    set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
    set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
    set /p ECR_REPO="Enter ECR repository name (default: !IMAGE_NAME!): "
    if "!ECR_REPO!"=="" set ECR_REPO=!IMAGE_NAME!
    
    set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
    set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!
    
    echo.
    echo Authenticating with AWS ECR...
    aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: ECR authentication failed. Please check your AWS credentials and region.
        exit /b 1
    )
    
    echo ECR authentication successful!
    echo.
    
    echo Checking if ECR repository exists...
    aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Repository does not exist. Creating ECR repository: !ECR_REPO!
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
        echo ECR repository created successfully!
    )
    echo.
    
) else if "!REGISTRY_CHOICE!"=="2" (
    echo === Docker Hub Configuration ===
    set /p DOCKER_USERNAME="Enter Docker Hub username: "
    set /p DOCKER_PASSWORD="Enter Docker Hub password or access token: "
    set /p DOCKER_REPO="Enter repository name (default: !IMAGE_NAME!): "
    if "!DOCKER_REPO!"=="" set DOCKER_REPO=!IMAGE_NAME!
    
    set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!DOCKER_REPO!
    
    echo.
    echo Authenticating with Docker Hub...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Docker Hub authentication failed. Please check your credentials.
        exit /b 1
    )
    
    echo Docker Hub authentication successful!
    echo.
) else (
    echo ERROR: Invalid choice. Please run the script again and select 1 or 2.
    exit /b 1
)

set /p IMAGE_TAG="Enter image tag (default: latest): "
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest

REM Sanitize tag using PowerShell
for /f "delims=" %%i in ('powershell -Command "'!IMAGE_TAG!'.ToLower() -replace '[^a-z0-9.-]+','-' -replace '^-+','' -replace '-+$',''"') do set IMAGE_TAG=%%i
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest

set FULL_IMAGE_TAG=!FULL_IMAGE_NAME!:!IMAGE_TAG!

echo.
echo Building Docker image...
echo Image: !FULL_IMAGE_TAG!
echo.

docker build -f Dockerfile -t !FULL_IMAGE_TAG! .

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)

echo.
echo Docker build completed successfully!
echo.

echo Pushing image to registry...
docker push !FULL_IMAGE_TAG!

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ============================================
echo Build and push completed successfully!
echo Image: !FULL_IMAGE_TAG!
echo ============================================
echo.
echo Next steps:
echo 1. Update your ECS task definition with this image URI
echo 2. Run the deploy-image.bat script to deploy to ECS
echo.

endlocal