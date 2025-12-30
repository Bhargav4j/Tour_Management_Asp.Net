#!/bin/bash
set -e

echo "============================================"
echo "  TourManagement - Docker Build & Push"
echo "============================================"
echo ""

# Project configuration
PROJECT_NAME="TourManagement"
DOCKERFILE="Dockerfile"

# Sanitize image name: lowercase, replace non-alphanumeric with hyphens, trim leading/trailing hyphens
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "Sanitized image name: $IMAGE_NAME"
echo ""

# Prompt for image tag
read -p "Enter image tag (default: latest): " IMAGE_TAG
IMAGE_TAG=${IMAGE_TAG:-latest}

# Sanitize tag: lowercase, replace non-alphanumeric with hyphens, trim
IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9.-' '-' | sed 's/^-*//;s/-*$//')

if [ -z "$IMAGE_TAG" ]; then
    IMAGE_TAG="latest"
fi

echo "Using tag: $IMAGE_TAG"
echo ""

# Registry selection
echo "Select Docker Registry:"
echo "1. AWS ECR (Elastic Container Registry)"
echo "2. Docker Hub"
read -p "Enter choice (1 or 2): " REGISTRY_CHOICE

if [ "$REGISTRY_CHOICE" = "1" ]; then
    echo ""
    echo "--- AWS ECR Configuration ---"
    read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
    read -p "Enter AWS Account ID: " AWS_ACCOUNT_ID
    read -p "Enter ECR Repository Name (default: $IMAGE_NAME): " ECR_REPO
    ECR_REPO=${ECR_REPO:-$IMAGE_NAME}
    
    REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
    FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"
    
    echo ""
    echo "Authenticating with AWS ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
    
    if [ $? -ne 0 ]; then
        echo "ERROR: ECR authentication failed"
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
        
        echo "Repository created successfully"
    }
    
elif [ "$REGISTRY_CHOICE" = "2" ]; then
    echo ""
    echo "--- Docker Hub Configuration ---"
    read -p "Enter Docker Hub username: " DOCKER_USERNAME
    read -sp "Enter Docker Hub password/token: " DOCKER_PASSWORD
    echo ""
    
    FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"
    
    echo ""
    echo "Authenticating with Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Docker Hub authentication failed"
        exit 1
    fi
    
else
    echo "ERROR: Invalid choice. Please select 1 or 2."
    exit 1
fi

echo ""
echo "============================================"
echo "Building Docker image: $FULL_IMAGE_NAME"
echo "============================================"
echo ""

# Build Docker image
docker build -f "$DOCKERFILE" -t "$FULL_IMAGE_NAME" .

if [ $? -ne 0 ]; then
    echo "ERROR: Docker build failed"
    exit 1
fi

echo ""
echo "============================================"
echo "Pushing image to registry"
echo "============================================"
echo ""

# Push Docker image
docker push "$FULL_IMAGE_NAME"

if [ $? -ne 0 ]; then
    echo "ERROR: Docker push failed"
    exit 1
fi

echo ""
echo "============================================"
echo "  Build and Push Completed Successfully!"
echo "============================================"
echo ""
echo "Image: $FULL_IMAGE_NAME"
echo ""
echo "Next steps:"
echo "1. Update ECS task definition with this image URI"
echo "2. Run deploy-image.sh to deploy to ECS Fargate"
echo ""