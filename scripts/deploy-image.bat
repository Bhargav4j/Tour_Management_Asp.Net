@echo off
setlocal enabledelayedexpansion

echo ============================================
echo   TourManagement AWS ECS Fargate Deployment
echo ============================================
echo.

REM Project configuration
set PROJECT_NAME=tourmanagement
set TASK_FAMILY=!PROJECT_NAME!-task
set SERVICE_NAME=!PROJECT_NAME!-service
set CONTAINER_NAME=!PROJECT_NAME!
set CONTAINER_PORT=8080

REM Prompt for AWS configuration
echo === AWS Configuration ===
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set AWS_DEFAULT_REGION=!AWS_REGION!

set /p CLUSTER_NAME="Enter ECS cluster name (e.g., my-ecs-cluster): "

echo.
echo === Network Configuration ===
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNET_IDS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "

for /f "tokens=1,2 delims=," %%a in ("!SUBNET_IDS!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "

echo.
echo === Docker Image Configuration ===
set /p IMAGE_URI="Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): "

echo.
echo === Database Configuration ===
set /p DB_CONNECTION_STRING="Enter PostgreSQL connection string: "

echo.
set /p REDIS_CONNECTION="Enter Redis connection string (optional, press Enter to skip): "

echo.
echo Getting AWS Account ID...
for /f "delims=" %%a in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%a
echo Account ID: !ACCOUNT_ID!

echo.
echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Cluster does not exist. Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
)

echo.
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo.
    echo === Creating Application Load Balancer ===
    
    set LB_NAME=!PROJECT_NAME!-alb
    set TG_NAME=!PROJECT_NAME!-tg
    
    echo Creating Application Load Balancer: !LB_NAME!
    for /f "delims=" %%a in ('aws elbv2 create-load-balancer --name !LB_NAME! --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --ip-address-type ipv4 --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set LB_ARN=%%a
    if "!LB_ARN!"=="" (
        for /f "delims=" %%a in ('aws elbv2 describe-load-balancers --names !LB_NAME! --query "LoadBalancers[0].LoadBalancerArn" --output text') do set LB_ARN=%%a
    )
    
    echo Load Balancer ARN: !LB_ARN!
    
    echo Creating Target Group: !TG_NAME!
    for /f "delims=" %%a in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port !CONTAINER_PORT! --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path "/health" --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%a
    if "!TARGET_GROUP_ARN!"=="" (
        for /f "delims=" %%a in ('aws elbv2 describe-target-groups --names !TG_NAME! --query "TargetGroups[0].TargetGroupArn" --output text') do set TARGET_GROUP_ARN=%%a
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    
    echo Creating Listener...
    for /f "delims=" %%a in ('aws elbv2 create-listener --load-balancer-arn !LB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! --region !AWS_REGION! --query "Listeners[0].ListenerArn" --output text 2^>nul') do set LISTENER_ARN=%%a
    if "!LISTENER_ARN!"=="" (
        for /f "delims=" %%a in ('aws elbv2 describe-listeners --load-balancer-arn !LB_ARN! --query "Listeners[0].ListenerArn" --output text') do set LISTENER_ARN=%%a
    )
    
    echo Listener ARN: !LISTENER_ARN!
    
    for /f "delims=" %%a in ('aws elbv2 describe-load-balancers --load-balancer-arns !LB_ARN! --query "LoadBalancers[0].DNSName" --output text') do set LB_DNS=%%a
    echo Load Balancer DNS: !LB_DNS!
)

echo.
echo === Preparing ECS Task Definition ===

set TASK_DEF_FILE=ecs\task-definition.json

if not exist "!TASK_DEF_FILE!" (
    echo ERROR: Task definition file not found: !TASK_DEF_FILE!
    exit /b 1
)

copy "!TASK_DEF_FILE!" "!TASK_DEF_FILE!.tmp" >nul

powershell -command "(Get-Content '!TASK_DEF_FILE!.tmp') -replace '{{IMAGE_URI}}', '!IMAGE_URI!' -replace '{{AWS_REGION}}', '!AWS_REGION!' -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' -replace '{{DB_CONNECTION_STRING}}', '!DB_CONNECTION_STRING!' -replace '{{REDIS_CONNECTION}}', '!REDIS_CONNECTION!' | Set-Content '!TASK_DEF_FILE!.tmp'"

echo Registering ECS task definition...
for /f "delims=" %%a in ('aws ecs register-task-definition --cli-input-json file://!TASK_DEF_FILE!.tmp --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%a

echo Task Definition ARN: !TASK_DEF_ARN!

del "!TASK_DEF_FILE!.tmp"

echo.
echo === CloudWatch Logs Configuration ===
set LOG_GROUP=/ecs/!PROJECT_NAME!
echo Ensuring CloudWatch log group exists: !LOG_GROUP!
aws logs create-log-group --log-group-name !LOG_GROUP! --region !AWS_REGION! >nul 2>&1
aws logs put-retention-policy --log-group-name !LOG_GROUP! --retention-in-days 7 --region !AWS_REGION! >nul 2>&1

echo.
echo === Preparing ECS Service Definition ===

set SERVICE_DEF_FILE=ecs\service-definition.json

if not exist "!SERVICE_DEF_FILE!" (
    echo ERROR: Service definition file not found: !SERVICE_DEF_FILE!
    exit /b 1
)

copy "!SERVICE_DEF_FILE!" "!SERVICE_DEF_FILE!.tmp" >nul

if /i "!NEED_LB!"=="y" (
    powershell -command "(Get-Content '!SERVICE_DEF_FILE!.tmp') -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' -replace '{{SUBNET_1}}', '!SUBNET_1!' -replace '{{SUBNET_2}}', '!SUBNET_2!' -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content '!SERVICE_DEF_FILE!.tmp'"
) else (
    powershell -command "$content = Get-Content '!SERVICE_DEF_FILE!.tmp' | Out-String | ConvertFrom-Json; $content.PSObject.Properties.Remove('loadBalancers'); $content.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $content | ConvertTo-Json -Depth 10 | Set-Content '!SERVICE_DEF_FILE!.tmp'"
    powershell -command "(Get-Content '!SERVICE_DEF_FILE!.tmp') -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' -replace '{{SUBNET_1}}', '!SUBNET_1!' -replace '{{SUBNET_2}}', '!SUBNET_2!' -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content '!SERVICE_DEF_FILE!.tmp'"
)

echo Checking if ECS service exists...
for /f "delims=" %%a in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[?status==`ACTIVE`].serviceName" --output text') do set EXISTING_SERVICE=%%a

if "!EXISTING_SERVICE!"=="" (
    echo Service does not exist. Creating new ECS service...
    aws ecs create-service --cli-input-json file://!SERVICE_DEF_FILE!.tmp --region !AWS_REGION!
) else (
    echo Service exists. Updating ECS service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --force-new-deployment --region !AWS_REGION!
)

del "!SERVICE_DEF_FILE!.tmp"

echo.
echo Waiting for service to become stable...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

echo.
echo === Deployment Verification ===
aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].{ServiceName:serviceName,Status:status,DesiredCount:desiredCount,RunningCount:runningCount}" --output table

echo.
echo ============================================
echo   Deployment Completed Successfully!
echo ============================================
echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Task Definition: !TASK_DEF_ARN!
echo CloudWatch Logs: !LOG_GROUP!
if /i "!NEED_LB!"=="y" (
    echo Load Balancer URL: http://!LB_DNS!
)
echo.
echo To view logs:
echo   aws logs tail !LOG_GROUP! --follow --region !AWS_REGION!
echo.

endlocal