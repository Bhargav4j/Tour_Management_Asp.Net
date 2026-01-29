#!/bin/bash
set -e
set -o pipefail

echo "================================================"
echo "   Tour Management - ECS Fargate Deployment"
echo "================================================"
echo ""

# Validate AWS CLI is installed
if ! command -v aws &> /dev/null; then
    echo "ERROR: AWS CLI is not installed. Please install it first."
    exit 1
fi

# Get AWS credentials
echo "--- AWS Configuration ---"
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., tour-management-cluster): " CLUSTER_NAME
echo ""

# Get AWS Account ID
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to retrieve AWS Account ID. Check AWS credentials."
    exit 1
fi
echo "AWS Account ID: $ACCOUNT_ID"
echo ""

# Check/Create ECS Cluster
echo "Checking ECS cluster..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
}
echo "Cluster ready: $CLUSTER_NAME"
echo ""

# Network Configuration
echo "--- Network Configuration ---"
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNET_IDS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP
echo ""

# Convert comma-separated subnets to array format
IFS=',' read -ra SUBNETS <<< "$SUBNET_IDS"
SUBNET_1="${SUBNETS[0]}"
SUBNET_2="${SUBNETS[1]:-$SUBNET_1}"

# Image Configuration
echo "--- Container Image Configuration ---"
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
echo ""

# Database Configuration
echo "--- Database Configuration ---"
read -p "Enter Database Server (e.g., tour-db.abc123.us-east-1.rds.amazonaws.com): " DB_SERVER
read -p "Enter Database Name (default: tourdb): " DB_NAME
DB_NAME=${DB_NAME:-tourdb}
read -p "Enter Database User (default: admin): " DB_USER
DB_USER=${DB_USER:-admin}
read -sp "Enter Database Password: " DB_PASSWORD
echo ""
echo ""

# Load Balancer Configuration
echo "--- Load Balancer Configuration ---"
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [ "${NEED_LB,,}" == "y" ]; then
    echo "Creating Application Load Balancer..."
    
    # Create ALB
    ALB_NAME="tour-management-alb"
    echo "Creating ALB: $ALB_NAME"
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name "$ALB_NAME" \
        --subnets $SUBNET_1 $SUBNET_2 \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null)
    
    if [ $? -ne 0 ]; then
        echo "WARNING: ALB creation failed or already exists. Attempting to find existing ALB..."
        ALB_ARN=$(aws elbv2 describe-load-balancers \
            --names "$ALB_NAME" \
            --region "$AWS_REGION" \
            --query 'LoadBalancers[0].LoadBalancerArn' \
            --output text 2>/dev/null)
    fi
    
    echo "ALB ARN: $ALB_ARN"
    
    # Create Target Group
    TG_NAME="tour-management-tg"
    echo "Creating Target Group: $TG_NAME"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port 80 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-protocol HTTP \
        --health-check-path "/health.aspx" \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 10 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null)
    
    if [ $? -ne 0 ]; then
        echo "WARNING: Target Group creation failed or already exists. Attempting to find existing TG..."
        TARGET_GROUP_ARN=$(aws elbv2 describe-target-groups \
            --names "$TG_NAME" \
            --region "$AWS_REGION" \
            --query 'TargetGroups[0].TargetGroupArn' \
            --output text 2>/dev/null)
    fi
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create Listener
    echo "Creating ALB Listener..."
    aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" >/dev/null 2>&1 || echo "Listener may already exist"
    
    # Get ALB DNS Name
    ALB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$ALB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    echo "Load Balancer DNS: $ALB_DNS"
    echo ""
else
    TARGET_GROUP_ARN=""
    echo "Skipping load balancer configuration"
    echo ""
fi

# Update task definition JSON
echo "Updating task definition..."
cp ecs/task-definition.json ecs/task-definition-temp.json

sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" ecs/task-definition-temp.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" ecs/task-definition-temp.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" ecs/task-definition-temp.json
sed -i "s|{{DB_SERVER}}|$DB_SERVER|g" ecs/task-definition-temp.json
sed -i "s|{{DB_NAME}}|$DB_NAME|g" ecs/task-definition-temp.json
sed -i "s|{{DB_USER}}|$DB_USER|g" ecs/task-definition-temp.json
sed -i "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" ecs/task-definition-temp.json

# Register task definition
echo "Registering ECS task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://ecs/task-definition-temp.json \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

if [ $? -ne 0 ]; then
    echo "ERROR: Failed to register task definition"
    rm -f ecs/task-definition-temp.json
    exit 1
fi

echo "Task Definition ARN: $TASK_DEF_ARN"
rm -f ecs/task-definition-temp.json
echo ""

# Update service definition JSON
echo "Updating service definition..."
cp ecs/service-definition.json ecs/service-definition-temp.json

sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" ecs/service-definition-temp.json
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" ecs/service-definition-temp.json
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" ecs/service-definition-temp.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" ecs/service-definition-temp.json
sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" ecs/service-definition-temp.json

# Remove load balancer section if not needed
if [ "${NEED_LB,,}" != "y" ]; then
    echo "Removing load balancer configuration from service definition..."
    # Remove loadBalancers and healthCheckGracePeriodSeconds using jq
    if command -v jq &> /dev/null; then
        jq 'del(.loadBalancers, .healthCheckGracePeriodSeconds)' ecs/service-definition-temp.json > ecs/service-definition-temp2.json
        mv ecs/service-definition-temp2.json ecs/service-definition-temp.json
    fi
fi

# Check if service exists
echo "Checking if ECS service exists..."
SERVICE_NAME="tour-management-service"
SERVICE_EXISTS=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text)

if [ -z "$SERVICE_EXISTS" ]; then
    echo "Creating new ECS service..."
    aws ecs create-service \
        --cli-input-json file://ecs/service-definition-temp.json \
        --region "$AWS_REGION"
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create ECS service"
        rm -f ecs/service-definition-temp.json
        exit 1
    fi
else
    echo "Updating existing ECS service..."
    
    UPDATE_CMD="aws ecs update-service \
        --cluster $CLUSTER_NAME \
        --service $SERVICE_NAME \
        --task-definition $TASK_DEF_ARN \
        --force-new-deployment \
        --region $AWS_REGION"
    
    if [ "${NEED_LB,,}" == "y" ]; then
        UPDATE_CMD="$UPDATE_CMD --health-check-grace-period-seconds 300"
    fi
    
    eval $UPDATE_CMD
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to update ECS service"
        rm -f ecs/service-definition-temp.json
        exit 1
    fi
fi

rm -f ecs/service-definition-temp.json
echo ""

# Wait for service stability
echo "Waiting for service to stabilize (this may take several minutes)..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

if [ $? -ne 0 ]; then
    echo "WARNING: Service did not stabilize within the expected time"
else
    echo "Service is stable"
fi
echo ""

# Verify deployment
echo "================================================"
echo "Deployment Status"
echo "================================================"
aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].{ServiceName:serviceName,Status:status,RunningCount:runningCount,DesiredCount:desiredCount}' \
    --output table

echo ""
echo "================================================"
echo "Deployment Complete!"
echo "================================================"
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Task Definition: $TASK_DEF_ARN"
if [ "${NEED_LB,,}" == "y" ]; then
    echo "Load Balancer DNS: http://$ALB_DNS"
    echo "Health Check URL: http://$ALB_DNS/health.aspx"
fi
echo "CloudWatch Logs: /ecs/tour-management"
echo ""
echo "Monitor your deployment:"
echo "  aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo "  aws logs tail /ecs/tour-management --follow --region $AWS_REGION"
echo "================================================"
