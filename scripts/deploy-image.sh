#!/bin/bash
set -e
set -o pipefail

echo "========================================"
echo "AWS ECS Fargate Deployment Script"
echo "========================================"
echo ""

# Configuration
PROJECT_NAME="tourmanagement"
TASK_FAMILY="${PROJECT_NAME}-task"
SERVICE_NAME="${PROJECT_NAME}-service"

# Prompt for deployment configuration
read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS Cluster Name (e.g., my-ecs-cluster): " CLUSTER_NAME
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNETS_INPUT
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP
read -p "Enter Docker Image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): " IMAGE_URI

# Database configuration
read -p "Enter Database Server (e.g., mydb.region.rds.amazonaws.com): " DB_SERVER
read -p "Enter Database Name: " DB_NAME
read -p "Enter Database User: " DB_USER
read -sp "Enter Database Password: " DB_PASSWORD
echo ""

# Parse subnets
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNETS_INPUT"
SUBNET_1=$(echo "${SUBNET_ARRAY[0]}" | xargs)
SUBNET_2=$(echo "${SUBNET_ARRAY[1]:-${SUBNET_ARRAY[0]}}" | xargs)

echo ""
echo "========================================"
echo "Configuration Summary"
echo "========================================"
echo "Region: $AWS_REGION"
echo "Cluster: $CLUSTER_NAME"
echo "VPC: $VPC_ID"
echo "Subnets: $SUBNET_1, $SUBNET_2"
echo "Security Group: $SECURITY_GROUP"
echo "Image: $IMAGE_URI"
echo "Database Server: $DB_SERVER"
echo ""

# Get AWS Account ID
echo "Getting AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "Account ID: $ACCOUNT_ID"
echo ""

# Check if cluster exists, create if not
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" --query 'clusters[0].clusterName' --output text 2>/dev/null | grep -q "$CLUSTER_NAME" || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
    echo "Cluster created successfully."
}
echo ""

# Load balancer configuration
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB
echo ""

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo "========================================"
    echo "Creating Application Load Balancer"
    echo "========================================"
    
    LB_NAME="${PROJECT_NAME}-alb"
    TG_NAME="${PROJECT_NAME}-tg"
    
    # Create ALB
    echo "Creating Application Load Balancer..."
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name "$LB_NAME" \
        --subnets "$SUBNET_1" "$SUBNET_2" \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || aws elbv2 describe-load-balancers --names "$LB_NAME" --region "$AWS_REGION" --query 'LoadBalancers[0].LoadBalancerArn' --output text)
    
    echo "ALB ARN: $ALB_ARN"
    
    # Get ALB DNS
    ALB_DNS=$(aws elbv2 describe-load-balancers --load-balancer-arns "$ALB_ARN" --region "$AWS_REGION" --query 'LoadBalancers[0].DNSName' --output text)
    
    # Create Target Group with ip target type for Fargate
    echo "Creating Target Group..."
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port 8080 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-protocol HTTP \
        --health-check-path /health \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null || aws elbv2 describe-target-groups --names "$TG_NAME" --region "$AWS_REGION" --query 'TargetGroups[0].TargetGroupArn' --output text)
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create Listener
    echo "Creating ALB Listener..."
    aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" 2>/dev/null || echo "Listener already exists"
    
    echo "Load balancer setup complete."
    echo ""
    
    # Update service definition with load balancer
    sed -i.bak "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" ecs/service-definition.json
else
    echo "Skipping load balancer configuration."
    # Remove load balancer section from service definition
    if [ -f ecs/service-definition.json ]; then
        python3 -c "
import json
import sys
with open('ecs/service-definition.json', 'r') as f:
    data = json.load(f)
if 'loadBalancers' in data:
    del data['loadBalancers']
if 'healthCheckGracePeriodSeconds' in data:
    del data['healthCheckGracePeriodSeconds']
with open('ecs/service-definition.json', 'w') as f:
    json.dump(data, f, indent=2)
" 2>/dev/null || {
            # Fallback if python not available
            cp ecs/service-definition.json ecs/service-definition.json.bak
            grep -v '"loadBalancers"\|"targetGroupArn"\|"healthCheckGracePeriodSeconds"' ecs/service-definition.json.bak > ecs/service-definition.json || true
        }
    fi
    echo ""
fi

# Replace placeholders in task definition
echo "Preparing task definition..."
cp ecs/task-definition.json ecs/task-definition.json.tmp
sed -i.bak "s|{{IMAGE_URI}}|$IMAGE_URI|g" ecs/task-definition.json.tmp
sed -i.bak "s|{{AWS_REGION}}|$AWS_REGION|g" ecs/task-definition.json.tmp
sed -i.bak "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" ecs/task-definition.json.tmp
sed -i.bak "s|{{DB_SERVER}}|$DB_SERVER|g" ecs/task-definition.json.tmp
sed -i.bak "s|{{DB_NAME}}|$DB_NAME|g" ecs/task-definition.json.tmp
sed -i.bak "s|{{DB_USER}}|$DB_USER|g" ecs/task-definition.json.tmp
sed -i.bak "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" ecs/task-definition.json.tmp

# Register task definition
echo "Registering ECS task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://ecs/task-definition.json.tmp \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

echo "Task Definition ARN: $TASK_DEF_ARN"
echo ""

# Replace placeholders in service definition
echo "Preparing service definition..."
cp ecs/service-definition.json ecs/service-definition.json.tmp
sed -i.bak "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" ecs/service-definition.json.tmp
sed -i.bak "s|{{SUBNET_1}}|$SUBNET_1|g" ecs/service-definition.json.tmp
sed -i.bak "s|{{SUBNET_2}}|$SUBNET_2|g" ecs/service-definition.json.tmp
sed -i.bak "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" ecs/service-definition.json.tmp

# Check if service exists
echo "Checking if service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].serviceName' \
    --output text 2>/dev/null || echo "None")

if [ "$EXISTING_SERVICE" = "None" ] || [ "$EXISTING_SERVICE" = "" ]; then
    echo "Service does not exist. Creating new service..."
    aws ecs create-service \
        --cli-input-json file://ecs/service-definition.json.tmp \
        --region "$AWS_REGION"
    echo "Service created successfully."
else
    echo "Service exists. Updating service with new task definition..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --region "$AWS_REGION" \
        --force-new-deployment
    echo "Service updated successfully."
fi

echo ""
echo "Waiting for service to stabilize..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

echo ""
echo "========================================"
echo "Deployment Completed Successfully!"
echo "========================================"

# Get service details
RUNNING_COUNT=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].runningCount' \
    --output text)

echo "Service: $SERVICE_NAME"
echo "Running Tasks: $RUNNING_COUNT"
echo "Task Definition: $TASK_DEF_ARN"

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo "Load Balancer DNS: $ALB_DNS"
    echo "Application URL: http://$ALB_DNS"
fi

echo "CloudWatch Logs: /ecs/$PROJECT_NAME"
echo ""
echo "Monitor your deployment:"
echo "  aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo "  aws logs tail /ecs/$PROJECT_NAME --follow --region $AWS_REGION"
echo ""

# Cleanup temporary files
rm -f ecs/task-definition.json.tmp ecs/task-definition.json.tmp.bak
rm -f ecs/service-definition.json.tmp ecs/service-definition.json.tmp.bak

echo "Deployment complete!"