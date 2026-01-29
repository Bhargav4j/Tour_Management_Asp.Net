@echo off
setlocal enabledelayedexpansion

echo ================================================
echo    Tour Management - Docker Build ^& Push Script
echo ================================================
echo.

set PROJECT_NAME=Tour_Management

REM Sanitize image name using PowerShell
for /f "delims=" %%i in ('powershell -Command "'!PROJECT_NAME!' -replace '[^a-zA-Z0-9]', '-' | ForEach-Object { $_.ToLower().Trim('-') }"') do set IMAGE_NAME=%%i

echo Project: !PROJECT_NAME!
echo Sanitized Image Name: !IMAGE_NAME!
echo.

echo Select Docker Registry:
echo 1. AWS ECR (Elastic Container Registry)
echo 2. Docker Hub
set /p REGISTRY_CHOICE="Enter choice [1-2]: "

if "!REGISTRY_CHOICE!"=="1" (
    echo.
    echo --- AWS ECR Configuration ---
    set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
    set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
    set /p ECR_REPO="Enter ECR Repository Name (e.g., tour-management): "
    
    set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
    
    echo.
    echo Authenticating with AWS ECR...
    aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: ECR authentication failed
        exit /b 1
    )
    
    echo Checking if ECR repository exists...
    aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Creating ECR repository: !ECR_REPO!
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    )
    
    set /p IMAGE_TAG="Enter image tag (default: latest): "
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    for /f "delims=" %%i in ('powershell -Command "'!IMAGE_TAG!' -replace '[^a-zA-Z0-9._-]', '-' | ForEach-Object { $_.ToLower().Trim('-') }"') do set IMAGE_TAG=%%i
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    
    set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!
    
) else if "!REGISTRY_CHOICE!"=="2" (
    echo.
    echo --- Docker Hub Configuration ---
    set /p DOCKER_USERNAME="Enter Docker Hub username: "
    set /p DOCKER_PASSWORD="Enter Docker Hub password or token: "
    
    echo Authenticating with Docker Hub...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Docker Hub authentication failed
        exit /b 1
    )
    
    set /p IMAGE_TAG="Enter image tag (default: latest): "
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    for /f "delims=" %%i in ('powershell -Command "'!IMAGE_TAG!' -replace '[^a-zA-Z0-9._-]', '-' | ForEach-Object { $_.ToLower().Trim('-') }"') do set IMAGE_TAG=%%i
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    
    set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!
) else (
    echo ERROR: Invalid choice
    exit /b 1
)

echo.
echo ================================================
echo Building Docker Image...
echo Image: !FULL_IMAGE_NAME!
echo ================================================
echo.

docker build -f Dockerfile -t "!FULL_IMAGE_NAME!" .

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed
    exit /b 1
)

echo.
echo ================================================
echo Pushing Docker Image...
echo ================================================
echo.

docker push "!FULL_IMAGE_NAME!"

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed
    exit /b 1
)

echo.
echo ================================================
echo SUCCESS!
echo ================================================
echo Image: !FULL_IMAGE_NAME!
echo Tag: !IMAGE_TAG!
echo.
echo Next Steps:
echo 1. Update ecs/task-definition.json with image URI
echo 2. Run .\scripts\deploy-image.bat to deploy to ECS
echo ================================================

endlocal
