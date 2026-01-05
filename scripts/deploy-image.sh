#!/bin/bash
set -e
set -o pipefail

echo "========================================"
echo "AWS ECS Fargate Deployment Script"
echo "========================================"
echo ""

# Configuration
PROJECT_NAME="tourmanagement"
SERVICE_NAME="${PROJECT_NAME}-service"
TASK_FAMILY="${PROJECT_NAME}-task"

# Prompt for AWS configuration
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME

# Get AWS Account ID
echo ""
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "Account ID: $ACCOUNT_ID"

# Prompt for network configuration
echo ""
echo "=== Network Configuration ==="
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNETS_INPUT
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP

# Parse subnets
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNETS_INPUT"
SUBNET_1=${SUBNET_ARRAY[0]}
SUBNET_2=${SUBNET_ARRAY[1]:-$SUBNET_1}

# Prompt for Docker image
echo ""
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): " IMAGE_URI

# Prompt for database configuration
echo ""
echo "=== Database Configuration ==="
read -p "Enter DB Server (e.g., mydb.xxx.rds.amazonaws.com): " DB_SERVER
read -p "Enter DB Name (default: TourManagementDB): " DB_NAME
DB_NAME=${DB_NAME:-TourManagementDB}
read -p "Enter DB User (default: admin): " DB_USER
DB_USER=${DB_USER:-admin}
read -sp "Enter DB Password: " DB_PASSWORD
echo ""
read -p "Enter Redis Connection String (optional, press Enter to skip): " REDIS_CONNECTION

# Check if cluster exists
echo ""
echo "Checking ECS cluster..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
  echo "Cluster does not exist. Creating cluster..."
  aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
}

# Load balancer configuration
echo ""
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
  echo ""
  echo "Creating Application Load Balancer and Target Group..."
  
  # Create ALB
  ALB_NAME="${PROJECT_NAME}-alb"
  echo "Creating ALB: $ALB_NAME"
  ALB_ARN=$(aws elbv2 create-load-balancer \
    --name "$ALB_NAME" \
    --subnets $SUBNET_1 $SUBNET_2 \
    --security-groups "$SECURITY_GROUP" \
    --scheme internet-facing \
    --type application \
    --region "$AWS_REGION" \
    --query 'LoadBalancers[0].LoadBalancerArn' \
    --output text 2>/dev/null || aws elbv2 describe-load-balancers --names "$ALB_NAME" --region "$AWS_REGION" --query 'LoadBalancers[0].LoadBalancerArn' --output text)
  
  # Wait for ALB to be active
  echo "Waiting for ALB to become active..."
  aws elbv2 wait load-balancer-available --load-balancer-arns "$ALB_ARN" --region "$AWS_REGION"
  
  # Create Target Group with ip target type for Fargate
  TG_NAME="${PROJECT_NAME}-tg"
  echo "Creating Target Group: $TG_NAME"
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
    --output text 2>/dev/null || aws elbv2 describe-target-groups --names "$TG_NAME" --region "$AWS_REGION" --query 'TargetGroups[0].TargetGroupArn' --output text)
  
  # Create listener
  echo "Creating ALB listener..."
  aws elbv2 create-listener \
    --load-balancer-arn "$ALB_ARN" \
    --protocol HTTP \
    --port 80 \
    --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
    --region "$AWS_REGION" >/dev/null 2>&1 || echo "Listener already exists"
  
  # Get ALB DNS name
  ALB_DNS=$(aws elbv2 describe-load-balancers --load-balancer-arns "$ALB_ARN" --region "$AWS_REGION" --query 'LoadBalancers[0].DNSName' --output text)
  
  echo "Load Balancer DNS: $ALB_DNS"
else
  TARGET_GROUP_ARN=""
fi

# Create CloudWatch log group
echo ""
echo "Creating CloudWatch log group..."
aws logs create-log-group --log-group-name "/ecs/${PROJECT_NAME}" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists"

# Replace placeholders in task definition
echo ""
echo "Preparing task definition..."
cp ecs/task-definition.json ecs/task-definition-resolved.json

sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" ecs/task-definition-resolved.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" ecs/task-definition-resolved.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_SERVER}}|$DB_SERVER|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_NAME}}|$DB_NAME|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_USER}}|$DB_USER|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" ecs/task-definition-resolved.json
sed -i "s|{{REDIS_CONNECTION}}|$REDIS_CONNECTION|g" ecs/task-definition-resolved.json

# Register task definition
echo "Registering task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
  --cli-input-json file://ecs/task-definition-resolved.json \
  --region "$AWS_REGION" \
  --query 'taskDefinition.taskDefinitionArn' \
  --output text)

echo "Task Definition ARN: $TASK_DEF_ARN"

# Prepare service definition
echo ""
echo "Preparing service definition..."
cp ecs/service-definition.json ecs/service-definition-resolved.json

sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" ecs/service-definition-resolved.json
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" ecs/service-definition-resolved.json
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" ecs/service-definition-resolved.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" ecs/service-definition-resolved.json

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
  sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" ecs/service-definition-resolved.json
else
  # Remove loadBalancers section if no LB needed
  sed -i '/"loadBalancers":/,/],/d' ecs/service-definition-resolved.json
  sed -i '/"healthCheckGracePeriodSeconds":/d' ecs/service-definition-resolved.json
fi

# Check if service exists
echo ""
echo "Checking if service exists..."
SERVICE_EXISTS=$(aws ecs describe-services \
  --cluster "$CLUSTER_NAME" \
  --services "$SERVICE_NAME" \
  --region "$AWS_REGION" \
  --query 'services[0].serviceName' \
  --output text 2>/dev/null)

if [ "$SERVICE_EXISTS" = "$SERVICE_NAME" ]; then
  echo "Service exists. Updating service..."
  aws ecs update-service \
    --cluster "$CLUSTER_NAME" \
    --service "$SERVICE_NAME" \
    --task-definition "$TASK_DEF_ARN" \
    --force-new-deployment \
    --region "$AWS_REGION"
else
  echo "Service does not exist. Creating service..."
  aws ecs create-service \
    --cli-input-json file://ecs/service-definition-resolved.json \
    --region "$AWS_REGION"
fi

# Wait for service stability
echo ""
echo "Waiting for service to become stable (this may take a few minutes)..."
aws ecs wait services-stable \
  --cluster "$CLUSTER_NAME" \
  --services "$SERVICE_NAME" \
  --region "$AWS_REGION"

# Verify deployment
echo ""
echo "Verifying deployment..."
RUNNING_COUNT=$(aws ecs describe-services \
  --cluster "$CLUSTER_NAME" \
  --services "$SERVICE_NAME" \
  --region "$AWS_REGION" \
  --query 'services[0].runningCount' \
  --output text)

echo ""
echo "========================================"
echo "DEPLOYMENT SUCCESSFUL!"
echo "========================================"
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Running Tasks: $RUNNING_COUNT"
echo "CloudWatch Logs: /ecs/${PROJECT_NAME}"

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
  echo "Load Balancer URL: http://$ALB_DNS"
  echo ""
  echo "Access your application at: http://$ALB_DNS"
fi

echo ""
echo "To view logs:"
echo "aws logs tail /ecs/${PROJECT_NAME} --follow --region $AWS_REGION"
echo ""
echo "To view service details:"
echo "aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo ""

# Cleanup temporary files
rm -f ecs/task-definition-resolved.json ecs/service-definition-resolved.json

echo "Deployment complete!"