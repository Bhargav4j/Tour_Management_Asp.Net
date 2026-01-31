#!/bin/bash
set -e
set -o pipefail

echo "============================================"
echo "  TourManagement AWS ECS Fargate Deployment"
echo "============================================"
echo ""

# Project configuration
PROJECT_NAME="tourmanagement"
TASK_FAMILY="${PROJECT_NAME}-task"
SERVICE_NAME="${PROJECT_NAME}-service"
CONTAINER_NAME="${PROJECT_NAME}"
CONTAINER_PORT=8080

# Prompt for AWS configuration
echo "=== AWS Configuration ==="
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
export AWS_DEFAULT_REGION="$AWS_REGION"

read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME

echo ""
echo "=== Network Configuration ==="
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNET_IDS
IFS=',' read -ra SUBNETS <<< "$SUBNET_IDS"
SUBNET_1="${SUBNETS[0]}"
SUBNET_2="${SUBNETS[1]:-$SUBNET_1}"

read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP

echo ""
echo "=== Docker Image Configuration ==="
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): " IMAGE_URI

echo ""
echo "=== Database Configuration ==="
read -p "Enter PostgreSQL connection string: " DB_CONNECTION_STRING

echo ""
read -p "Enter Redis connection string (optional, press Enter to skip): " REDIS_CONNECTION

echo ""
echo "Getting AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "Account ID: $ACCOUNT_ID"

echo ""
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
}

echo ""
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo ""
    echo "=== Creating Application Load Balancer ==="
    
    LB_NAME="${PROJECT_NAME}-alb"
    TG_NAME="${PROJECT_NAME}-tg"
    
    echo "Creating Application Load Balancer: $LB_NAME"
    LB_ARN=$(aws elbv2 create-load-balancer \
        --name "$LB_NAME" \
        --subnets "$SUBNET_1" "$SUBNET_2" \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || aws elbv2 describe-load-balancers --names "$LB_NAME" --query 'LoadBalancers[0].LoadBalancerArn' --output text)
    
    echo "Load Balancer ARN: $LB_ARN"
    
    echo "Creating Target Group: $TG_NAME"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port "$CONTAINER_PORT" \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-protocol HTTP \
        --health-check-path "/health" \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null || aws elbv2 describe-target-groups --names "$TG_NAME" --query 'TargetGroups[0].TargetGroupArn' --output text)
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    echo "Creating Listener..."
    LISTENER_ARN=$(aws elbv2 create-listener \
        --load-balancer-arn "$LB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" \
        --query 'Listeners[0].ListenerArn' \
        --output text 2>/dev/null || aws elbv2 describe-listeners --load-balancer-arn "$LB_ARN" --query 'Listeners[0].ListenerArn' --output text)
    
    echo "Listener ARN: $LISTENER_ARN"
    
    LB_DNS=$(aws elbv2 describe-load-balancers --load-balancer-arns "$LB_ARN" --query 'LoadBalancers[0].DNSName' --output text)
    echo "Load Balancer DNS: $LB_DNS"
fi

echo ""
echo "=== Preparing ECS Task Definition ==="

TASK_DEF_FILE="ecs/task-definition.json"

if [ ! -f "$TASK_DEF_FILE" ]; then
    echo "ERROR: Task definition file not found: $TASK_DEF_FILE"
    exit 1
fi

cp "$TASK_DEF_FILE" "${TASK_DEF_FILE}.tmp"

sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" "${TASK_DEF_FILE}.tmp"
sed -i "s|{{AWS_REGION}}|${AWS_REGION}|g" "${TASK_DEF_FILE}.tmp"
sed -i "s|{{ACCOUNT_ID}}|${ACCOUNT_ID}|g" "${TASK_DEF_FILE}.tmp"
sed -i "s|{{DB_CONNECTION_STRING}}|${DB_CONNECTION_STRING}|g" "${TASK_DEF_FILE}.tmp"
sed -i "s|{{REDIS_CONNECTION}}|${REDIS_CONNECTION}|g" "${TASK_DEF_FILE}.tmp"

echo "Registering ECS task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://"${TASK_DEF_FILE}.tmp" \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

echo "Task Definition ARN: $TASK_DEF_ARN"

rm "${TASK_DEF_FILE}.tmp"

echo ""
echo "=== CloudWatch Logs Configuration ==="
LOG_GROUP="/ecs/${PROJECT_NAME}"
echo "Ensuring CloudWatch log group exists: $LOG_GROUP"
aws logs create-log-group --log-group-name "$LOG_GROUP" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists"
aws logs put-retention-policy --log-group-name "$LOG_GROUP" --retention-in-days 7 --region "$AWS_REGION" 2>/dev/null || true

echo ""
echo "=== Preparing ECS Service Definition ==="

SERVICE_DEF_FILE="ecs/service-definition.json"

if [ ! -f "$SERVICE_DEF_FILE" ]; then
    echo "ERROR: Service definition file not found: $SERVICE_DEF_FILE"
    exit 1
fi

cp "$SERVICE_DEF_FILE" "${SERVICE_DEF_FILE}.tmp"

sed -i "s|{{CLUSTER_NAME}}|${CLUSTER_NAME}|g" "${SERVICE_DEF_FILE}.tmp"
sed -i "s|{{SUBNET_1}}|${SUBNET_1}|g" "${SERVICE_DEF_FILE}.tmp"
sed -i "s|{{SUBNET_2}}|${SUBNET_2}|g" "${SERVICE_DEF_FILE}.tmp"
sed -i "s|{{SECURITY_GROUP}}|${SECURITY_GROUP}|g" "${SERVICE_DEF_FILE}.tmp"

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    sed -i "s|{{TARGET_GROUP_ARN}}|${TARGET_GROUP_ARN}|g" "${SERVICE_DEF_FILE}.tmp"
else
    # Remove loadBalancers section if no LB needed
    sed -i '/"loadBalancers":/,/],/d' "${SERVICE_DEF_FILE}.tmp"
    sed -i '/"healthCheckGracePeriodSeconds":/d' "${SERVICE_DEF_FILE}.tmp"
fi

echo "Checking if ECS service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text)

if [ -z "$EXISTING_SERVICE" ] || [ "$EXISTING_SERVICE" = "None" ]; then
    echo "Service does not exist. Creating new ECS service..."
    aws ecs create-service \
        --cli-input-json file://"${SERVICE_DEF_FILE}.tmp" \
        --region "$AWS_REGION"
else
    echo "Service exists. Updating ECS service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --force-new-deployment \
        --region "$AWS_REGION"
fi

rm "${SERVICE_DEF_FILE}.tmp"

echo ""
echo "Waiting for service to become stable..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

echo ""
echo "=== Deployment Verification ==="
aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].{ServiceName:serviceName,Status:status,DesiredCount:desiredCount,RunningCount:runningCount}' \
    --output table

echo ""
echo "============================================"
echo "  Deployment Completed Successfully!"
echo "============================================"
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Task Definition: $TASK_DEF_ARN"
echo "CloudWatch Logs: $LOG_GROUP"
if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo "Load Balancer URL: http://$LB_DNS"
fi
echo ""
echo "To view logs:"
echo "  aws logs tail $LOG_GROUP --follow --region $AWS_REGION"
echo ""