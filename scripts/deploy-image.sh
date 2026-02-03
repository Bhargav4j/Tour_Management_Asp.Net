#!/bin/bash
set -e
set -o pipefail

echo "====================================="
echo "AWS ECS Fargate Deployment Script"
echo "====================================="
echo ""

# Prompt for AWS configuration
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., tourmanagement-cluster): " CLUSTER_NAME
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNETS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement-web:latest): " IMAGE_URI

echo ""
echo "Database Configuration:"
read -p "Enter Database Host (e.g., db.example.com): " DB_HOST
read -p "Enter Database Port (default: 5432): " DB_PORT
DB_PORT=${DB_PORT:-5432}
read -p "Enter Database Name (default: tourmanagementdb): " DB_NAME
DB_NAME=${DB_NAME:-tourmanagementdb}
read -p "Enter Database User (default: postgres): " DB_USER
DB_USER=${DB_USER:-postgres}
read -sp "Enter Database Password: " DB_PASSWORD
echo ""

echo ""
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

# Get AWS Account ID
echo ""
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "AWS Account ID: $ACCOUNT_ID"

# Check if cluster exists, create if not
echo ""
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" 2>/dev/null | grep -q "$CLUSTER_NAME" || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
}

# Parse subnets
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNETS"
SUBNET_1="${SUBNET_ARRAY[0]}"
SUBNET_2="${SUBNET_ARRAY[1]:-$SUBNET_1}"

# Create CloudWatch log group
echo ""
echo "Creating CloudWatch log group..."
aws logs create-log-group --log-group-name "/ecs/tourmanagement-web" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists"

# Handle load balancer
if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo ""
    echo "Creating Application Load Balancer and Target Group..."
    
    # Create ALB
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name tourmanagement-alb \
        --subnets "$SUBNET_1" "$SUBNET_2" \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || aws elbv2 describe-load-balancers \
            --names tourmanagement-alb \
            --region "$AWS_REGION" \
            --query 'LoadBalancers[0].LoadBalancerArn' \
            --output text)
    
    echo "ALB ARN: $ALB_ARN"
    
    # Create Target Group with ip target type for Fargate
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name tourmanagement-tg \
        --protocol HTTP \
        --port 8080 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-path /health \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null || aws elbv2 describe-target-groups \
            --names tourmanagement-tg \
            --region "$AWS_REGION" \
            --query 'TargetGroups[0].TargetGroupArn' \
            --output text)
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create listener
    aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" 2>/dev/null || echo "Listener already exists"
    
    # Get ALB DNS name
    ALB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$ALB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    echo "ALB DNS: $ALB_DNS"
else
    TARGET_GROUP_ARN=""
    echo "Skipping load balancer creation"
fi

# Replace placeholders in task definition
echo ""
echo "Preparing task definition..."
cp ecs/task-definition.json ecs/task-definition-resolved.json
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" ecs/task-definition-resolved.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" ecs/task-definition-resolved.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_HOST}}|$DB_HOST|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_PORT}}|$DB_PORT|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_NAME}}|$DB_NAME|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_USER}}|$DB_USER|g" ecs/task-definition-resolved.json
sed -i "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" ecs/task-definition-resolved.json

# Register task definition
echo ""
echo "Registering task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://ecs/task-definition-resolved.json \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

echo "Task definition registered: $TASK_DEF_ARN"

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
    # Remove loadBalancers section if no LB
    python3 -c "import json; f=open('ecs/service-definition-resolved.json','r'); d=json.load(f); f.close(); d.pop('loadBalancers',None); d.pop('healthCheckGracePeriodSeconds',None); f=open('ecs/service-definition-resolved.json','w'); json.dump(d,f,indent=2); f.close()"
fi

# Check if service exists
echo ""
echo "Checking if service exists..."
SERVICE_EXISTS=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services tourmanagement-web-service \
    --region "$AWS_REGION" \
    --query 'services[0].serviceName' \
    --output text 2>/dev/null)

if [ "$SERVICE_EXISTS" = "tourmanagement-web-service" ]; then
    echo "Service exists. Updating service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service tourmanagement-web-service \
        --task-definition "$TASK_DEF_ARN" \
        --region "$AWS_REGION"
else
    echo "Service does not exist. Creating service..."
    aws ecs create-service \
        --cli-input-json file://ecs/service-definition-resolved.json \
        --region "$AWS_REGION"
fi

# Wait for service stability
echo ""
echo "Waiting for service to become stable..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services tourmanagement-web-service \
    --region "$AWS_REGION"

# Verify deployment
echo ""
echo "====================================="
echo "Deployment Summary"
echo "====================================="
aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services tourmanagement-web-service \
    --region "$AWS_REGION" \
    --query 'services[0].[serviceName,status,runningCount,desiredCount]' \
    --output table

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo ""
    echo "Application URL: http://$ALB_DNS"
fi

echo ""
echo "CloudWatch Logs: /ecs/tourmanagement-web"
echo ""
echo "====================================="
echo "SUCCESS: Deployment completed!"
echo "====================================="

# Cleanup temporary files
rm -f ecs/task-definition-resolved.json ecs/service-definition-resolved.json