@echo off
setlocal enabledelayedexpansion

echo ============================================
echo   TourManagement - ECS Fargate Deployment
echo ============================================
echo.

REM Configuration
set PROJECT_NAME=tourmanagement
set TASK_FAMILY=!PROJECT_NAME!-task
set SERVICE_NAME=!PROJECT_NAME!-service

REM Prompt for AWS configuration
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set AWS_DEFAULT_REGION=!AWS_REGION!

set /p CLUSTER_NAME="Enter ECS cluster name (e.g., my-ecs-cluster): "

REM Get AWS Account ID
echo.
echo Retrieving AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i

if "!ACCOUNT_ID!"=="" (
    echo ERROR: Failed to retrieve AWS Account ID. Check AWS credentials.
    exit /b 1
)

echo AWS Account ID: !ACCOUNT_ID!

REM Check/Create ECS Cluster
echo.
echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Cluster does not exist. Creating ECS cluster !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
    echo Cluster created successfully
)

REM Network configuration
echo.
echo --- Network Configuration ---
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNETS_INPUT="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "

REM Parse subnets
for /f "tokens=1,2 delims=," %%a in ("!SUBNETS_INPUT!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

echo.
echo Network Configuration:
echo   VPC: !VPC_ID!
echo   Subnet 1: !SUBNET_1!
echo   Subnet 2: !SUBNET_2!
echo   Security Group: !SECURITY_GROUP!

REM Database configuration
echo.
echo --- Database Configuration ---
set /p DB_SERVER="Enter database server (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): "
set /p DB_NAME="Enter database name (default: TourManagementDb): "
if "!DB_NAME!"=="" set DB_NAME=TourManagementDb
set /p DB_USER="Enter database user (default: admin): "
if "!DB_USER!"=="" set DB_USER=admin
set /p DB_PASSWORD="Enter database password: "

REM Docker image configuration
echo.
set /p IMAGE_URI="Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): "

REM Load balancer configuration
echo.
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo.
    echo Creating Application Load Balancer and Target Group...
    
    set TG_NAME=!PROJECT_NAME!-tg
    
    REM Create target group
    for /f "delims=" %%i in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-path "/health" --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    
    if "!TARGET_GROUP_ARN!"=="" (
        for /f "delims=" %%i in ('aws elbv2 describe-target-groups --names !TG_NAME! --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text') do set TARGET_GROUP_ARN=%%i
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    
    REM Create ALB
    set ALB_NAME=!PROJECT_NAME!-alb
    for /f "delims=" %%i in ('aws elbv2 create-load-balancer --name !ALB_NAME! --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --ip-address-type ipv4 --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%i
    
    if "!ALB_ARN!"=="" (
        for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --names !ALB_NAME! --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text') do set ALB_ARN=%%i
    )
    
    echo Load Balancer ARN: !ALB_ARN!
    
    REM Create listener
    aws elbv2 create-listener --load-balancer-arn !ALB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! --region !AWS_REGION! >nul 2>&1
    
    REM Get ALB DNS
    for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns !ALB_ARN! --region !AWS_REGION! --query "LoadBalancers[0].DNSName" --output text') do set ALB_DNS=%%i
    
    echo Load Balancer DNS: !ALB_DNS!
) else (
    set TARGET_GROUP_ARN=
    echo Skipping load balancer configuration
)

REM Create CloudWatch Log Group
echo.
echo Creating CloudWatch Log Group...
set LOG_GROUP=/ecs/!PROJECT_NAME!
aws logs create-log-group --log-group-name !LOG_GROUP! --region !AWS_REGION! >nul 2>&1

REM Update task definition
echo.
echo Preparing ECS task definition...
copy ecs\task-definition.json ecs\task-definition-temp.json >nul

powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{IMAGE_URI}}','!IMAGE_URI!' -replace '{{AWS_REGION}}','!AWS_REGION!' -replace '{{ACCOUNT_ID}}','!ACCOUNT_ID!' -replace '{{DB_SERVER}}','!DB_SERVER!' -replace '{{DB_NAME}}','!DB_NAME!' -replace '{{DB_USER}}','!DB_USER!' -replace '{{DB_PASSWORD}}','!DB_PASSWORD!' | Set-Content ecs\task-definition-temp.json"

echo Registering task definition...
for /f "delims=" %%i in ('aws ecs register-task-definition --cli-input-json file://ecs/task-definition-temp.json --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%i

echo Task Definition ARN: !TASK_DEF_ARN!

REM Clean up temp file
del ecs\task-definition-temp.json >nul 2>&1

REM Update service definition
echo.
echo Preparing ECS service definition...
copy ecs\service-definition.json ecs\service-definition-temp.json >nul

powershell -Command "(Get-Content ecs\service-definition-temp.json) -replace '{{CLUSTER_NAME}}','!CLUSTER_NAME!' -replace '{{SUBNET_1}}','!SUBNET_1!' -replace '{{SUBNET_2}}','!SUBNET_2!' -replace '{{SECURITY_GROUP}}','!SECURITY_GROUP!' -replace '{{TARGET_GROUP_ARN}}','!TARGET_GROUP_ARN!' | Set-Content ecs\service-definition-temp.json"

REM Remove loadBalancers if not needed
if "!TARGET_GROUP_ARN!"=="" (
    powershell -Command "$json = Get-Content ecs\service-definition-temp.json | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content ecs\service-definition-temp.json"
)

REM Check if service exists
echo.
echo Checking if service exists...
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[?status=='ACTIVE'].serviceName" --output text 2^>nul') do set EXISTING_SERVICE=%%i

if "!EXISTING_SERVICE!"=="" (
    echo Service does not exist. Creating new service...
    aws ecs create-service --cli-input-json file://ecs/service-definition-temp.json --region !AWS_REGION!
) else (
    echo Service exists. Updating service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --desired-count 2 --region !AWS_REGION!
)

REM Clean up temp file
del ecs\service-definition-temp.json >nul 2>&1

echo.
echo Waiting for service to stabilize...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

REM Verify deployment
echo.
echo ============================================
echo   Deployment Completed Successfully!
echo ============================================
echo.
echo Deployment Details:
echo   Cluster: !CLUSTER_NAME!
echo   Service: !SERVICE_NAME!
echo   Task Definition: !TASK_DEF_ARN!
echo   Region: !AWS_REGION!

if not "!ALB_DNS!"=="" (
    echo   Application URL: http://!ALB_DNS!
    echo   Health Check: http://!ALB_DNS!/health
)

echo   CloudWatch Logs: !LOG_GROUP!
echo.

REM Display running tasks
echo Fetching running tasks...
aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].{Running:runningCount,Desired:desiredCount,Status:status}" --output table

echo.
echo To view logs use:
echo   aws logs tail !LOG_GROUP! --follow --region !AWS_REGION!
echo.

endlocal