@echo off
setlocal enabledelayedexpansion

echo ================================================
echo    Tour Management - ECS Fargate Deployment
echo ================================================
echo.

REM Validate AWS CLI
where aws >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo ERROR: AWS CLI is not installed. Please install it first.
    exit /b 1
)

echo --- AWS Configuration ---
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS cluster name (e.g., tour-management-cluster): "
echo.

echo Retrieving AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to retrieve AWS Account ID. Check AWS credentials.
    exit /b 1
)
echo AWS Account ID: !ACCOUNT_ID!
echo.

echo Checking ECS cluster...
aws ecs describe-clusters --clusters "!CLUSTER_NAME!" --region "!AWS_REGION!" >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name "!CLUSTER_NAME!" --region "!AWS_REGION!"
)
echo Cluster ready: !CLUSTER_NAME!
echo.

echo --- Network Configuration ---
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNET_IDS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "
echo.

REM Parse subnets
for /f "tokens=1,2 delims=," %%a in ("!SUBNET_IDS!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

echo --- Container Image Configuration ---
set /p IMAGE_URI="Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): "
echo.

echo --- Database Configuration ---
set /p DB_SERVER="Enter Database Server (e.g., tour-db.abc123.us-east-1.rds.amazonaws.com): "
set /p DB_NAME="Enter Database Name (default: tourdb): "
if "!DB_NAME!"=="" set DB_NAME=tourdb
set /p DB_USER="Enter Database User (default: admin): "
if "!DB_USER!"=="" set DB_USER=admin
set /p DB_PASSWORD="Enter Database Password: "
echo.

echo --- Load Balancer Configuration ---
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo Creating Application Load Balancer...
    
    set ALB_NAME=tour-management-alb
    echo Creating ALB: !ALB_NAME!
    for /f "delims=" %%i in ('aws elbv2 create-load-balancer --name "!ALB_NAME!" --subnets !SUBNET_1! !SUBNET_2! --security-groups "!SECURITY_GROUP!" --scheme internet-facing --type application --ip-address-type ipv4 --region "!AWS_REGION!" --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%i
    
    if !ERRORLEVEL! neq 0 (
        echo WARNING: ALB creation failed or already exists. Attempting to find existing ALB...
        for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --names "!ALB_NAME!" --region "!AWS_REGION!" --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%i
    )
    
    echo ALB ARN: !ALB_ARN!
    
    set TG_NAME=tour-management-tg
    echo Creating Target Group: !TG_NAME!
    for /f "delims=" %%i in ('aws elbv2 create-target-group --name "!TG_NAME!" --protocol HTTP --port 80 --vpc-id "!VPC_ID!" --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path "/health.aspx" --health-check-interval-seconds 30 --health-check-timeout-seconds 10 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region "!AWS_REGION!" --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    
    if !ERRORLEVEL! neq 0 (
        echo WARNING: Target Group creation failed or already exists. Attempting to find existing TG...
        for /f "delims=" %%i in ('aws elbv2 describe-target-groups --names "!TG_NAME!" --region "!AWS_REGION!" --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    
    echo Creating ALB Listener...
    aws elbv2 create-listener --load-balancer-arn "!ALB_ARN!" --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn="!TARGET_GROUP_ARN!" --region "!AWS_REGION!" >nul 2>&1
    
    for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns "!ALB_ARN!" --region "!AWS_REGION!" --query "LoadBalancers[0].DNSName" --output text') do set ALB_DNS=%%i
    
    echo Load Balancer DNS: !ALB_DNS!
    echo.
) else (
    set TARGET_GROUP_ARN=
    echo Skipping load balancer configuration
    echo.
)

echo Updating task definition...
copy ecs\task-definition.json ecs\task-definition-temp.json >nul

powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content ecs\task-definition-temp.json"
powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content ecs\task-definition-temp.json"
powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content ecs\task-definition-temp.json"
powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{DB_SERVER}}', '!DB_SERVER!' | Set-Content ecs\task-definition-temp.json"
powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{DB_NAME}}', '!DB_NAME!' | Set-Content ecs\task-definition-temp.json"
powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{DB_USER}}', '!DB_USER!' | Set-Content ecs\task-definition-temp.json"
powershell -Command "(Get-Content ecs\task-definition-temp.json) -replace '{{DB_PASSWORD}}', '!DB_PASSWORD!' | Set-Content ecs\task-definition-temp.json"

echo Registering ECS task definition...
for /f "delims=" %%i in ('aws ecs register-task-definition --cli-input-json file://ecs/task-definition-temp.json --region "!AWS_REGION!" --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%i

if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to register task definition
    del ecs\task-definition-temp.json
    exit /b 1
)

echo Task Definition ARN: !TASK_DEF_ARN!
del ecs\task-definition-temp.json
echo.

echo Updating service definition...
copy ecs\service-definition.json ecs\service-definition-temp.json >nul

powershell -Command "(Get-Content ecs\service-definition-temp.json) -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content ecs\service-definition-temp.json"
powershell -Command "(Get-Content ecs\service-definition-temp.json) -replace '{{SUBNET_1}}', '!SUBNET_1!' | Set-Content ecs\service-definition-temp.json"
powershell -Command "(Get-Content ecs\service-definition-temp.json) -replace '{{SUBNET_2}}', '!SUBNET_2!' | Set-Content ecs\service-definition-temp.json"
powershell -Command "(Get-Content ecs\service-definition-temp.json) -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content ecs\service-definition-temp.json"
powershell -Command "(Get-Content ecs\service-definition-temp.json) -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content ecs\service-definition-temp.json"

if /i not "!NEED_LB!"=="y" (
    echo Removing load balancer configuration from service definition...
    where jq >nul 2>&1
    if !ERRORLEVEL! equ 0 (
        jq "del(.loadBalancers, .healthCheckGracePeriodSeconds)" ecs\service-definition-temp.json > ecs\service-definition-temp2.json
        move /y ecs\service-definition-temp2.json ecs\service-definition-temp.json >nul
    )
)

echo Checking if ECS service exists...
set SERVICE_NAME=tour-management-service
for /f "delims=" %%i in ('aws ecs describe-services --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!" --query "services[?status==`ACTIVE`].serviceName" --output text') do set SERVICE_EXISTS=%%i

if "!SERVICE_EXISTS!"=="" (
    echo Creating new ECS service...
    aws ecs create-service --cli-input-json file://ecs/service-definition-temp.json --region "!AWS_REGION!"
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECS service
        del ecs\service-definition-temp.json
        exit /b 1
    )
) else (
    echo Updating existing ECS service...
    
    if /i "!NEED_LB!"=="y" (
        aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --force-new-deployment --health-check-grace-period-seconds 300 --region !AWS_REGION!
    ) else (
        aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --force-new-deployment --region !AWS_REGION!
    )
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to update ECS service
        del ecs\service-definition-temp.json
        exit /b 1
    )
)

del ecs\service-definition-temp.json
echo.

echo Waiting for service to stabilize (this may take several minutes)...
aws ecs wait services-stable --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!"

if !ERRORLEVEL! neq 0 (
    echo WARNING: Service did not stabilize within the expected time
) else (
    echo Service is stable
)
echo.

echo ================================================
echo Deployment Status
echo ================================================
aws ecs describe-services --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!" --query "services[0].{ServiceName:serviceName,Status:status,RunningCount:runningCount,DesiredCount:desiredCount}" --output table

echo.
echo ================================================
echo Deployment Complete!
echo ================================================
echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Task Definition: !TASK_DEF_ARN!
if /i "!NEED_LB!"=="y" (
    echo Load Balancer DNS: http://!ALB_DNS!
    echo Health Check URL: http://!ALB_DNS!/health.aspx
)
echo CloudWatch Logs: /ecs/tour-management
echo.
echo Monitor your deployment:
echo   aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo   aws logs tail /ecs/tour-management --follow --region !AWS_REGION!
echo ================================================

endlocal
