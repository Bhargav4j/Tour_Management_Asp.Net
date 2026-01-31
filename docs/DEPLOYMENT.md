# TourManagement - AWS ECS Fargate Deployment Guide

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Docker Containerization](#docker-containerization)
4. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
5. [ECS Fargate Setup](#ecs-fargate-setup)
6. [ECS Task Definition Explained](#ecs-task-definition-explained)
7. [ECS Service Configuration](#ecs-service-configuration)
8. [ECS Fargate Deployment Walkthrough](#ecs-fargate-deployment-walkthrough)
9. [ECS-Specific Troubleshooting](#ecs-specific-troubleshooting)
10. [ECS Fargate Scaling and Management](#ecs-fargate-scaling-and-management)
11. [Configuration Management](#configuration-management)
12. [Security Considerations](#security-considerations)
13. [Monitoring and Logging](#monitoring-and-logging)

---

## Prerequisites

### Required Software

- **.NET 8.0 SDK** or later
- **Docker Desktop** (for Windows/Mac) or Docker Engine (for Linux)
- **AWS CLI v2** - [Installation Guide](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
- **Git** (for version control)

### AWS Account Requirements

- Active AWS account with appropriate permissions
- IAM user with ECS, ECR, VPC, CloudWatch, and IAM permissions
- AWS CLI configured with access credentials

### .NET Application Requirements

- ASP.NET Core 8.0 application
- PostgreSQL database (external or AWS RDS)
- Redis for distributed caching (optional, falls back to in-memory)
- Health check endpoints configured: `/health`, `/health/ready`

---

## Local Development Setup

### 1. Clone and Build the Application

```bash
cd /path/to/TourContainerCMP
dotnet restore
dotnet build
```

### 2. Configure Application Settings

Update `src/TourManagement.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tourmanagement;Username=postgres;Password=yourpassword"
  },
  "Serilog": {
    "EnableFileLogging": true,
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### 3. Run Locally

```bash
cd src/TourManagement.Web
dotnet run
```

Access the application at `http://localhost:5000` or `https://localhost:5001`.

---

## Docker Containerization

### 1. Build Docker Image Locally

```bash
# From repository root
docker build -f Dockerfile -t tourmanagement:local .
```

### 2. Test Container Locally

```bash
docker run -d -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=tourmanagement;Username=postgres;Password=yourpassword" \
  -e ASPNETCORE_ENVIRONMENT=Development \
  --name tourmanagement-test \
  tourmanagement:local
```

### 3. Verify Health Checks

```bash
curl http://localhost:8080/health
curl http://localhost:8080/health/ready
```

### 4. Build and Push to Registry

**Linux/macOS:**

```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows:**

```cmd
scripts\build-push.bat
```

Follow the prompts to select AWS ECR or Docker Hub and provide credentials.

---

## AWS ECS Fargate Prerequisites

### 1. AWS CLI Configuration

```bash
aws configure
# Enter:
# - AWS Access Key ID
# - AWS Secret Access Key
# - Default region (e.g., us-east-1)
# - Output format (json)
```

### 2. Required IAM Roles

#### ECS Task Execution Role

This role allows ECS to pull images from ECR and write logs to CloudWatch.

```bash
aws iam create-role --role-name ecsTaskExecutionRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'

aws iam attach-role-policy --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (Optional)

For application-level AWS permissions (e.g., S3 access, DynamoDB).

```bash
aws iam create-role --role-name ecsTaskRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'
```

### 3. VPC and Networking Setup

#### Create VPC (if needed)

```bash
VPC_ID=$(aws ec2 create-vpc --cidr-block 10.0.0.0/16 \
  --query 'Vpc.VpcId' --output text)

aws ec2 create-tags --resources $VPC_ID --tags Key=Name,Value=ecs-vpc
```

#### Create Subnets

```bash
SUBNET_1=$(aws ec2 create-subnet --vpc-id $VPC_ID \
  --cidr-block 10.0.1.0/24 --availability-zone us-east-1a \
  --query 'Subnet.SubnetId' --output text)

SUBNET_2=$(aws ec2 create-subnet --vpc-id $VPC_ID \
  --cidr-block 10.0.2.0/24 --availability-zone us-east-1b \
  --query 'Subnet.SubnetId' --output text)
```

#### Create Internet Gateway

```bash
IGW_ID=$(aws ec2 create-internet-gateway \
  --query 'InternetGateway.InternetGatewayId' --output text)

aws ec2 attach-internet-gateway --vpc-id $VPC_ID --internet-gateway-id $IGW_ID
```

#### Configure Route Table

```bash
ROUTE_TABLE_ID=$(aws ec2 describe-route-tables \
  --filters "Name=vpc-id,Values=$VPC_ID" \
  --query 'RouteTables[0].RouteTableId' --output text)

aws ec2 create-route --route-table-id $ROUTE_TABLE_ID \
  --destination-cidr-block 0.0.0.0/0 --gateway-id $IGW_ID

aws ec2 associate-route-table --subnet-id $SUBNET_1 --route-table-id $ROUTE_TABLE_ID
aws ec2 associate-route-table --subnet-id $SUBNET_2 --route-table-id $ROUTE_TABLE_ID
```

### 4. Security Group Configuration

```bash
SG_ID=$(aws ec2 create-security-group \
  --group-name ecs-tourmanagement-sg \
  --description "Security group for TourManagement ECS tasks" \
  --vpc-id $VPC_ID \
  --query 'GroupId' --output text)

# Allow HTTP traffic (port 8080)
aws ec2 authorize-security-group-ingress --group-id $SG_ID \
  --protocol tcp --port 8080 --cidr 0.0.0.0/0

# Allow HTTPS traffic (port 443) if needed
aws ec2 authorize-security-group-ingress --group-id $SG_ID \
  --protocol tcp --port 443 --cidr 0.0.0.0/0

# Allow ALB health checks
aws ec2 authorize-security-group-ingress --group-id $SG_ID \
  --protocol tcp --port 8080 --source-group $SG_ID
```

---

## ECS Fargate Setup

### 1. Create ECS Cluster

```bash
aws ecs create-cluster --cluster-name tourmanagement-cluster
```

### 2. CloudWatch Log Group

```bash
aws logs create-log-group --log-group-name /ecs/tourmanagement
aws logs put-retention-policy --log-group-name /ecs/tourmanagement --retention-in-days 7
```

---

## ECS Task Definition Explained

### Key Configuration Parameters

#### Launch Type Configuration

```json
"requiresCompatibilities": ["FARGATE"],
"networkMode": "awsvpc"
```

- **FARGATE**: Serverless compute engine for containers
- **awsvpc**: Required network mode for Fargate (each task gets its own ENI)

#### CPU and Memory

```json
"cpu": "512",
"memory": "1024"
```

**Valid Fargate CPU/Memory Combinations:**

| CPU (vCPU) | Memory (MB) |
|------------|-------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

#### Execution Role

```json
"executionRoleArn": "arn:aws:iam::123456789012:role/ecsTaskExecutionRole"
```

Allows ECS to:
- Pull images from ECR
- Write logs to CloudWatch Logs
- Retrieve secrets from Secrets Manager/Parameter Store

#### Container Definition

```json
"containerDefinitions": [{
  "name": "tourmanagement",
  "image": "123456789012.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest",
  "essential": true,
  "portMappings": [{"containerPort": 8080, "protocol": "tcp"}],
  "environment": [...],
  "logConfiguration": {...}
}]
```

---

## ECS Service Configuration

### Service Definition Key Features

#### Deployment Configuration

```json
"deploymentConfiguration": {
  "maximumPercent": 200,
  "minimumHealthyPercent": 50,
  "deploymentCircuitBreaker": {
    "enable": true,
    "rollback": true
  }
}
```

- **maximumPercent**: Maximum % of desired count during deployment (allows for 2x tasks temporarily)
- **minimumHealthyPercent**: Minimum % that must remain healthy during deployment
- **deploymentCircuitBreaker**: Automatically rolls back failed deployments

#### Load Balancer Integration

```json
"loadBalancers": [{
  "targetGroupArn": "arn:aws:elasticloadbalancing:...",
  "containerName": "tourmanagement",
  "containerPort": 8080
}],
"healthCheckGracePeriodSeconds": 300
```

---

## ECS Fargate Deployment Walkthrough

### Step 1: Build and Push Docker Image

```bash
# Linux/macOS
chmod +x scripts/build-push.sh
./scripts/build-push.sh

# Windows
scripts\build-push.bat
```

**Example Prompts:**
- Registry: Select **1** for AWS ECR
- AWS Region: `us-east-1`
- AWS Account ID: `123456789012`
- ECR Repository Name: `tourmanagement` (default)
- Image Tag: `v1.0.0` or `latest`

### Step 2: Deploy to ECS Fargate

```bash
# Linux/macOS
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh

# Windows
scripts\deploy-image.bat
```

**Example Prompts:**
- AWS Region: `us-east-1`
- ECS Cluster Name: `tourmanagement-cluster`
- VPC ID: `vpc-0abc123def456`
- Subnet IDs: `subnet-0abc123,subnet-0def456`
- Security Group ID: `sg-0abc123def`
- Docker Image URI: `123456789012.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:v1.0.0`
- PostgreSQL Connection String: `Host=mydb.rds.amazonaws.com;Database=tourmanagement;Username=admin;Password=yourpassword`
- Redis Connection String: `myredis.cache.amazonaws.com:6379` (optional)
- Load Balancer: `y` (if using ALB)

### Step 3: Monitor Deployment

```bash
# Watch service status
aws ecs describe-services --cluster tourmanagement-cluster \
  --services tourmanagement-service --query 'services[0].events[0:5]'

# View running tasks
aws ecs list-tasks --cluster tourmanagement-cluster \
  --service-name tourmanagement-service

# Check task health
aws ecs describe-tasks --cluster tourmanagement-cluster \
  --tasks <task-id> --query 'tasks[0].healthStatus'
```

### Step 4: View Logs

```bash
# Tail logs in real-time
aws logs tail /ecs/tourmanagement --follow

# View specific time range
aws logs filter-log-events --log-group-name /ecs/tourmanagement \
  --start-time $(date -d '10 minutes ago' +%s)000
```

### Step 5: Access Application

If using Application Load Balancer:

```bash
# Get ALB DNS name
ALB_DNS=$(aws elbv2 describe-load-balancers \
  --names tourmanagement-alb \
  --query 'LoadBalancers[0].DNSName' --output text)

echo "Application URL: http://$ALB_DNS"
```

Test health endpoints:

```bash
curl http://$ALB_DNS/health
curl http://$ALB_DNS/health/ready
```

---

## ECS-Specific Troubleshooting

### Task Fails to Start

**Check Task Stopped Reason:**

```bash
aws ecs describe-tasks --cluster tourmanagement-cluster \
  --tasks <task-id> --query 'tasks[0].stoppedReason'
```

**Common Issues:**

1. **"CannotPullContainerError"**
   - **Cause**: ECS cannot pull image from ECR
   - **Solution**: Verify execution role has `ecr:GetAuthorizationToken`, `ecr:BatchCheckLayerAvailability`, `ecr:GetDownloadUrlForLayer`, `ecr:BatchGetImage` permissions

2. **"ResourceInitializationError: failed to create log stream"**
   - **Cause**: Insufficient CloudWatch Logs permissions
   - **Solution**: Ensure execution role has `logs:CreateLogStream`, `logs:PutLogEvents` permissions

3. **"Essential container exited"**
   - **Cause**: Application crashed or failed to start
   - **Solution**: Check CloudWatch Logs for application errors

### Invalid CPU/Memory Combination

**Error:**
```
ClientException: No Fargate configuration exists for the given memory and CPU combination
```

**Solution**: Use valid Fargate CPU/memory combinations (see table in [ECS Task Definition Explained](#ecs-task-definition-explained)).

### Health Check Failures

**Check Target Group Health:**

```bash
aws elbv2 describe-target-health --target-group-arn <target-group-arn>
```

**Common Fixes:**
- Verify security group allows ALB → ECS task traffic
- Increase `healthCheckGracePeriodSeconds` (default: 300 seconds)
- Verify `/health` endpoint responds with 200 status

### Network Connectivity Issues

**Verify Task Has Public IP:**

```bash
aws ecs describe-tasks --cluster tourmanagement-cluster \
  --tasks <task-id> --query 'tasks[0].attachments[0].details[?name==`networkInterfaceId`].value' --output text
```

**Check Security Group Rules:**

```bash
aws ec2 describe-security-groups --group-ids $SG_ID
```

---

## ECS Fargate Scaling and Management

### Manual Scaling

```bash
aws ecs update-service --cluster tourmanagement-cluster \
  --service tourmanagement-service --desired-count 4
```

### Service Auto Scaling

#### Register Scalable Target

```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10
```

#### Create Scaling Policy (Target Tracking)

```bash
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 70.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
    },
    "ScaleOutCooldown": 60,
    "ScaleInCooldown": 60
  }'
```

### Blue/Green Deployments

For zero-downtime deployments, configure ECS with CodeDeploy:

```bash
aws deploy create-deployment-group \
  --application-name AppECS-tourmanagement-cluster-tourmanagement-service \
  --deployment-group-name DgpECS-tourmanagement-cluster-tourmanagement-service \
  --deployment-config-name CodeDeployDefault.ECSAllAtOnce \
  --ecs-services serviceName=tourmanagement-service,clusterName=tourmanagement-cluster \
  --load-balancer-info targetGroupInfoList=[{name=tourmanagement-tg}] \
  --service-role-arn arn:aws:iam::123456789012:role/CodeDeployServiceRole
```

---

## Configuration Management

### Environment-Specific Settings

**Using AWS Systems Manager Parameter Store:**

```bash
# Store connection string
aws ssm put-parameter --name /tourmanagement/prod/db-connection \
  --value "Host=mydb.rds.amazonaws.com;Database=tourmanagement;Username=admin;Password=securepassword" \
  --type SecureString

# Reference in task definition
"secrets": [
  {
    "name": "ConnectionStrings__DefaultConnection",
    "valueFrom": "/tourmanagement/prod/db-connection"
  }
]
```

**Using AWS Secrets Manager:**

```bash
aws secretsmanager create-secret --name tourmanagement/prod/database \
  --secret-string '{"username":"admin","password":"securepassword","host":"mydb.rds.amazonaws.com","database":"tourmanagement"}'
```

---

## Security Considerations

### 1. Use Private Subnets with NAT Gateway

For production deployments:

```bash
# Create NAT Gateway
EIP_ID=$(aws ec2 allocate-address --domain vpc --query 'AllocationId' --output text)
NAT_GW_ID=$(aws ec2 create-nat-gateway --subnet-id $SUBNET_1 \
  --allocation-id $EIP_ID --query 'NatGateway.NatGatewayId' --output text)

# Update route table for private subnets
aws ec2 create-route --route-table-id $PRIVATE_ROUTE_TABLE_ID \
  --destination-cidr-block 0.0.0.0/0 --nat-gateway-id $NAT_GW_ID
```

Update service definition:

```json
"assignPublicIp": "DISABLED"
```

### 2. Secrets Management

**Never hardcode secrets** in task definitions. Use:
- AWS Secrets Manager
- AWS Systems Manager Parameter Store (SecureString)

### 3. IAM Least Privilege

Grant only necessary permissions to task roles:

```json
{
  "Version": "2012-10-17",
  "Statement": [{
    "Effect": "Allow",
    "Action": [
      "s3:GetObject",
      "s3:PutObject"
    ],
    "Resource": "arn:aws:s3:::my-bucket/*"
  }]
}
```

### 4. Container Image Scanning

```bash
aws ecr start-image-scan --repository-name tourmanagement --image-id imageTag=latest
aws ecr describe-image-scan-findings --repository-name tourmanagement --image-id imageTag=latest
```

---

## Monitoring and Logging

### CloudWatch Metrics

**Key Metrics to Monitor:**

- `CPUUtilization`
- `MemoryUtilization`
- `TargetResponseTime` (ALB)
- `HealthyHostCount` (ALB)
- `UnHealthyHostCount` (ALB)

**View Metrics:**

```bash
aws cloudwatch get-metric-statistics --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 --statistics Average
```

### CloudWatch Alarms

**CPU Utilization Alarm:**

```bash
aws cloudwatch put-metric-alarm --alarm-name tourmanagement-high-cpu \
  --alarm-description "Alert when CPU exceeds 80%" \
  --metric-name CPUUtilization --namespace AWS/ECS \
  --statistic Average --period 300 --threshold 80 \
  --comparison-operator GreaterThanThreshold \
  --evaluation-periods 2 \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster
```

### Application Insights Integration

For .NET applications, integrate Application Insights:

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**appsettings.json:**

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key-here"
  }
}
```

---

## Additional Resources

- [AWS ECS Developer Guide](https://docs.aws.amazon.com/ecs/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [.NET on AWS Guide](https://aws.amazon.com/developer/language/net/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)

---

## Support and Troubleshooting

For issues or questions:

1. Check CloudWatch Logs: `aws logs tail /ecs/tourmanagement --follow`
2. Review ECS events: `aws ecs describe-services --cluster tourmanagement-cluster --services tourmanagement-service`
3. Validate task definition: `aws ecs describe-task-definition --task-definition tourmanagement-task`
4. Check target health: `aws elbv2 describe-target-health --target-group-arn <arn>`

---

**Document Version:** 1.0  
**Last Updated:** 2026-01-31  
**Target Platform:** AWS ECS Fargate  
**Application:** TourManagement (.NET 8.0 ASP.NET Core)
