@echo off
setlocal enabledelayedexpansion

echo ========================================
echo AWS ECS Fargate Deployment Script
echo ========================================
echo.

set PROJECT_NAME=tourmanagement
set SERVICE_NAME=%PROJECT_NAME%-service
set TASK_FAMILY=%PROJECT_NAME%-task

set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS cluster name (e.g., my-ecs-cluster): "

echo.
echo Retrieving AWS Account ID...
for /f "delims=" %%a in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%a
echo Account ID: !ACCOUNT_ID!

echo.
echo === Network Configuration ===
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNETS_INPUT="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "

for /f "tokens=1,2 delims=," %%a in ("!SUBNETS_INPUT!") do (
  set SUBNET_1=%%a
  set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

echo.
set /p IMAGE_URI="Enter Docker image URI: "

echo.
echo === Database Configuration ===
set /p DB_SERVER="Enter DB Server: "
set /p DB_NAME="Enter DB Name (default: TourManagementDB): "
if "!DB_NAME!"=="" set DB_NAME=TourManagementDB
set /p DB_USER="Enter DB User (default: admin): "
if "!DB_USER!"=="" set DB_USER=admin
set /p DB_PASSWORD="Enter DB Password: "
set /p REDIS_CONNECTION="Enter Redis Connection String (optional): "

echo.
echo Checking ECS cluster...
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
  echo Cluster does not exist. Creating cluster...
  aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
)

echo.
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
  echo.
  echo Creating Application Load Balancer and Target Group...
  
  set ALB_NAME=%PROJECT_NAME%-alb
  echo Creating ALB: !ALB_NAME!
  
  for /f "delims=" %%a in ('aws elbv2 create-load-balancer --name !ALB_NAME! --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text 2^>nul') do set ALB_ARN=%%a
  
  if "!ALB_ARN!"=="" (
    for /f "delims=" %%a in ('aws elbv2 describe-load-balancers --names !ALB_NAME! --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text') do set ALB_ARN=%%a
  )
  
  echo Waiting for ALB to become active...
  aws elbv2 wait load-balancer-available --load-balancer-arns !ALB_ARN! --region !AWS_REGION!
  
  set TG_NAME=%PROJECT_NAME%-tg
  echo Creating Target Group: !TG_NAME!
  
  for /f "delims=" %%a in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-path "/health" --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text 2^>nul') do set TARGET_GROUP_ARN=%%a
  
  if "!TARGET_GROUP_ARN!"=="" (
    for /f "delims=" %%a in ('aws elbv2 describe-target-groups --names !TG_NAME! --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text') do set TARGET_GROUP_ARN=%%a
  )
  
  echo Creating ALB listener...
  aws elbv2 create-listener --load-balancer-arn !ALB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! --region !AWS_REGION! >nul 2>&1
  
  for /f "delims=" %%a in ('aws elbv2 describe-load-balancers --load-balancer-arns !ALB_ARN! --region !AWS_REGION! --query "LoadBalancers[0].DNSName" --output text') do set ALB_DNS=%%a
  
  echo Load Balancer DNS: !ALB_DNS!
) else (
  set TARGET_GROUP_ARN=
)

echo.
echo Creating CloudWatch log group...
aws logs create-log-group --log-group-name "/ecs/%PROJECT_NAME%" --region !AWS_REGION! 2>nul

echo.
echo Preparing task definition...
copy ecs\task-definition.json ecs\task-definition-resolved.json >nul

powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{IMAGE_URI}}','!IMAGE_URI!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{AWS_REGION}}','!AWS_REGION!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{ACCOUNT_ID}}','!ACCOUNT_ID!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_SERVER}}','!DB_SERVER!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_NAME}}','!DB_NAME!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_USER}}','!DB_USER!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{DB_PASSWORD}}','!DB_PASSWORD!' | Set-Content ecs\task-definition-resolved.json"
powershell -Command "(Get-Content ecs\task-definition-resolved.json) -replace '{{REDIS_CONNECTION}}','!REDIS_CONNECTION!' | Set-Content ecs\task-definition-resolved.json"

echo Registering task definition...
for /f "delims=" %%a in ('aws ecs register-task-definition --cli-input-json file://ecs/task-definition-resolved.json --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%a

echo Task Definition ARN: !TASK_DEF_ARN!

echo.
echo Preparing service definition...
copy ecs\service-definition.json ecs\service-definition-resolved.json >nul

powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{CLUSTER_NAME}}','!CLUSTER_NAME!' | Set-Content ecs\service-definition-resolved.json"
powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{SUBNET_1}}','!SUBNET_1!' | Set-Content ecs\service-definition-resolved.json"
powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{SUBNET_2}}','!SUBNET_2!' | Set-Content ecs\service-definition-resolved.json"
powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{SECURITY_GROUP}}','!SECURITY_GROUP!' | Set-Content ecs\service-definition-resolved.json"

if /i "!NEED_LB!"=="y" (
  powershell -Command "(Get-Content ecs\service-definition-resolved.json) -replace '{{TARGET_GROUP_ARN}}','!TARGET_GROUP_ARN!' | Set-Content ecs\service-definition-resolved.json"
) else (
  powershell -Command "$json = Get-Content ecs\service-definition-resolved.json | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content ecs\service-definition-resolved.json"
)

echo.
echo Checking if service exists...
for /f "delims=" %%a in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].serviceName" --output text 2^>nul') do set SERVICE_EXISTS=%%a

if "!SERVICE_EXISTS!"=="!SERVICE_NAME!" (
  echo Service exists. Updating service...
  aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --force-new-deployment --region !AWS_REGION!
) else (
  echo Service does not exist. Creating service...
  aws ecs create-service --cli-input-json file://ecs/service-definition-resolved.json --region !AWS_REGION!
)

echo.
echo Waiting for service to become stable...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

echo.
echo Verifying deployment...
for /f "delims=" %%a in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[0].runningCount" --output text') do set RUNNING_COUNT=%%a

echo.
echo ========================================
echo DEPLOYMENT SUCCESSFUL!
echo ========================================
echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Running Tasks: !RUNNING_COUNT!
echo CloudWatch Logs: /ecs/%PROJECT_NAME%

if /i "!NEED_LB!"=="y" (
  echo Load Balancer URL: http://!ALB_DNS!
  echo.
  echo Access your application at: http://!ALB_DNS!
)

echo.
echo To view logs:
echo aws logs tail /ecs/%PROJECT_NAME% --follow --region !AWS_REGION!
echo.
echo To view service details:
echo aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo.

del /f /q ecs\task-definition-resolved.json ecs\service-definition-resolved.json 2>nul

echo Deployment complete!

endlocal