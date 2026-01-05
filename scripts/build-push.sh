#!/bin/bash
set -e

echo "========================================"
echo "Docker Image Build and Push Script"
echo "========================================"
echo ""

# Project configuration
PROJECT_NAME="tourmanagement"

# Prompt for image tag
echo "Enter image tag (default: latest): "
read -r IMAGE_TAG
IMAGE_TAG=${IMAGE_TAG:-latest}

# Sanitize tag
IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9.-' '-' | sed 's/^-*//;s/-*$//')
IMAGE_TAG=${IMAGE_TAG:-latest}

echo ""
echo "Select registry type:"
echo "1. AWS ECR"
echo "2. Docker Hub"
echo ""
read -p "Enter choice (1 or 2): " REGISTRY_CHOICE

case $REGISTRY_CHOICE in
  1)
    echo ""
    echo "=== AWS ECR Configuration ==="
    read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
    read -p "Enter AWS Account ID: " AWS_ACCOUNT_ID
    read -p "Enter ECR Repository Name (default: $PROJECT_NAME): " ECR_REPO
    ECR_REPO=${ECR_REPO:-$PROJECT_NAME}
    
    REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
    IMAGE_NAME=$(echo "$ECR_REPO" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')
    FULL_IMAGE_NAME="${REGISTRY_URL}/${IMAGE_NAME}:${IMAGE_TAG}"
    
    echo ""
    echo "Authenticating with AWS ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
    
    if [ $? -ne 0 ]; then
      echo "ERROR: ECR authentication failed"
      exit 1
    fi
    
    echo "Checking if ECR repository exists..."
    aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || {
      echo "Repository does not exist. Creating ECR repository..."
      aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
    }
    ;;
  
  2)
    echo ""
    echo "=== Docker Hub Configuration ==="
    read -p "Enter Docker Hub username: " DOCKER_USERNAME
    read -sp "Enter Docker Hub password/token: " DOCKER_PASSWORD
    echo ""
    read -p "Enter repository name (default: $PROJECT_NAME): " DOCKER_REPO
    DOCKER_REPO=${DOCKER_REPO:-$PROJECT_NAME}
    
    IMAGE_NAME=$(echo "$DOCKER_REPO" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')
    FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"
    
    echo ""
    echo "Authenticating with Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
    
    if [ $? -ne 0 ]; then
      echo "ERROR: Docker Hub authentication failed"
      exit 1
    fi
    ;;
  
  *)
    echo "Invalid choice. Exiting."
    exit 1
    ;;
esac

echo ""
echo "Building Docker image: $FULL_IMAGE_NAME"
echo "Build context: $(pwd)"
echo ""

docker build -f Dockerfile -t "$FULL_IMAGE_NAME" .

if [ $? -ne 0 ]; then
  echo "ERROR: Docker build failed"
  exit 1
fi

echo ""
echo "Pushing image to registry..."
docker push "$FULL_IMAGE_NAME"

if [ $? -ne 0 ]; then
  echo "ERROR: Docker push failed"
  exit 1
fi

echo ""
echo "========================================"
echo "SUCCESS!"
echo "========================================"
echo "Image: $FULL_IMAGE_NAME"
echo "Tag: $IMAGE_TAG"
echo ""
echo "Use this image URI for deployment:"
echo "$FULL_IMAGE_NAME"
echo ""