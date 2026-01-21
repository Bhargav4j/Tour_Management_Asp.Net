#!/bin/bash
set -e
set -o pipefail

# TourManagement Application - AWS ECS Fargate Deployment Script

echo "==================================================="
echo "TourManagement AWS ECS Fargate Deployment Script"
echo "==================================================="
echo ""

# Configuration
PROJECT_NAME="tourmanagement"
TASK_FAMILY="${PROJECT_NAME}-task"
SERVICE_NAME="${PROJECT_NAME}-service"
CONTAINER_NAME="${PROJECT_NAME}-app"
LOG_GROUP="/ecs/${PROJECT_NAME}-app"

# Prompt for AWS configuration
echo "=== AWS Configuration ==="
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME
echo ""

# Get AWS Account ID
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
if [ -z "$ACCOUNT_ID" ]; then
    echo "ERROR: Failed to retrieve AWS Account ID. Please check your AWS credentials."
    exit 1
fi
echo "AWS Account ID: $ACCOUNT_ID"
echo ""

# Check if ECS cluster exists, create if it doesn't
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
    echo "ECS cluster created successfully!"
}
echo ""

# Prompt for network configuration
echo "=== Network Configuration ==="
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNET_IDS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP
echo ""

# Split subnet IDs
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNET_IDS"
SUBNET_1="${SUBNET_ARRAY[0]}"
SUBNET_2="${SUBNET_ARRAY[1]:-$SUBNET_1}"

# Prompt for Docker image URI
echo "=== Docker Image Configuration ==="
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): " IMAGE_URI
echo ""

# Prompt for application environment variables
echo "=== Application Configuration ==="
read -p "Enter database host (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): " DB_HOST
read -p "Enter database port (default: 5432): " DB_PORT
DB_PORT=${DB_PORT:-5432}
read -p "Enter database name (default: tourmanagementdb): " DB_NAME
DB_NAME=${DB_NAME:-tourmanagementdb}
read -p "Enter database username: " DB_USER
read -sp "Enter database password: " DB_PASSWORD
echo ""
read -p "Enter Redis connection string (e.g., redis.abc123.cache.amazonaws.com:6379) [optional]: " REDIS_CONNECTION_STRING
echo ""

# Load balancer configuration
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB
echo ""

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo "Creating Application Load Balancer and Target Group..."
    
    # Create Target Group with target-type ip (required for Fargate awsvpc mode)
    TG_NAME="${PROJECT_NAME}-tg"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port 8080 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-path "/health" \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null)
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        # Target group might already exist, try to describe it
        TARGET_GROUP_ARN=$(aws elbv2 describe-target-groups \
            --names "$TG_NAME" \
            --region "$AWS_REGION" \
            --query 'TargetGroups[0].TargetGroupArn' \
            --output text 2>/dev/null)
    fi
    
    if [ -z "$TARGET_GROUP_ARN" ] || [ "$TARGET_GROUP_ARN" = "None" ]; then
        echo "ERROR: Failed to create or find target group."
        exit 1
    fi
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    echo ""
else
    echo "Skipping load balancer configuration."
    TARGET_GROUP_ARN=""
    echo ""
fi

# Create CloudWatch log group if it doesn't exist
echo "Creating CloudWatch log group..."
aws logs create-log-group --log-group-name "$LOG_GROUP" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists."
echo ""

# Prepare task definition JSON
echo "Preparing task definition..."
cp ecs/task-definition.json /tmp/task-definition.json

# Replace placeholders in task definition
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" /tmp/task-definition.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" /tmp/task-definition.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" /tmp/task-definition.json
sed -i "s|{{DB_HOST}}|$DB_HOST|g" /tmp/task-definition.json
sed -i "s|{{DB_PORT}}|$DB_PORT|g" /tmp/task-definition.json
sed -i "s|{{DB_NAME}}|$DB_NAME|g" /tmp/task-definition.json
sed -i "s|{{DB_USER}}|$DB_USER|g" /tmp/task-definition.json
sed -i "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" /tmp/task-definition.json
sed -i "s|{{REDIS_CONNECTION_STRING}}|$REDIS_CONNECTION_STRING|g" /tmp/task-definition.json

echo "Registering task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file:///tmp/task-definition.json \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

if [ -z "$TASK_DEF_ARN" ]; then
    echo "ERROR: Failed to register task definition."
    exit 1
fi

echo "Task definition registered: $TASK_DEF_ARN"
echo ""

# Prepare service definition JSON
echo "Preparing service definition..."
cp ecs/service-definition.json /tmp/service-definition.json

# Replace placeholders in service definition
sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" /tmp/service-definition.json
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" /tmp/service-definition.json
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" /tmp/service-definition.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" /tmp/service-definition.json

if [ -n "$TARGET_GROUP_ARN" ]; then
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" /tmp/service-definition.json
else
    # Remove loadBalancers section if no load balancer is needed
    sed -i '/"loadBalancers":/,/],/d' /tmp/service-definition.json
    sed -i '/"healthCheckGracePeriodSeconds":/d' /tmp/service-definition.json
fi

# Check if service exists
echo "Checking if ECS service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].serviceName' \
    --output text 2>/dev/null)

if [ "$EXISTING_SERVICE" = "$SERVICE_NAME" ]; then
    echo "Service exists. Updating service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --force-new-deployment \
        --region "$AWS_REGION" >/dev/null
    echo "Service update initiated."
else
    echo "Service does not exist. Creating service..."
    aws ecs create-service \
        --cli-input-json file:///tmp/service-definition.json \
        --region "$AWS_REGION" >/dev/null
    echo "Service created."
fi
echo ""

# Wait for service stability
echo "Waiting for service to become stable (this may take a few minutes)..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

echo "Service is stable!"
echo ""

# Verify deployment
echo "Verifying deployment..."
RUNNING_COUNT=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].runningCount' \
    --output text)

echo "Running tasks: $RUNNING_COUNT"
echo ""

if [ -n "$TARGET_GROUP_ARN" ]; then
    # Get load balancer DNS name
    LB_ARN=$(aws elbv2 describe-target-groups \
        --target-group-arns "$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].LoadBalancerArns[0]' \
        --output text 2>/dev/null)
    
    if [ -n "$LB_ARN" ] && [ "$LB_ARN" != "None" ]; then
        LB_DNS=$(aws elbv2 describe-load-balancers \
            --load-balancer-arns "$LB_ARN" \
            --region "$AWS_REGION" \
            --query 'LoadBalancers[0].DNSName' \
            --output text 2>/dev/null)
        
        if [ -n "$LB_DNS" ] && [ "$LB_DNS" != "None" ]; then
            echo "Load Balancer DNS: http://$LB_DNS"
            echo "Health Check: http://$LB_DNS/health"
            echo ""
        fi
    fi
fi

echo "==================================================="
echo "Deployment completed successfully!"
echo "==================================================="
echo ""
echo "Service Name: $SERVICE_NAME"
echo "Cluster: $CLUSTER_NAME"
echo "Region: $AWS_REGION"
echo "CloudWatch Logs: $LOG_GROUP"
echo ""
echo "To view logs:"
echo "aws logs tail $LOG_GROUP --follow --region $AWS_REGION"
echo ""
echo "To check service status:"
echo "aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo ""

# Cleanup
rm -f /tmp/task-definition.json /tmp/service-definition.json