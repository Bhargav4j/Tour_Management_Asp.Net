@echo off
setlocal enabledelayedexpansion

echo =====================================
echo AWS ECS Fargate Deployment Script
echo =====================================
echo.

echo === Deployment Configuration ===
set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS cluster name (e.g., my-ecs-cluster): "
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNET_IDS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "
set /p IMAGE_URI="Enter Docker image URI: "

echo.
echo === AWS Account Configuration ===
for /f "tokens=*" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i
echo AWS Account ID: !ACCOUNT_ID!

for /f "tokens=1,2 delims=," %%a in ("!SUBNET_IDS!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"==" " set SUBNET_2=!SUBNET_1!

echo.
echo === ECS Cluster Setup ===
echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters "!CLUSTER_NAME!" --region "!AWS_REGION!" >nul 2>&1

if !ERRORLEVEL! neq 0 (
    echo Cluster does not exist. Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name "!CLUSTER_NAME!" --region "!AWS_REGION!"
    echo Cluster created successfully
)

echo.
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo.
    echo === Creating Application Load Balancer ===
    
    for /f "tokens=*" %%i in ('powershell -Command "Get-Date -UFormat %%s"') do set TIMESTAMP=%%i
    set ALB_NAME=tourmgmt-alb-!TIMESTAMP!
    echo Creating ALB: !ALB_NAME!
    
    for /f "tokens=*" %%i in ('aws elbv2 create-load-balancer --name "!ALB_NAME!" --subnets !SUBNET_1! !SUBNET_2! --security-groups "!SECURITY_GROUP!" --scheme internet-facing --type application --ip-address-type ipv4 --region "!AWS_REGION!" --query "LoadBalancers[0].LoadBalancerArn" --output text') do set ALB_ARN=%%i
    
    echo ALB created: !ALB_ARN!
    
    echo Creating Target Group with IP target type for Fargate...
    set TG_NAME=tourmgmt-tg-!TIMESTAMP!
    
    for /f "tokens=*" %%i in ('aws elbv2 create-target-group --name "!TG_NAME!" --protocol HTTP --port 8080 --vpc-id "!VPC_ID!" --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path /health --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region "!AWS_REGION!" --query "TargetGroups[0].TargetGroupArn" --output text') do set TARGET_GROUP_ARN=%%i
    
    echo Target Group created: !TARGET_GROUP_ARN!
    
    echo Creating ALB Listener...
    aws elbv2 create-listener --load-balancer-arn "!ALB_ARN!" --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn="!TARGET_GROUP_ARN!" --region "!AWS_REGION!" >nul
    
    echo ALB Listener created successfully
    
    for /f "tokens=*" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns "!ALB_ARN!" --region "!AWS_REGION!" --query "LoadBalancers[0].DNSName" --output text') do set ALB_DNS=%%i
    
    echo Load Balancer DNS: !ALB_DNS!
) else (
    echo Skipping load balancer creation
    set TARGET_GROUP_ARN=
)

echo.
echo === CloudWatch Logs Setup ===
set LOG_GROUP=/ecs/tourmgmt-container
echo Checking CloudWatch log group: !LOG_GROUP!
aws logs describe-log-groups --log-group-name-prefix "!LOG_GROUP!" --region "!AWS_REGION!" >nul 2>&1

if !ERRORLEVEL! neq 0 (
    echo Creating CloudWatch log group: !LOG_GROUP!
    aws logs create-log-group --log-group-name "!LOG_GROUP!" --region "!AWS_REGION!"
    echo Log group created successfully
)

echo.
echo === Preparing ECS Task Definition ===
copy ecs\task-definition.json ecs\task-definition-deploy.json >nul

powershell -Command "(Get-Content ecs\task-definition-deploy.json) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content ecs\task-definition-deploy.json"
powershell -Command "(Get-Content ecs\task-definition-deploy.json) -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content ecs\task-definition-deploy.json"
powershell -Command "(Get-Content ecs\task-definition-deploy.json) -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content ecs\task-definition-deploy.json"

echo Registering ECS task definition...
for /f "tokens=*" %%i in ('aws ecs register-task-definition --cli-input-json file://ecs/task-definition-deploy.json --region "!AWS_REGION!" --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%i

echo Task definition registered: !TASK_DEF_ARN!

echo.
echo === Preparing ECS Service Definition ===
copy ecs\service-definition.json ecs\service-definition-deploy.json >nul

powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content ecs\service-definition-deploy.json"
powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{SUBNET_1}}', '!SUBNET_1!' | Set-Content ecs\service-definition-deploy.json"
powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{SUBNET_2}}', '!SUBNET_2!' | Set-Content ecs\service-definition-deploy.json"
powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content ecs\service-definition-deploy.json"

if "!TARGET_GROUP_ARN!"==" " (
    echo Removing load balancer configuration from service definition...
    powershell -Command "$json = Get-Content ecs\service-definition-deploy.json | ConvertFrom-Json; $json.PSObject.Properties.Remove('loadBalancers'); $json.PSObject.Properties.Remove('healthCheckGracePeriodSeconds'); $json | ConvertTo-Json -Depth 10 | Set-Content ecs\service-definition-deploy.json"
) else (
    powershell -Command "(Get-Content ecs\service-definition-deploy.json) -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content ecs\service-definition-deploy.json"
)

set SERVICE_NAME=tourmgmt-container-service

echo.
echo === Checking if ECS service exists ===
for /f "tokens=*" %%i in ('aws ecs describe-services --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!" --query "services[?status==`ACTIVE`].serviceName" --output text') do set SERVICE_EXISTS=%%i

if "!SERVICE_EXISTS!"==" " (
    echo Service does not exist. Creating new ECS service...
    aws ecs create-service --cli-input-json file://ecs/service-definition-deploy.json --region "!AWS_REGION!" >nul
    echo Service created successfully
) else (
    echo Service exists. Updating ECS service...
    aws ecs update-service --cluster "!CLUSTER_NAME!" --service "!SERVICE_NAME!" --task-definition "!TASK_DEF_ARN!" --region "!AWS_REGION!" >nul
    echo Service updated successfully
)

echo.
echo === Waiting for service stability ===
echo This may take several minutes...
aws ecs wait services-stable --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!"

echo.
echo === Deployment Status ===
for /f "tokens=*" %%i in ('aws ecs describe-services --cluster "!CLUSTER_NAME!" --services "!SERVICE_NAME!" --region "!AWS_REGION!" --query "services[0].runningCount" --output text') do set RUNNING_COUNT=%%i

echo Service: !SERVICE_NAME!
echo Running tasks: !RUNNING_COUNT!
echo CloudWatch log group: !LOG_GROUP!

if not "!TARGET_GROUP_ARN!"==" " (
    echo Load Balancer DNS: http://!ALB_DNS!
    echo.
    echo Access your application at: http://!ALB_DNS!
)

echo.
echo =====================================
echo Deployment completed successfully!
echo =====================================

del /f /q ecs\task-definition-deploy.json ecs\service-definition-deploy.json >nul 2>&1

echo.
echo Troubleshooting:
echo - View logs: aws logs tail !LOG_GROUP! --follow --region !AWS_REGION!
echo - Describe service: aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo - List tasks: aws ecs list-tasks --cluster !CLUSTER_NAME! --service-name !SERVICE_NAME! --region !AWS_REGION!

endlocal