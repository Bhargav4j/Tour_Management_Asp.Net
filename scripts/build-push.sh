#!/bin/bash
set -e

# TourManagement Application - Docker Build and Push Script
# This script builds the Docker image and pushes it to your chosen registry

echo "============================================"
echo "TourManagement Docker Build and Push Script"
echo "============================================"
echo ""

# Project configuration
PROJECT_NAME="TourManagement"

# Sanitize image name: lowercase, replace non-alphanumeric with hyphens, trim leading/trailing hyphens
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "Project: $PROJECT_NAME"
echo "Image name: $IMAGE_NAME"
echo ""

# Select registry type
echo "Select container registry:"
echo "1. AWS ECR (Elastic Container Registry)"
echo "2. Docker Hub"
read -p "Enter your choice (1 or 2): " REGISTRY_CHOICE
echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
    # AWS ECR Configuration
    echo "=== AWS ECR Configuration ==="
    read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
    read -p "Enter AWS Account ID: " AWS_ACCOUNT_ID
    read -p "Enter ECR repository name (default: $IMAGE_NAME): " ECR_REPO
    ECR_REPO=${ECR_REPO:-$IMAGE_NAME}
    
    REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
    FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}"
    
    echo ""
    echo "Authenticating with AWS ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
    
    if [ $? -ne 0 ]; then
        echo "ERROR: ECR authentication failed. Please check your AWS credentials and region."
        exit 1
    fi
    
    echo "ECR authentication successful!"
    echo ""
    
    # Check if ECR repository exists, create if it doesn't
    echo "Checking if ECR repository exists..."
    aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || {
        echo "Repository does not exist. Creating ECR repository: $ECR_REPO"
        aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
        echo "ECR repository created successfully!"
    }
    echo ""
    
elif [ "$REGISTRY_CHOICE" = "2" ]; then
    # Docker Hub Configuration
    echo "=== Docker Hub Configuration ==="
    read -p "Enter Docker Hub username: " DOCKER_USERNAME
    read -sp "Enter Docker Hub password or access token: " DOCKER_PASSWORD
    echo ""
    read -p "Enter repository name (default: $IMAGE_NAME): " DOCKER_REPO
    DOCKER_REPO=${DOCKER_REPO:-$IMAGE_NAME}
    
    FULL_IMAGE_NAME="${DOCKER_USERNAME}/${DOCKER_REPO}"
    
    echo ""
    echo "Authenticating with Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Docker Hub authentication failed. Please check your credentials."
        exit 1
    fi
    
    echo "Docker Hub authentication successful!"
    echo ""
else
    echo "ERROR: Invalid choice. Please run the script again and select 1 or 2."
    exit 1
fi

# Prompt for image tag
read -p "Enter image tag (default: latest): " IMAGE_TAG
IMAGE_TAG=${IMAGE_TAG:-latest}

# Sanitize tag: lowercase, replace non-alphanumeric with hyphens, trim leading/trailing hyphens
IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9.-' '-' | sed 's/^-*//;s/-*$//')
IMAGE_TAG=${IMAGE_TAG:-latest}

FULL_IMAGE_TAG="${FULL_IMAGE_NAME}:${IMAGE_TAG}"

echo ""
echo "Building Docker image..."
echo "Image: $FULL_IMAGE_TAG"
echo ""

# Build Docker image
docker build -f Dockerfile -t "$FULL_IMAGE_TAG" .

if [ $? -ne 0 ]; then
    echo "ERROR: Docker build failed."
    exit 1
fi

echo ""
echo "Docker build completed successfully!"
echo ""

# Push image to registry
echo "Pushing image to registry..."
docker push "$FULL_IMAGE_TAG"

if [ $? -ne 0 ]; then
    echo "ERROR: Docker push failed."
    exit 1
fi

echo ""
echo "============================================"
echo "Build and push completed successfully!"
echo "Image: $FULL_IMAGE_TAG"
echo "============================================"
echo ""
echo "Next steps:"
echo "1. Update your ECS task definition with this image URI"
echo "2. Run the deploy-image.sh script to deploy to ECS"
echo ""