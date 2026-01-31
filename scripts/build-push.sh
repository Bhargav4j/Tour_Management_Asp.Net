#!/bin/bash
set -e

echo "============================================"
echo "  TourManagement Docker Build & Push Script"
echo "============================================"
echo ""

# Project configuration
PROJECT_NAME="TourManagement"
DOCKERFILE_PATH="Dockerfile"

# Sanitize project name for Docker image tag
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')
echo "Sanitized image name: $IMAGE_NAME"
echo ""

# Prompt for registry selection
echo "Select container registry:"
echo "1. AWS ECR (Elastic Container Registry)"
echo "2. Docker Hub"
read -p "Enter choice (1 or 2): " REGISTRY_CHOICE
echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
    # AWS ECR Configuration
    echo "=== AWS ECR Configuration ==="
    read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
    read -p "Enter AWS Account ID: " AWS_ACCOUNT_ID
    read -p "Enter ECR Repository Name (default: $IMAGE_NAME): " ECR_REPO
    ECR_REPO=${ECR_REPO:-$IMAGE_NAME}
    
    REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
    
    echo ""
    echo "Authenticating with AWS ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to authenticate with AWS ECR"
        exit 1
    fi
    
    echo "Checking if ECR repository exists..."
    aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || {
        echo "Repository does not exist. Creating ECR repository: $ECR_REPO"
        aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
        if [ $? -ne 0 ]; then
            echo "ERROR: Failed to create ECR repository"
            exit 1
        fi
    }
    
    read -p "Enter image tag (default: latest): " IMAGE_TAG
    IMAGE_TAG=${IMAGE_TAG:-latest}
    IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9.-' '-' | sed 's/^-*//;s/-*$//')
    
    FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"
    
elif [ "$REGISTRY_CHOICE" = "2" ]; then
    # Docker Hub Configuration
    echo "=== Docker Hub Configuration ==="
    read -p "Enter Docker Hub username: " DOCKER_USERNAME
    read -s -p "Enter Docker Hub password/token: " DOCKER_PASSWORD
    echo ""
    
    echo "Authenticating with Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to authenticate with Docker Hub"
        exit 1
    fi
    
    read -p "Enter image tag (default: latest): " IMAGE_TAG
    IMAGE_TAG=${IMAGE_TAG:-latest}
    IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9.-' '-' | sed 's/^-*//;s/-*$//')
    
    FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"
    
else
    echo "ERROR: Invalid choice. Please select 1 or 2."
    exit 1
fi

echo ""
echo "=== Building Docker Image ==="
echo "Image: $FULL_IMAGE_NAME"
echo "Context: $(pwd)"
echo "Dockerfile: $DOCKERFILE_PATH"
echo ""

docker build -f "$DOCKERFILE_PATH" -t "$FULL_IMAGE_NAME" .

if [ $? -ne 0 ]; then
    echo "ERROR: Docker build failed"
    exit 1
fi

echo ""
echo "=== Pushing Docker Image ==="
echo "Pushing: $FULL_IMAGE_NAME"
echo ""

docker push "$FULL_IMAGE_NAME"

if [ $? -ne 0 ]; then
    echo "ERROR: Docker push failed"
    exit 1
fi

echo ""
echo "============================================"
echo "  Build and Push Completed Successfully!"
echo "============================================"
echo "Image: $FULL_IMAGE_NAME"
echo ""