@echo off
setlocal enabledelayedexpansion

echo ===================================================
echo TourManagement AWS ECS Fargate Deployment Script
echo ===================================================
echo.

set PROJECT_NAME=tourmanagement
set TASK_FAMILY=!PROJECT_NAME!-task
set SERVICE_NAME=!PROJECT_NAME!-service
set CONTAINER_NAME=!PROJECT_NAME!-app
set LOG_GROUP=/ecs/!PROJECT_NAME!-app

echo === AWS Configuration ===
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS cluster name (e.g., my-ecs-cluster): "
echo.

echo Retrieving AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i
if "!ACCOUNT_ID!"==" " (
    echo ERROR: Failed to retrieve AWS Account ID. Please check your AWS credentials.
    exit /b 1
)
echo AWS Account ID: !ACCOUNT_ID!
echo.

echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo Cluster does not exist. Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
    echo ECS cluster created successfully!
)
echo.

echo === Network Configuration ===
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNET_IDS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "
echo.

for /f "tokens=1,2 delims=," %%a in ("!SUBNET_IDS!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

echo === Docker Image Configuration ===
set /p IMAGE_URI="Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest): "
echo.

echo === Application Configuration ===
set /p DB_HOST="Enter database host (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): "
set /p DB_PORT="Enter database port (default: 5432): "
if "!DB_PORT!"=="" set DB_PORT=5432
set /p DB_NAME="Enter database name (default: tourmanagementdb): "
if "!DB_NAME!"=="" set DB_NAME=tourmanagementdb
set /p DB_USER="Enter database username: "
set /p DB_PASSWORD="Enter database password: "
set /p REDIS_CONNECTION_STRING="Enter Redis connection string (e.g., redis.abc123.cache.amazonaws.com:6379) [optional]: "
echo.

set /p NEED_LB="Do you need a load balancer for this service? (y/n): "
echo.

if /i "!NEED_LB!"=="y" (
    echo Creating Application Load Balancer and Target Group...
    
    set TG_NAME=!PROJECT_NAME!-tg
    for /f "delims=" %%i in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-path /health --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    
    if "!TARGET_GROUP_ARN!"=="" (
        for /f "delims=" %%i in ('aws elbv2 describe-target-groups --names !TG_NAME! --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%i
    )
    
    if "!TARGET_GROUP_ARN!"=="" (
        echo ERROR: Failed to create or find target group.
        exit /b 1
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    echo.
) else (
    echo Skipping load balancer configuration.
    set TARGET_GROUP_ARN=
    echo.
)

echo Creating CloudWatch log group...
aws logs create-log-group --log-group-name !LOG_GROUP! --region !AWS_REGION! 2>nul
echo.

echo Preparing task definition...
copy ecs\task-definition.json %TEMP%\task-definition.json >nul

powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_HOST}}', '!DB_HOST!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_PORT}}', '!DB_PORT!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_NAME}}', '!DB_NAME!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_USER}}', '!DB_USER!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{DB_PASSWORD}}', '!DB_PASSWORD!' | Set-Content %TEMP%\task-definition.json"
powershell -Command "(Get-Content %TEMP%\task-definition.json) -replace '{{REDIS_CONNECTION_STRING}}', '!REDIS_CONNECTION_STRING!' | Set-Content %TEMP%\task-definition.json"

echo Registering task definition...
for /f "delims=" %%i in ('aws ecs register-task-definition --cli-input-json file://%TEMP%/task-definition.json --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%i

if "!TASK_DEF_ARN!"=="" (
    echo ERROR: Failed to register task definition.
    exit /b 1
)

echo Task definition registered: !TASK_DEF_ARN!
echo.

echo Preparing service definition...
copy ecs\service-definition.json %TEMP%\service-definition.json >nul

powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content %TEMP%\service-definition.json"
powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{SUBNET_1}}', '!SUBNET_1!' | Set-Content %TEMP%\service-definition.json"
powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{SUBNET_2}}', '!SUBNET_2!' | Set-Content %TEMP%\service-definition.json"
powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content %TEMP%\service-definition.json"

if not "!TARGET_GROUP_ARN!"=="" (
    powershell -Command "(Get-Content %TEMP%\service-definition.json) -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content %TEMP%\service-definition.json"
) else (
    powershell -Command "$content = Get-Content %TEMP%\service-definition.json | Out-String; $content = $content -replace '(?s)\s*\"loadBalancers\":\s*\[.*?\],?', ''; $content = $content -replace '\s*\"healthCheckGracePeriodSeconds\":\s*\d+,?', ''; $content | Set-Content %TEMP%\service-definition.json"
)

echo Checking if ECS service exists...
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].serviceName" --output text 2^>nul') do set EXISTING_SERVICE=%%i

if "!EXISTING_SERVICE!"=="!SERVICE_NAME!" (
    echo Service exists. Updating service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --force-new-deployment --region !AWS_REGION! >nul
    echo Service update initiated.
) else (
    echo Service does not exist. Creating service...
    aws ecs create-service --cli-input-json file://%TEMP%/service-definition.json --region !AWS_REGION! >nul
    echo Service created.
)
echo.

echo Waiting for service to become stable (this may take a few minutes)...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

echo Service is stable!
echo.

echo Verifying deployment...
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].runningCount" --output text') do set RUNNING_COUNT=%%i

echo Running tasks: !RUNNING_COUNT!
echo.

if not "!TARGET_GROUP_ARN!"=="" (
    for /f "delims=" %%i in ('aws elbv2 describe-target-groups --target-group-arns !TARGET_GROUP_ARN! --region !AWS_REGION! --query "TargetGroups[0].LoadBalancerArns[0]" --output text 2^>nul') do set LB_ARN=%%i
    
    if not "!LB_ARN!"=="" (
        for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns !LB_ARN! --region !AWS_REGION! --query "LoadBalancers[0].DNSName" --output text 2^>nul') do set LB_DNS=%%i
        
        if not "!LB_DNS!"=="" (
            echo Load Balancer DNS: http://!LB_DNS!
            echo Health Check: http://!LB_DNS!/health
            echo.
        )
    )
)

echo ===================================================
echo Deployment completed successfully!
echo ===================================================
echo.
echo Service Name: !SERVICE_NAME!
echo Cluster: !CLUSTER_NAME!
echo Region: !AWS_REGION!
echo CloudWatch Logs: !LOG_GROUP!
echo.
echo To view logs:
echo aws logs tail !LOG_GROUP! --follow --region !AWS_REGION!
echo.
echo To check service status:
echo aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo.

del %TEMP%\task-definition.json %TEMP%\service-definition.json 2>nul

endlocal