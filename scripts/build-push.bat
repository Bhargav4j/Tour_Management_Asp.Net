@echo off
setlocal enabledelayedexpansion

echo ========================================
echo Docker Build and Push Script
echo ========================================
echo.

REM Project configuration
set PROJECT_NAME=TourManagement

REM Sanitize image name: lowercase, replace special chars with hyphens
set IMAGE_NAME=!PROJECT_NAME!
for %%i in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
    set IMAGE_NAME=!IMAGE_NAME:%%i=%%i!
)
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
set IMAGE_NAME=tourmanagement

echo Project: %PROJECT_NAME%
echo Sanitized Image Name: !IMAGE_NAME!
echo.

REM Select registry type
echo Select Docker Registry:
echo 1. AWS ECR (Elastic Container Registry)
echo 2. Docker Hub
set /p REGISTRY_CHOICE="Enter your choice (1 or 2): "
echo.

if "!REGISTRY_CHOICE!"=="1" (
    echo ========================================
    echo AWS ECR Configuration
    echo ========================================
    
    set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
    set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
    set /p ECR_REPO="Enter ECR Repository Name (default: !IMAGE_NAME!): "
    if "!ECR_REPO!"==" " set ECR_REPO=!IMAGE_NAME!
    
    set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
    
    echo.
    echo Logging into AWS ECR...
    aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: ECR login failed. Please check your AWS credentials and region.
        exit /b 1
    )
    
    echo Checking if ECR repository exists...
    aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Repository does not exist. Creating ECR repository: !ECR_REPO!
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
        echo Repository created successfully.
    )
    
    set /p IMAGE_TAG="Enter image tag (default: latest): "
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    
    set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!
    
) else if "!REGISTRY_CHOICE!"=="2" (
    echo ========================================
    echo Docker Hub Configuration
    echo ========================================
    
    set /p DOCKER_USERNAME="Enter Docker Hub username: "
    set /p DOCKER_PASSWORD="Enter Docker Hub password or access token: "
    
    echo Logging into Docker Hub...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Docker Hub login failed. Please check your credentials.
        exit /b 1
    )
    
    set /p IMAGE_TAG="Enter image tag (default: latest): "
    if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
    
    set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!
    
) else (
    echo ERROR: Invalid choice. Please select 1 or 2.
    exit /b 1
)

echo.
echo ========================================
echo Building Docker Image
echo ========================================
echo Image: !FULL_IMAGE_NAME!
echo.

docker build -t "!FULL_IMAGE_NAME!" -f Dockerfile .

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)

echo.
echo ========================================
echo Pushing Image to Registry
echo ========================================
echo.

docker push "!FULL_IMAGE_NAME!"

if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ========================================
echo Build and Push Completed Successfully!
echo ========================================
echo Image: !FULL_IMAGE_NAME!
echo.
echo Next steps:
echo 1. Update your deployment configuration with this image URI
echo 2. Deploy to your target environment (ECS, Kubernetes, etc.)
echo.

endlocal