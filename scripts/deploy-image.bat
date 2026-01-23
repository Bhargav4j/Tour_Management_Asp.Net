@echo off
setlocal enabledelayedexpansion

echo ==========================================
echo AWS ECS Fargate Deployment Script
echo ==========================================
echo.

set PROJECT_NAME=tmscontainerize
set TASK_FAMILY=tmscontainerize-task
set SERVICE_NAME=tmscontainerize-service

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
set /p IMAGE_URI="Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): "

echo.
echo Getting AWS Account ID...
for /f "tokens=*" %%a in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%a
echo Account ID: !ACCOUNT_ID!

echo.
echo === Load Balancer Configuration ===
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo.
    echo Creating Application Load Balancer...
    
    set LB_NAME=!PROJECT_NAME!-alb
    set TG_NAME=!PROJECT_NAME!-tg
    
    echo Creating ALB: !LB_NAME!
    for /f "tokens=*" %%a in ('aws elbv2 create-load-balancer --name !LB_NAME! --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --ip-address-type ipv4 --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set LB_ARN=%%a
    
    if "!LB_ARN!"=="" (
        echo Load balancer may already exist, retrieving ARN...
        for /f "tokens=*" %%a in ('aws elbv2 describe-load-balancers --names !LB_NAME! --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set LB_ARN=%%a
    )
    
    if "!LB_ARN!"=="" (
        echo ERROR: Failed to create or find load balancer
        exit /b 1
    )
    
    echo Load Balancer ARN: !LB_ARN!
    
    echo Creating Target Group: !TG_NAME!
    for /f "tokens=*" %%a in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path /health --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%a
    
    if "!TARGET_GROUP_ARN!"=="" (
        echo Target group may already exist, retrieving ARN...
        for /f "tokens=*" %%a in ('aws elbv2 describe-target-groups --names !TG_NAME! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%a
    )
    
    if "!TARGET_GROUP_ARN!"=="" (
        echo ERROR: Failed to create or find target group
        exit /b 1
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    
    echo Creating ALB Listener...
    aws elbv2 create-listener --load-balancer-arn !LB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! >nul 2>&1
    
    for /f "tokens=*" %%a in ('aws elbv2 describe-load-balancers --load-balancer-arns !LB_ARN! --query "LoadBalancers[0].DNSName" --output text') do set LB_DNS=%%a
    echo Load Balancer DNS: !LB_DNS!
    
    set USE_LB=true
) else (
    set USE_LB=false
    echo Skipping load balancer configuration
)

echo.
echo === Checking ECS Cluster ===
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Cluster does not exist. Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
)

echo Cluster !CLUSTER_NAME! is ready

echo.
echo === Creating CloudWatch Log Group ===
set LOG_GROUP=/ecs/!PROJECT_NAME!
aws logs create-log-group --log-group-name !LOG_GROUP! --region !AWS_REGION! 2>nul

echo.
echo === Preparing Task Definition ===

copy ecs\task-definition.json %TEMP%\task-definition-temp.json >nul

powershell -Command "(Get-Content '%TEMP%\task-definition-temp.json') -replace '{{IMAGE_URI}}','!IMAGE_URI!' -replace '{{AWS_REGION}}','!AWS_REGION!' -replace '{{ACCOUNT_ID}}','!ACCOUNT_ID!' | Set-Content '%TEMP%\task-definition-temp.json'"

echo Task definition prepared

echo.
echo === Registering Task Definition ===
for /f "tokens=*" %%a in ('aws ecs register-task-definition --cli-input-json file://%TEMP%/task-definition-temp.json --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%a

if "!TASK_DEF_ARN!"=="" (
    echo ERROR: Failed to register task definition
    exit /b 1
)

echo Task definition registered: !TASK_DEF_ARN!

echo.
echo === Preparing Service Definition ===

copy ecs\service-definition.json %TEMP%\service-definition-temp.json >nul

powershell -Command "(Get-Content '%TEMP%\service-definition-temp.json') -replace '{{CLUSTER_NAME}}','!CLUSTER_NAME!' -replace '{{SUBNET_1}}','!SUBNET_1!' -replace '{{SUBNET_2}}','!SUBNET_2!' -replace '{{SECURITY_GROUP}}','!SECURITY_GROUP!' -replace '{{TARGET_GROUP_ARN}}','!TARGET_GROUP_ARN!' | Set-Content '%TEMP%\service-definition-temp.json'"

if "!USE_LB!"=="false" (
    powershell -Command "$json = Get-Content '%TEMP%\service-definition-temp.json' | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content '%TEMP%\service-definition-temp.json'"
)

echo Service definition prepared

echo.
echo === Checking if Service Exists ===
for /f "tokens=*" %%a in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[?status==`ACTIVE`].serviceName" --output text 2^>nul') do set EXISTING_SERVICE=%%a

if "!EXISTING_SERVICE!"=="" (
    echo Service does not exist. Creating new service...
    
    aws ecs create-service --cli-input-json file://%TEMP%/service-definition-temp.json --region !AWS_REGION! >nul
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create service
        exit /b 1
    )
    
    echo Service created successfully
) else (
    echo Service exists. Updating service...
    
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --desired-count 2 --force-new-deployment --region !AWS_REGION! >nul
    
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to update service
        exit /b 1
    )
    
    echo Service updated successfully
)

echo.
echo === Waiting for Service Stability ===
echo This may take several minutes...

aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

if !ERRORLEVEL! equ 0 (
    echo Service is stable
) else (
    echo WARNING: Service stability check timed out or failed
    echo Check the ECS console for service status
)

echo.
echo === Deployment Status ===

for /f "tokens=*" %%a in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].runningCount" --output text') do set RUNNING_COUNT=%%a

for /f "tokens=*" %%a in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].desiredCount" --output text') do set DESIRED_COUNT=%%a

echo Running tasks: !RUNNING_COUNT! / !DESIRED_COUNT!

echo.
echo ==========================================
echo Deployment Completed Successfully
echo ==========================================
echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Task Definition: !TASK_DEF_ARN!
echo Region: !AWS_REGION!

if "!USE_LB!"=="true" (
    echo Load Balancer DNS: http://!LB_DNS!
    echo.
    echo Note: Wait a few minutes for the load balancer to become active
    echo Access your application at: http://!LB_DNS!
)

echo.
echo CloudWatch Logs: !LOG_GROUP!
echo.
echo View logs with:
echo   aws logs tail !LOG_GROUP! --follow --region !AWS_REGION!
echo.
echo Monitor service with:
echo   aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo.

del %TEMP%\task-definition-temp.json 2>nul
del %TEMP%\service-definition-temp.json 2>nul

echo Deployment script completed

endlocal