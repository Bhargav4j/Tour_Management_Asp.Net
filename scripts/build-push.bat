@echo off
setlocal enabledelayedexpansion

echo ==========================================
echo Docker Build and Push Script
echo ==========================================
echo.

set PROJECT_NAME=TMSContainerize

for /f "delims=" %%a in ('powershell -Command "'!PROJECT_NAME!' -replace '[^a-zA-Z0-9]','-' -replace '^-+|-+$','' | ForEach-Object { $_.ToLower() }"') do set IMAGE_NAME=%%a

echo Project: !PROJECT_NAME!
echo Image name: !IMAGE_NAME!
echo.

echo Select registry type:
echo 1. AWS ECR (Elastic Container Registry)
echo 2. Docker Hub
set /p REGISTRY_CHOICE="Enter choice (1 or 2): "

if "!REGISTRY_CHOICE!"=="1" (
    echo.
    echo === AWS ECR Configuration ===
    set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
    set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
    set /p ECR_REPO="Enter ECR Repository Name (default: !IMAGE_NAME!): "
    if "!ECR_REPO!"==" " set ECR_REPO=!IMAGE_NAME!
    
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
        echo Repository does not exist. Creating ECR repository...
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
        if !ERRORLEVEL! neq 0 (
            echo ERROR: Failed to create ECR repository
            exit /b 1
        )
    )
    
    set /p IMAGE_TAG="Enter image tag (default: latest): "
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    for /f "delims=" %%a in ('powershell -Command "'!IMAGE_TAG!' -replace '[^a-zA-Z0-9.-]','-' -replace '^-+|-+$','' | ForEach-Object { $_.ToLower() }"') do set IMAGE_TAG=%%a
    
    set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!
    
) else if "!REGISTRY_CHOICE!"=="2" (
    echo.
    echo === Docker Hub Configuration ===
    set /p DOCKER_USERNAME="Enter Docker Hub Username: "
    set /p DOCKER_PASSWORD="Enter Docker Hub Password or Access Token: "
    
    echo.
    echo Authenticating with Docker Hub...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Docker Hub authentication failed
        exit /b 1
    )
    
    set /p IMAGE_TAG="Enter image tag (default: latest): "
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    for /f "delims=" %%a in ('powershell -Command "'!IMAGE_TAG!' -replace '[^a-zA-Z0-9.-]','-' -replace '^-+|-+$','' | ForEach-Object { $_.ToLower() }"') do set IMAGE_TAG=%%a
    
    set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!
    
) else (
    echo ERROR: Invalid registry choice
    exit /b 1
)

echo.
echo ==========================================
echo Building Docker Image
echo ==========================================
echo Image: !FULL_IMAGE_NAME!
echo.

docker build -f Dockerfile -t !FULL_IMAGE_NAME! .

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed
    exit /b 1
)

echo.
echo ==========================================
echo Pushing Docker Image
echo ==========================================
echo.

docker push !FULL_IMAGE_NAME!

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed
    exit /b 1
)

echo.
echo ==========================================
echo Build and Push Completed Successfully
echo ==========================================
echo Image: !FULL_IMAGE_NAME!
echo.
echo Next steps:
if "!REGISTRY_CHOICE!"=="1" (
    echo 1. Update ECS task definition with image URI: !FULL_IMAGE_NAME!
    echo 2. Run deploy-image.bat to deploy to ECS Fargate
) else (
    echo 1. Update your deployment configuration with: !FULL_IMAGE_NAME!
    echo 2. Deploy using your preferred method
)
echo.

endlocal