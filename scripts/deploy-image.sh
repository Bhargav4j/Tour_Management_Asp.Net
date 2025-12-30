#!/bin/bash
set -e
set -o pipefail

echo "============================================"
echo "  TourManagement - ECS Fargate Deployment"
echo "============================================"
echo ""

# Configuration
PROJECT_NAME="tourmanagement"
TASK_FAMILY="${PROJECT_NAME}-task"
SERVICE_NAME="${PROJECT_NAME}-service"

# Prompt for AWS configuration
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
export AWS_DEFAULT_REGION="$AWS_REGION"

read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME

# Get AWS Account ID
echo ""
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)

if [ -z "$ACCOUNT_ID" ]; then
    echo "ERROR: Failed to retrieve AWS Account ID. Check AWS credentials."
    exit 1
fi

echo "AWS Account ID: $ACCOUNT_ID"

# Check/Create ECS Cluster
echo ""
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
    echo "Cluster created successfully"
}

# Network configuration
echo ""
echo "--- Network Configuration ---"
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNETS_INPUT
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP

# Parse subnets
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNETS_INPUT"
SUBNET_1="${SUBNET_ARRAY[0]}"
SUBNET_2="${SUBNET_ARRAY[1]:-$SUBNET_1}"

echo ""
echo "Network Configuration:"
echo "  VPC: $VPC_ID"
echo "  Subnet 1: $SUBNET_1"
echo "  Subnet 2: $SUBNET_2"
echo "  Security Group: $SECURITY_GROUP"

# Database configuration
echo ""
echo "--- Database Configuration ---"
read -p "Enter database server (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): " DB_SERVER
read -p "Enter database name (default: TourManagementDb): " DB_NAME
DB_NAME=${DB_NAME:-TourManagementDb}
read -p "Enter database user (default: admin): " DB_USER
DB_USER=${DB_USER:-admin}
read -sp "Enter database password: " DB_PASSWORD
echo ""

# Docker image configuration
echo ""
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): " IMAGE_URI

# Load balancer configuration
echo ""
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo ""
    echo "Creating Application Load Balancer and Target Group..."
    
    # Create target group with ip target type for Fargate
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
        --output text 2>/dev/null || \
        aws elbv2 describe-target-groups \
        --names "$TG_NAME" \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text)
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create ALB
    ALB_NAME="${PROJECT_NAME}-alb"
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name "$ALB_NAME" \
        --subnets $SUBNET_1 $SUBNET_2 \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || \
        aws elbv2 describe-load-balancers \
        --names "$ALB_NAME" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text)
    
    echo "Load Balancer ARN: $ALB_ARN"
    
    # Create listener
    aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" >/dev/null 2>&1 || echo "Listener already exists"
    
    # Get ALB DNS name
    ALB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$ALB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    echo "Load Balancer DNS: $ALB_DNS"
else
    TARGET_GROUP_ARN=""
    echo "Skipping load balancer configuration"
fi

# Create CloudWatch Log Group
echo ""
echo "Creating CloudWatch Log Group..."
LOG_GROUP="/ecs/${PROJECT_NAME}"
aws logs create-log-group --log-group-name "$LOG_GROUP" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists"

# Update task definition
echo ""
echo "Preparing ECS task definition..."
cp ecs/task-definition.json ecs/task-definition-temp.json

sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" ecs/task-definition-temp.json
sed -i "s|{{AWS_REGION}}|${AWS_REGION}|g" ecs/task-definition-temp.json
sed -i "s|{{ACCOUNT_ID}}|${ACCOUNT_ID}|g" ecs/task-definition-temp.json
sed -i "s|{{DB_SERVER}}|${DB_SERVER}|g" ecs/task-definition-temp.json
sed -i "s|{{DB_NAME}}|${DB_NAME}|g" ecs/task-definition-temp.json
sed -i "s|{{DB_USER}}|${DB_USER}|g" ecs/task-definition-temp.json
sed -i "s|{{DB_PASSWORD}}|${DB_PASSWORD}|g" ecs/task-definition-temp.json

echo "Registering task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://ecs/task-definition-temp.json \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

echo "Task Definition ARN: $TASK_DEF_ARN"

# Clean up temp file
rm -f ecs/task-definition-temp.json

# Update service definition
echo ""
echo "Preparing ECS service definition..."
cp ecs/service-definition.json ecs/service-definition-temp.json

sed -i "s|{{CLUSTER_NAME}}|${CLUSTER_NAME}|g" ecs/service-definition-temp.json
sed -i "s|{{SUBNET_1}}|${SUBNET_1}|g" ecs/service-definition-temp.json
sed -i "s|{{SUBNET_2}}|${SUBNET_2}|g" ecs/service-definition-temp.json
sed -i "s|{{SECURITY_GROUP}}|${SECURITY_GROUP}|g" ecs/service-definition-temp.json
sed -i "s|{{TARGET_GROUP_ARN}}|${TARGET_GROUP_ARN}|g" ecs/service-definition-temp.json

# Remove loadBalancers section if not needed
if [ -z "$TARGET_GROUP_ARN" ]; then
    # Remove loadBalancers and healthCheckGracePeriodSeconds
    jq 'del(.loadBalancers, .healthCheckGracePeriodSeconds)' ecs/service-definition-temp.json > ecs/service-definition-temp2.json
    mv ecs/service-definition-temp2.json ecs/service-definition-temp.json
fi

# Check if service exists
echo ""
echo "Checking if service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text 2>/dev/null)

if [ -z "$EXISTING_SERVICE" ] || [ "$EXISTING_SERVICE" = "None" ]; then
    echo "Service does not exist. Creating new service..."
    aws ecs create-service \
        --cli-input-json file://ecs/service-definition-temp.json \
        --region "$AWS_REGION"
else
    echo "Service exists. Updating service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --desired-count 2 \
        --region "$AWS_REGION"
fi

# Clean up temp file
rm -f ecs/service-definition-temp.json

echo ""
echo "Waiting for service to stabilize..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

# Verify deployment
echo ""
echo "============================================"
echo "  Deployment Completed Successfully!"
echo "============================================"
echo ""
echo "Deployment Details:"
echo "  Cluster: $CLUSTER_NAME"
echo "  Service: $SERVICE_NAME"
echo "  Task Definition: $TASK_DEF_ARN"
echo "  Region: $AWS_REGION"

if [ -n "$ALB_DNS" ]; then
    echo "  Application URL: http://$ALB_DNS"
    echo "  Health Check: http://$ALB_DNS/health"
fi

echo "  CloudWatch Logs: $LOG_GROUP"
echo ""

# Display running tasks
echo "Fetching running tasks..."
aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].{Running:runningCount,Desired:desiredCount,Status:status}' \
    --output table

echo ""
echo "To view logs, use:"
echo "  aws logs tail $LOG_GROUP --follow --region $AWS_REGION"
echo ""