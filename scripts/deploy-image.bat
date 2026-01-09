@echo off
setlocal enabledelayedexpansion

echo ========================================
echo AWS ECS Fargate Deployment Script
echo tour-management-cont
echo ========================================
echo.

REM Configuration
set PROJECT_NAME=tour-management-cont
set SERVICE_NAME=%PROJECT_NAME%-service
set TASK_FAMILY=%PROJECT_NAME%-task

REM Prompt for AWS configuration
set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS Cluster Name (e.g., my-ecs-cluster): "
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNET_IDS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "
set /p IMAGE_URI="Enter Docker Image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management-cont:latest): "

echo.
echo Getting AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i
echo Account ID: !ACCOUNT_ID!

echo.
echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters "!CLUSTER_NAME!" --region "!AWS_REGION!" >nul 2>&1
if !ERRORLEVEL! neq 0 (
  echo Cluster does not exist. Creating cluster: !CLUSTER_NAME!
  aws ecs create-cluster --cluster-name "!CLUSTER_NAME!" --region "!AWS_REGION!"
  echo Cluster created successfully.
)

echo.
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
  echo.
  echo === Creating Application Load Balancer ===
  
  set ALB_NAME=!PROJECT_NAME!-alb
  echo Creating Application Load Balancer: !ALB_NAME!
  
  REM Parse subnets
  set SUBNET_LIST=!SUBNET_IDS:,= !
  
  for /f "delims=" %%a in ('aws elbv2 create-load-balancer --name "!ALB_NAME!" --subnets !SUBNET_LIST! --security-groups "!SECURITY_GROUP!" --scheme internet-facing --type application --ip-address-type ipv4 --region "!AWS_REGION!" --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%a
  
  if "!ALB_ARN!"=="" (
    echo ALB may already exist. Fetching existing ALB...
    for /f "delims=" %%a in ('aws elbv2 describe-load-balancers --names "!ALB_NAME!" --region "!AWS_REGION!" --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%a
  )
  
  if "!ALB_ARN!"=="" (
    echo Error: Failed to create or find Application Load Balancer
    exit /b 1
  )
  
  echo ALB ARN: !ALB_ARN!
  
  REM Get ALB DNS
  for /f "delims=" %%d in ('aws elbv2 describe-load-balancers --load-balancer-arns "!ALB_ARN!" --region "!AWS_REGION!" --query "LoadBalancers[0].DNSName" --output text') do set ALB_DNS=%%d
  
  REM Create Target Group
  set TG_NAME=!PROJECT_NAME!-tg
  echo Creating Target Group: !TG_NAME!
  
  for /f "delims=" %%t in ('aws elbv2 create-target-group --name "!TG_NAME!" --protocol HTTP --port 8080 --vpc-id "!VPC_ID!" --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path "/health" --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region "!AWS_REGION!" --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%t
  
  if "!TARGET_GROUP_ARN!"=="" (
    echo Target Group may already exist. Fetching existing Target Group...
    for /f "delims=" %%t in ('aws elbv2 describe-target-groups --names "!TG_NAME!" --region "!AWS_REGION!" --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%t
  )
  
  if "!TARGET_GROUP_ARN!"=="" (
    echo Error: Failed to create or find Target Group
    exit /b 1
  )
  
  echo Target Group ARN: !TARGET_GROUP_ARN!
  
  REM Create Listener
  echo Creating ALB Listener...
  aws elbv2 create-listener --load-balancer-arn "!ALB_ARN!" --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn="!TARGET_GROUP_ARN!" --region "!AWS_REGION!" >nul 2>&1
  
  echo Load Balancer setup complete.
  echo.
) else (
  echo Skipping load balancer creation.
  set TARGET_GROUP_ARN=
)

echo Preparing ECS task definition...
for /f "tokens=1,2 delims=," %%a in ("!SUBNET_IDS!") do (
  set SUBNET_1=%%a
  set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

REM Replace placeholders in task definition
copy ecs\task-definition.json ecs\task-definition-deploy.json >nul
powershell -Command "(Get-Content ecs\task-definition-deploy.json) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content ecs\task-definition-deploy.json"
powershell -Command "(Get-Content ecs\task-definition-deploy.json) -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content ecs\task-definition-deploy.json"
powershell -Command "(Get-Content ecs\task-definition-deploy.json) -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content ecs\task-definition-deploy.json"

echo Registering ECS task definition...
for /f "delims=" %%t in ('aws ecs register-task-definition --cli-input-json file://ecs/task-definition-deploy.json --region "!AWS_REGION!" --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%t

echo Task Definition ARN: !TASK_DEF_ARN!

REM Prepare service definition
copy ecs\service-definition.json ecs\service-definition-deploy.json >nul
powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content ecs\service-definition-deploy.json"
powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{SUBNET_1}}', '!SUBNET_1!' | Set-Content ecs\service-definition-deploy.json"
powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{SUBNET_2}}', '!SUBNET_2!' | Set-Content ecs\service-definition-deploy.json"
powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content ecs\service-definition-deploy.json"

if not "!TARGET_GROUP_ARN!"=="" (
  powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content ecs\service-definition-deploy.json"
) else (
  powershell -Command "$json = Get-Content ecs\service-definition-deploy.json | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content ecs\service-definition-deploy.json"
)

echo.
echo Checking if service exists...
for /f "delims=" %%s in ('aws ecs describe-services --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!" --query "services[0].serviceName" --output text 2^>nul') do set SERVICE_EXISTS=%%s

if "!SERVICE_EXISTS!"=="None" (
  echo Service does not exist. Creating new service...
  aws ecs create-service --cli-input-json file://ecs/service-definition-deploy.json --region "!AWS_REGION!"
  echo Service created successfully.
) else (
  echo Service exists. Updating service...
  aws ecs update-service --cluster "!CLUSTER_NAME!" --service "!SERVICE_NAME!" --task-definition "!TASK_DEF_ARN!" --region "!AWS_REGION!" --force-new-deployment
  echo Service updated successfully.
)

echo.
echo Waiting for service to become stable...
aws ecs wait services-stable --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!"

echo.
echo ========================================
echo Deployment Complete!
echo ========================================
echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Task Definition: !TASK_DEF_ARN!

if not "!ALB_DNS!"=="" (
  echo Load Balancer DNS: !ALB_DNS!
  echo Application URL: http://!ALB_DNS!
)

echo CloudWatch Logs: /ecs/%PROJECT_NAME%
echo.
echo To view service status:
echo aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo.
echo To view tasks:
echo aws ecs list-tasks --cluster !CLUSTER_NAME! --service-name !SERVICE_NAME! --region !AWS_REGION!
echo.

endlocal