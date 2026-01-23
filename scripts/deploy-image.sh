#!/bin/bash
set -e
set -o pipefail

echo "=========================================="
echo "AWS ECS Fargate Deployment Script"
echo "=========================================="
echo ""

# Project configuration
PROJECT_NAME="tmscontainerize"
TASK_FAMILY="tmscontainerize-task"
SERVICE_NAME="tmscontainerize-service"

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
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): " IMAGE_URI

echo ""
echo "Getting AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "Account ID: $ACCOUNT_ID"

echo ""
echo "=== Load Balancer Configuration ==="
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [ "$NEED_LB" = "y" ] || [ "$NEED_LB" = "Y" ]; then
    echo ""
    echo "Creating Application Load Balancer..."
    
    LB_NAME="${PROJECT_NAME}-alb"
    TG_NAME="${PROJECT_NAME}-tg"
    
    # Create load balancer
    echo "Creating ALB: $LB_NAME"
    LB_ARN=$(aws elbv2 create-load-balancer \
        --name "$LB_NAME" \
        --subnets "$SUBNET_1" "$SUBNET_2" \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$LB_ARN" ]; then
        echo "Load balancer may already exist, retrieving ARN..."
        LB_ARN=$(aws elbv2 describe-load-balancers \
            --names "$LB_NAME" \
            --query 'LoadBalancers[0].LoadBalancerArn' \
            --output text 2>/dev/null || echo "")
    fi
    
    if [ -z "$LB_ARN" ]; then
        echo "ERROR: Failed to create or find load balancer"
        exit 1
    fi
    
    echo "Load Balancer ARN: $LB_ARN"
    
    # Create target group with ip target type for Fargate
    echo "Creating Target Group: $TG_NAME"
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
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        echo "Target group may already exist, retrieving ARN..."
        TARGET_GROUP_ARN=$(aws elbv2 describe-target-groups \
            --names "$TG_NAME" \
            --query 'TargetGroups[0].TargetGroupArn' \
            --output text 2>/dev/null || echo "")
    fi
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        echo "ERROR: Failed to create or find target group"
        exit 1
    fi
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create listener
    echo "Creating ALB Listener..."
    aws elbv2 create-listener \
        --load-balancer-arn "$LB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        >/dev/null 2>&1 || echo "Listener may already exist"
    
    # Get load balancer DNS name
    LB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$LB_ARN" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    echo "Load Balancer DNS: $LB_DNS"
    USE_LB=true
else
    USE_LB=false
    echo "Skipping load balancer configuration"
fi

echo ""
echo "=== Checking ECS Cluster ==="
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
}

echo "Cluster $CLUSTER_NAME is ready"

echo ""
echo "=== Creating CloudWatch Log Group ==="
LOG_GROUP="/ecs/$PROJECT_NAME"
aws logs create-log-group --log-group-name "$LOG_GROUP" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists"

echo ""
echo "=== Preparing Task Definition ==="

# Create temporary task definition with substitutions
cp ecs/task-definition.json /tmp/task-definition-temp.json

sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" /tmp/task-definition-temp.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" /tmp/task-definition-temp.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" /tmp/task-definition-temp.json

echo "Task definition prepared"

echo ""
echo "=== Registering Task Definition ==="
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file:///tmp/task-definition-temp.json \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

if [ -z "$TASK_DEF_ARN" ]; then
    echo "ERROR: Failed to register task definition"
    exit 1
fi

echo "Task definition registered: $TASK_DEF_ARN"

echo ""
echo "=== Preparing Service Definition ==="

# Create temporary service definition with substitutions
cp ecs/service-definition.json /tmp/service-definition-temp.json

sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" /tmp/service-definition-temp.json
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" /tmp/service-definition-temp.json
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" /tmp/service-definition-temp.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" /tmp/service-definition-temp.json

if [ "$USE_LB" = false ]; then
    # Remove loadBalancers section if not using LB
    jq 'del(.loadBalancers, .healthCheckGracePeriodSeconds)' /tmp/service-definition-temp.json > /tmp/service-definition-temp2.json
    mv /tmp/service-definition-temp2.json /tmp/service-definition-temp.json
else
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" /tmp/service-definition-temp.json
fi

echo "Service definition prepared"

echo ""
echo "=== Checking if Service Exists ==="
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text 2>/dev/null || echo "")

if [ -z "$EXISTING_SERVICE" ] || [ "$EXISTING_SERVICE" = "None" ]; then
    echo "Service does not exist. Creating new service..."
    
    aws ecs create-service \
        --cli-input-json file:///tmp/service-definition-temp.json \
        --region "$AWS_REGION" \
        >/dev/null
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create service"
        exit 1
    fi
    
    echo "Service created successfully"
else
    echo "Service exists. Updating service..."
    
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --desired-count 2 \
        --force-new-deployment \
        --region "$AWS_REGION" \
        >/dev/null
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to update service"
        exit 1
    fi
    
    echo "Service updated successfully"
fi

echo ""
echo "=== Waiting for Service Stability ==="
echo "This may take several minutes..."

aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

if [ $? -eq 0 ]; then
    echo "Service is stable"
else
    echo "WARNING: Service stability check timed out or failed"
    echo "Check the ECS console for service status"
fi

echo ""
echo "=== Deployment Status ==="

RUNNING_COUNT=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].runningCount' \
    --output text)

DESIRED_COUNT=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].desiredCount' \
    --output text)

echo "Running tasks: $RUNNING_COUNT / $DESIRED_COUNT"

echo ""
echo "=========================================="
echo "Deployment Completed Successfully"
echo "=========================================="
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Task Definition: $TASK_DEF_ARN"
echo "Region: $AWS_REGION"

if [ "$USE_LB" = true ]; then
    echo "Load Balancer DNS: http://$LB_DNS"
    echo ""
    echo "Note: Wait a few minutes for the load balancer to become active"
    echo "Access your application at: http://$LB_DNS"
fi

echo ""
echo "CloudWatch Logs: $LOG_GROUP"
echo ""
echo "View logs with:"
echo "  aws logs tail $LOG_GROUP --follow --region $AWS_REGION"
echo ""
echo "Monitor service with:"
echo "  aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo ""

# Cleanup temporary files
rm -f /tmp/task-definition-temp.json /tmp/service-definition-temp.json

echo "Deployment script completed"