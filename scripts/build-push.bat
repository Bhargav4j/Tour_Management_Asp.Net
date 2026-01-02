@echo off
setlocal enabledelayedexpansion

echo ========================================
echo Docker Build and Push Script
echo ========================================
echo.

rem Project configuration
set PROJECT_NAME=TourManagement

rem Sanitize image name: lowercase, replace non-alphanumeric with hyphens
set IMAGE_NAME=%PROJECT_NAME%
for %%i in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do set IMAGE_NAME=!IMAGE_NAME:%%i=%%i!
set IMAGE_NAME=%IMAGE_NAME: =-%
set IMAGE_NAME=%IMAGE_NAME:A=a%
set IMAGE_NAME=%IMAGE_NAME:B=b%
set IMAGE_NAME=%IMAGE_NAME:C=c%
set IMAGE_NAME=%IMAGE_NAME:D=d%
set IMAGE_NAME=%IMAGE_NAME:E=e%
set IMAGE_NAME=%IMAGE_NAME:F=f%
set IMAGE_NAME=%IMAGE_NAME:G=g%
set IMAGE_NAME=%IMAGE_NAME:H=h%
set IMAGE_NAME=%IMAGE_NAME:I=i%
set IMAGE_NAME=%IMAGE_NAME:J=j%
set IMAGE_NAME=%IMAGE_NAME:K=k%
set IMAGE_NAME=%IMAGE_NAME:L=l%
set IMAGE_NAME=%IMAGE_NAME:M=m%
set IMAGE_NAME=%IMAGE_NAME:N=n%
set IMAGE_NAME=%IMAGE_NAME:O=o%
set IMAGE_NAME=%IMAGE_NAME:P=p%
set IMAGE_NAME=%IMAGE_NAME:Q=q%
set IMAGE_NAME=%IMAGE_NAME:R=r%
set IMAGE_NAME=%IMAGE_NAME:S=s%
set IMAGE_NAME=%IMAGE_NAME:T=t%
set IMAGE_NAME=%IMAGE_NAME:U=u%
set IMAGE_NAME=%IMAGE_NAME:V=v%
set IMAGE_NAME=%IMAGE_NAME:W=w%
set IMAGE_NAME=%IMAGE_NAME:X=x%
set IMAGE_NAME=%IMAGE_NAME:Y=y%
set IMAGE_NAME=%IMAGE_NAME:Z=z%

echo Project: %PROJECT_NAME%
echo Sanitized Image Name: !IMAGE_NAME!
echo.

rem Prompt for registry type
echo Select Container Registry:
echo 1. AWS ECR (Elastic Container Registry)
echo 2. Docker Hub
set /p REGISTRY_CHOICE="Enter your choice (1 or 2): "
echo.

if "!REGISTRY_CHOICE!"=="1" (
    echo ========================================
    echo AWS ECR Configuration
    echo ========================================
    echo.
    
    set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
    set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
    set /p ECR_REPO="Enter ECR Repository Name (default: !IMAGE_NAME!): "
    if "!ECR_REPO!"==" " set ECR_REPO=!IMAGE_NAME!
    
    set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
    set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!
    
    echo.
    echo Authenticating with AWS ECR...
    aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to authenticate with AWS ECR
        exit /b 1
    )
    
    echo Successfully authenticated with AWS ECR
    echo.
    
    echo Checking if ECR repository exists...
    aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Repository does not exist. Creating ECR repository: !ECR_REPO!
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
        echo Repository created successfully
    )
    echo.
    
) else if "!REGISTRY_CHOICE!"=="2" (
    echo ========================================
    echo Docker Hub Configuration
    echo ========================================
    echo.
    
    set /p DOCKER_USERNAME="Enter Docker Hub Username: "
    set /p DOCKER_PASSWORD="Enter Docker Hub Password or Access Token: "
    echo.
    
    echo Authenticating with Docker Hub...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to authenticate with Docker Hub
        exit /b 1
    )
    
    echo Successfully authenticated with Docker Hub
    echo.
    
    set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!
) else (
    echo ERROR: Invalid registry choice. Please select 1 or 2.
    exit /b 1
)

rem Prompt for image tag
set /p IMAGE_TAG="Enter image tag (default: latest): "
if "!IMAGE_TAG!"==" " set IMAGE_TAG=latest

rem Sanitize tag
for %%i in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do set IMAGE_TAG=!IMAGE_TAG:%%i=%%i!
set IMAGE_TAG=%IMAGE_TAG:A=a%
set IMAGE_TAG=%IMAGE_TAG:B=b%
set IMAGE_TAG=%IMAGE_TAG:C=c%
set IMAGE_TAG=%IMAGE_TAG:D=d%
set IMAGE_TAG=%IMAGE_TAG:E=e%
set IMAGE_TAG=%IMAGE_TAG:F=f%
set IMAGE_TAG=%IMAGE_TAG:G=g%
set IMAGE_TAG=%IMAGE_TAG:H=h%
set IMAGE_TAG=%IMAGE_TAG:I=i%
set IMAGE_TAG=%IMAGE_TAG:J=j%
set IMAGE_TAG=%IMAGE_TAG:K=k%
set IMAGE_TAG=%IMAGE_TAG:L=l%
set IMAGE_TAG=%IMAGE_TAG:M=m%
set IMAGE_TAG=%IMAGE_TAG:N=n%
set IMAGE_TAG=%IMAGE_TAG:O=o%
set IMAGE_TAG=%IMAGE_TAG:P=p%
set IMAGE_TAG=%IMAGE_TAG:Q=q%
set IMAGE_TAG=%IMAGE_TAG:R=r%
set IMAGE_TAG=%IMAGE_TAG:S=s%
set IMAGE_TAG=%IMAGE_TAG:T=t%
set IMAGE_TAG=%IMAGE_TAG:U=u%
set IMAGE_TAG=%IMAGE_TAG:V=v%
set IMAGE_TAG=%IMAGE_TAG:W=w%
set IMAGE_TAG=%IMAGE_TAG:X=x%
set IMAGE_TAG=%IMAGE_TAG:Y=y%
set IMAGE_TAG=%IMAGE_TAG:Z=z%

if "!IMAGE_TAG!"==" " set IMAGE_TAG=latest

set FULL_IMAGE_NAME=!FULL_IMAGE_NAME!:!IMAGE_TAG!

echo.
echo ========================================
echo Building Docker Image
echo ========================================
echo Image: !FULL_IMAGE_NAME!
echo.

docker build -f Dockerfile -t !FULL_IMAGE_NAME! .

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed
    exit /b 1
)

echo.
echo Docker image built successfully: !FULL_IMAGE_NAME!
echo.

echo ========================================
echo Pushing Docker Image
echo ========================================
echo.

docker push !FULL_IMAGE_NAME!

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed
    exit /b 1
)

echo.
echo ========================================
echo Build and Push Completed Successfully
echo ========================================
echo Image: !FULL_IMAGE_NAME!
echo.
echo Next steps:
echo 1. Use this image URI in your ECS task definition
echo 2. Run the deploy-image.bat script to deploy to AWS ECS
echo.

endlocal