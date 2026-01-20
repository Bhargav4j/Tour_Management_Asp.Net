#!/bin/bash
set -e
set -o pipefail

echo "====================================="
echo "AWS ECS Fargate Deployment Script"
echo "====================================="
echo ""

# Deployment configuration
echo "=== Deployment Configuration ==="
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNET_IDS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): " IMAGE_URI

echo ""
echo "=== AWS Account Configuration ==="
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "AWS Account ID: $ACCOUNT_ID"

# Parse subnet IDs
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNET_IDS"
SUBNET_1=${SUBNET_ARRAY[0]}
SUBNET_2=${SUBNET_ARRAY[1]:-$SUBNET_1}

echo ""
echo "=== ECS Cluster Setup ==="
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
    echo "Cluster created successfully"
}

echo ""
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [ "$NEED_LB" = "y" ] || [ "$NEED_LB" = "Y" ]; then
    echo ""
    echo "=== Creating Application Load Balancer ==="
    
    ALB_NAME="tourmgmt-alb-$(date +%s)"
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
        --output text)
    
    echo "ALB created: $ALB_ARN"
    
    echo "Creating Target Group with IP target type for Fargate..."
    TG_NAME="tourmgmt-tg-$(date +%s)"
    
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
        --output text)
    
    echo "Target Group created: $TARGET_GROUP_ARN"
    
    echo "Creating ALB Listener..."
    aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" >/dev/null
    
    echo "ALB Listener created successfully"
    
    ALB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$ALB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    echo "Load Balancer DNS: $ALB_DNS"
else
    echo "Skipping load balancer creation"
    TARGET_GROUP_ARN=""
fi

echo ""
echo "=== CloudWatch Logs Setup ==="
LOG_GROUP="/ecs/tourmgmt-container"
echo "Checking CloudWatch log group: $LOG_GROUP"
aws logs describe-log-groups --log-group-name-prefix "$LOG_GROUP" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Creating CloudWatch log group: $LOG_GROUP"
    aws logs create-log-group --log-group-name "$LOG_GROUP" --region "$AWS_REGION"
    echo "Log group created successfully"
}

echo ""
echo "=== Preparing ECS Task Definition ==="
cp ecs/task-definition.json ecs/task-definition-deploy.json

sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" ecs/task-definition-deploy.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" ecs/task-definition-deploy.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" ecs/task-definition-deploy.json

echo "Registering ECS task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://ecs/task-definition-deploy.json \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

echo "Task definition registered: $TASK_DEF_ARN"

echo ""
echo "=== Preparing ECS Service Definition ==="
cp ecs/service-definition.json ecs/service-definition-deploy.json

sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" ecs/service-definition-deploy.json
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" ecs/service-definition-deploy.json
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" ecs/service-definition-deploy.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" ecs/service-definition-deploy.json

if [ -z "$TARGET_GROUP_ARN" ]; then
    echo "Removing load balancer configuration from service definition..."
    jq 'del(.loadBalancers) | del(.healthCheckGracePeriodSeconds)' ecs/service-definition-deploy.json > ecs/service-definition-deploy-temp.json
    mv ecs/service-definition-deploy-temp.json ecs/service-definition-deploy.json
else
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" ecs/service-definition-deploy.json
fi

SERVICE_NAME="tourmgmt-container-service"

echo ""
echo "=== Checking if ECS service exists ==="
SERVICE_EXISTS=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text)

if [ -z "$SERVICE_EXISTS" ]; then
    echo "Service does not exist. Creating new ECS service..."
    aws ecs create-service \
        --cli-input-json file://ecs/service-definition-deploy.json \
        --region "$AWS_REGION" >/dev/null
    echo "Service created successfully"
else
    echo "Service exists. Updating ECS service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --region "$AWS_REGION" >/dev/null
    echo "Service updated successfully"
fi

echo ""
echo "=== Waiting for service stability ==="
echo "This may take several minutes..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

echo ""
echo "=== Deployment Status ==="
RUNNING_COUNT=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].runningCount' \
    --output text)

echo "Service: $SERVICE_NAME"
echo "Running tasks: $RUNNING_COUNT"
echo "CloudWatch log group: $LOG_GROUP"

if [ -n "$TARGET_GROUP_ARN" ]; then
    echo "Load Balancer DNS: http://$ALB_DNS"
    echo ""
    echo "Access your application at: http://$ALB_DNS"
fi

echo ""
echo "====================================="
echo "Deployment completed successfully!"
echo "====================================="

rm -f ecs/task-definition-deploy.json ecs/service-definition-deploy.json

echo ""
echo "Troubleshooting:"
echo "- View logs: aws logs tail $LOG_GROUP --follow --region $AWS_REGION"
echo "- Describe service: aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo "- List tasks: aws ecs list-tasks --cluster $CLUSTER_NAME --service-name $SERVICE_NAME --region $AWS_REGION"