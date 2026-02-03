@echo off
setlocal enabledelayedexpansion

echo =====================================
echo AWS ECS Fargate Deployment Script
echo =====================================
echo.

REM Prompt for AWS configuration
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS cluster name (e.g., tourmanagement-cluster): "
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNETS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "
set /p IMAGE_URI="Enter Docker image URI: "

echo.
echo Database Configuration:
set /p DB_HOST="Enter Database Host (e.g., db.example.com): "
set /p DB_PORT="Enter Database Port (default: 5432): "
if "!DB_PORT!"=="" set DB_PORT=5432
set /p DB_NAME="Enter Database Name (default: tourmanagementdb): "
if "!DB_NAME!"=="" set DB_NAME=tourmanagementdb
set /p DB_USER="Enter Database User (default: postgres): "
if "!DB_USER!"=="" set DB_USER=postgres
set /p DB_PASSWORD="Enter Database Password: "

echo.
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

REM Get AWS Account ID
echo.
echo Retrieving AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i
echo AWS Account ID: !ACCOUNT_ID!

REM Check if cluster exists
echo.
echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Cluster does not exist. Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
)

REM Parse subnets
for /f "tokens=1,2 delims=," %%a in ("!SUBNETS!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

REM Create CloudWatch log group
echo.
echo Creating CloudWatch log group...
aws logs create-log-group --log-group-name /ecs/tourmanagement-web --region !AWS_REGION! 2>nul

REM Handle load balancer
if /i "!NEED_LB!"=="y" (
    echo.
    echo Creating Application Load Balancer and Target Group...
    
    for /f "delims=" %%i in ('aws elbv2 create-load-balancer --name tourmanagement-alb --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --ip-address-type ipv4 --region !AWS_REGION! --query LoadBalancers[0].LoadBalancerArn --output text 2^>nul') do set ALB_ARN=%%i
    
    if "!ALB_ARN!"=="" (
        for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --names tourmanagement-alb --region !AWS_REGION! --query LoadBalancers[0].LoadBalancerArn --output text') do set ALB_ARN=%%i
    )
    
    echo ALB ARN: !ALB_ARN!
    
    for /f "delims=" %%i in ('aws elbv2 create-target-group --name tourmanagement-tg --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-path /health --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query TargetGroups[0].TargetGroupArn --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    
    if "!TARGET_GROUP_ARN!"=="" (
        for /f "delims=" %%i in ('aws elbv2 describe-target-groups --names tourmanagement-tg --region !AWS_REGION! --query TargetGroups[0].TargetGroupArn --output text') do set TARGET_GROUP_ARN=%%i
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    
    aws elbv2 create-listener --load-balancer-arn !ALB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! --region !AWS_REGION! 2>nul
    
    for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns !ALB_ARN! --region !AWS_REGION! --query LoadBalancers[0].DNSName --output text') do set ALB_DNS=%%i
    
    echo ALB DNS: !ALB_DNS!
) else (
    set TARGET_GROUP_ARN=
    echo Skipping load balancer creation
)

REM Prepare task definition
echo.
echo Preparing task definition...
copy ecs\task-definition.json ecs\task-definition-resolved.json >nul
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_HOST}}', '!DB_HOST!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_PORT}}', '!DB_PORT!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_NAME}}', '!DB_NAME!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_USER}}', '!DB_USER!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_PASSWORD}}', '!DB_PASSWORD!' | Set-Content ecs\task-definition-resolved.json"

REM Register task definition
echo.
echo Registering task definition...
for /f "delims=" %%i in ('aws ecs register-task-definition --cli-input-json file://ecs/task-definition-resolved.json --region !AWS_REGION! --query taskDefinition.taskDefinitionArn --output text') do set TASK_DEF_ARN=%%i

echo Task definition registered: !TASK_DEF_ARN!

REM Prepare service definition
echo.
echo Preparing service definition...
copy ecs\service-definition.json ecs\service-definition-resolved.json >nul
powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content ecs\service-definition-resolved.json"
powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{SUBNET_1}}', '!SUBNET_1!' | Set-Content ecs\service-definition-resolved.json"
powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{SUBNET_2}}', '!SUBNET_2!' | Set-Content ecs\service-definition-resolved.json"
powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content ecs\service-definition-resolved.json"

if /i "!NEED_LB!"=="y" (
    powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content ecs\service-definition-resolved.json"
) else (
    powershell -Command "$json = Get-Content ecs\service-definition-resolved.json | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content ecs\service-definition-resolved.json"
)

REM Check if service exists
echo.
echo Checking if service exists...
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services tourmanagement-web-service --region !AWS_REGION! --query services[0].serviceName --output text 2^>nul') do set SERVICE_EXISTS=%%i

if "!SERVICE_EXISTS!"=="tourmanagement-web-service" (
    echo Service exists. Updating service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service tourmanagement-web-service --task-definition !TASK_DEF_ARN! --region !AWS_REGION!
) else (
    echo Service does not exist. Creating service...
    aws ecs create-service --cli-input-json file://ecs/service-definition-resolved.json --region !AWS_REGION!
)

REM Wait for service stability
echo.
echo Waiting for service to become stable...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services tourmanagement-web-service --region !AWS_REGION!

REM Verify deployment
echo.
echo =====================================
echo Deployment Summary
echo =====================================
aws ecs describe-services --cluster !CLUSTER_NAME! --services tourmanagement-web-service --region !AWS_REGION! --query services[0].[serviceName,status,runningCount,desiredCount] --output table

if /i "!NEED_LB!"=="y" (
    echo.
    echo Application URL: http://!ALB_DNS!
)

echo.
echo CloudWatch Logs: /ecs/tourmanagement-web
echo.
echo =====================================
echo SUCCESS: Deployment completed!
echo =====================================

REM Cleanup
del /q ecs\task-definition-resolved.json ecs\service-definition-resolved.json 2>nul

endlocal