# TourManagement Application - AWS ECS Fargate Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Docker Containerization](#docker-containerization)
4. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
5. [AWS ECS Fargate Deployment](#aws-ecs-fargate-deployment)
6. [Configuration Management](#configuration-management)
7. [Monitoring and Logging](#monitoring-and-logging)
8. [Troubleshooting](#troubleshooting)
9. [Security Best Practices](#security-best-practices)

---

## Prerequisites

### Required Tools
- **.NET 8.0 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker**: [Install Docker](https://docs.docker.com/get-docker/)
- **AWS CLI v2**: [Install AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
- **Git**: Version control system

### AWS Account Requirements
- Active AWS account with appropriate permissions
- AWS credentials configured (`aws configure`)
- IAM permissions for:
  - ECS (Elastic Container Service)
  - ECR (Elastic Container Registry)
  - VPC and networking resources
  - CloudWatch Logs
  - Application Load Balancer (optional)
  - IAM role creation

### Application Requirements
- **Framework**: .NET 8.0 (ASP.NET Core)
- **Application Type**: Web Application (Razor Pages)
- **Database**: SQL Server (RDS or external)
- **Health Check Endpoint**: `/health`
- **Application Port**: 8080

---

## Local Development Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd TMS14
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Configure Database Connection
Create `appsettings.Development.json` in `src/TourManagement.Web/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TourManagementDB;User Id=sa;Password=YourPassword;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### 4. Run Database Migrations
```bash
cd src/TourManagement.Web
dotnet ef database update
```

### 5. Run the Application Locally
```bash
dotnet run --project src/TourManagement.Web/TourManagement.Web.csproj
```

Access the application at: `http://localhost:5000` or `https://localhost:5001`

### 6. Verify Health Check
```bash
curl http://localhost:5000/health
```

Expected response: `Healthy`

---

## Docker Containerization

### 1. Build Docker Image Locally
```bash
# From repository root
docker build -f Dockerfile -t tourmanagement:latest .
```

### 2. Run Container Locally
```bash
docker run -d \
  --name tourmanagement-app \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e DB_SERVER=your-db-server \
  -e DB_NAME=TourManagementDB \
  -e DB_USER=your-db-user \
  -e DB_PASSWORD=your-db-password \
  tourmanagement:latest
```

### 3. Test Containerized Application
```bash
curl http://localhost:8080/health
```

### 4. View Container Logs
```bash
docker logs tourmanagement-app
```

### 5. Stop and Remove Container
```bash
docker stop tourmanagement-app
docker rm tourmanagement-app
```

---

## AWS ECS Fargate Prerequisites

### 1. Configure AWS CLI
```bash
aws configure
```

Provide:
- AWS Access Key ID
- AWS Secret Access Key
- Default region (e.g., `us-east-1`)
- Default output format (e.g., `json`)

### 2. Create VPC and Networking (if not exists)

#### Create VPC
```bash
VPC_ID=$(aws ec2 create-vpc \
  --cidr-block 10.0.0.0/16 \
  --query 'Vpc.VpcId' \
  --output text)

echo "VPC ID: $VPC_ID"
```

#### Create Subnets
```bash
# Public Subnet 1
SUBNET_1=$(aws ec2 create-subnet \
  --vpc-id $VPC_ID \
  --cidr-block 10.0.1.0/24 \
  --availability-zone us-east-1a \
  --query 'Subnet.SubnetId' \
  --output text)

# Public Subnet 2
SUBNET_2=$(aws ec2 create-subnet \
  --vpc-id $VPC_ID \
  --cidr-block 10.0.2.0/24 \
  --availability-zone us-east-1b \
  --query 'Subnet.SubnetId' \
  --output text)

echo "Subnet 1: $SUBNET_1"
echo "Subnet 2: $SUBNET_2"
```

#### Create Internet Gateway
```bash
IGW_ID=$(aws ec2 create-internet-gateway \
  --query 'InternetGateway.InternetGatewayId' \
  --output text)

aws ec2 attach-internet-gateway \
  --vpc-id $VPC_ID \
  --internet-gateway-id $IGW_ID
```

#### Create Route Table
```bash
ROUTE_TABLE_ID=$(aws ec2 create-route-table \
  --vpc-id $VPC_ID \
  --query 'RouteTable.RouteTableId' \
  --output text)

aws ec2 create-route \
  --route-table-id $ROUTE_TABLE_ID \
  --destination-cidr-block 0.0.0.0/0 \
  --gateway-id $IGW_ID

aws ec2 associate-route-table \
  --subnet-id $SUBNET_1 \
  --route-table-id $ROUTE_TABLE_ID

aws ec2 associate-route-table \
  --subnet-id $SUBNET_2 \
  --route-table-id $ROUTE_TABLE_ID
```

### 3. Create Security Group
```bash
SECURITY_GROUP_ID=$(aws ec2 create-security-group \
  --group-name tourmanagement-sg \
  --description "Security group for TourManagement application" \
  --vpc-id $VPC_ID \
  --query 'GroupId' \
  --output text)

# Allow HTTP traffic (port 80)
aws ec2 authorize-security-group-ingress \
  --group-id $SECURITY_GROUP_ID \
  --protocol tcp \
  --port 80 \
  --cidr 0.0.0.0/0

# Allow application traffic (port 8080)
aws ec2 authorize-security-group-ingress \
  --group-id $SECURITY_GROUP_ID \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0

echo "Security Group ID: $SECURITY_GROUP_ID"
```

### 4. Create IAM Roles

#### ECS Task Execution Role
Create `ecs-task-execution-role-trust-policy.json`:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "ecs-tasks.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
```

Create the role:
```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document file://ecs-task-execution-role-trust-policy.json

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (optional, for application permissions)
Create `ecs-task-role-trust-policy.json`:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "ecs-tasks.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
```

Create the role:
```bash
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document file://ecs-task-role-trust-policy.json
```

### 5. Create RDS Database (if needed)
```bash
aws rds create-db-instance \
  --db-instance-identifier tourmanagement-db \
  --db-instance-class db.t3.micro \
  --engine sqlserver-ex \
  --master-username admin \
  --master-user-password YourSecurePassword123! \
  --allocated-storage 20 \
  --vpc-security-group-ids $SECURITY_GROUP_ID \
  --db-subnet-group-name your-db-subnet-group \
  --publicly-accessible
```

---

## AWS ECS Fargate Deployment

### Step 1: Build and Push Docker Image

#### Option A: Using the Build Script (Linux/macOS)
```bash
cd scripts
chmod +x build-push.sh
./build-push.sh
```

The script will prompt you to:
1. Select registry type (AWS ECR or Docker Hub)
2. Provide registry credentials and details
3. Enter image tag (default: latest)

The script will:
- Authenticate with the selected registry
- Build the Docker image
- Tag the image appropriately
- Push the image to the registry
- For ECR: Automatically create the repository if it doesn't exist

#### Option B: Using the Build Script (Windows)
```cmd
cd scripts
build-push.bat
```

#### Option C: Manual AWS ECR Push
```bash
# Set variables
AWS_REGION="us-east-1"
AWS_ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
ECR_REPO="tourmanagement"
IMAGE_TAG="latest"

# Create ECR repository
aws ecr create-repository \
  --repository-name $ECR_REPO \
  --region $AWS_REGION

# Authenticate Docker with ECR
aws ecr get-login-password --region $AWS_REGION | \
  docker login --username AWS --password-stdin \
  ${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com

# Build and tag image
docker build -f Dockerfile -t tourmanagement:$IMAGE_TAG .
docker tag tourmanagement:$IMAGE_TAG \
  ${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com/${ECR_REPO}:${IMAGE_TAG}

# Push image to ECR
docker push ${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com/${ECR_REPO}:${IMAGE_TAG}
```

### Step 2: Deploy to AWS ECS Fargate

#### Option A: Using the Deployment Script (Linux/macOS)
```bash
cd scripts
chmod +x deploy-image.sh
./deploy-image.sh
```

The script will prompt you for:
1. AWS region
2. ECS cluster name
3. VPC ID
4. Subnet IDs (comma-separated)
5. Security Group ID
6. Docker image URI from ECR
7. Database configuration
8. Load balancer requirement (y/n)

The script will:
- Create ECS cluster if it doesn't exist
- Create CloudWatch log group
- Register task definition with your configuration
- Create or update ECS service
- If load balancer requested:
  - Create Application Load Balancer
  - Create Target Group (with `target-type: ip` for Fargate)
  - Create listener
  - Configure health checks
- Wait for service to become stable
- Display deployment information

#### Option B: Using the Deployment Script (Windows)
```cmd
cd scripts
deploy-image.bat
```

#### Option C: Manual Deployment

##### Create ECS Cluster
```bash
aws ecs create-cluster \
  --cluster-name tourmanagement-cluster \
  --region us-east-1
```

##### Register Task Definition
```bash
# Update ecs/task-definition.json with your values
aws ecs register-task-definition \
  --cli-input-json file://ecs/task-definition.json \
  --region us-east-1
```

##### Create ECS Service
```bash
# Update ecs/service-definition.json with your values
aws ecs create-service \
  --cli-input-json file://ecs/service-definition.json \
  --region us-east-1
```

##### Wait for Service Stability
```bash
aws ecs wait services-stable \
  --cluster tourmanagement-cluster \
  --services tourmanagement-service \
  --region us-east-1
```

### Step 3: Verify Deployment

#### Check Service Status
```bash
aws ecs describe-services \
  --cluster tourmanagement-cluster \
  --services tourmanagement-service \
  --region us-east-1
```

#### List Running Tasks
```bash
aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-service \
  --region us-east-1
```

#### Get Task Details
```bash
TASK_ARN=$(aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-service \
  --region us-east-1 \
  --query 'taskArns[0]' \
  --output text)

aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks $TASK_ARN \
  --region us-east-1
```

#### Access Application
If using Application Load Balancer:
```bash
# Get ALB DNS name
ALB_DNS=$(aws elbv2 describe-load-balancers \
  --names tourmanagement-alb \
  --region us-east-1 \
  --query 'LoadBalancers[0].DNSName' \
  --output text)

echo "Application URL: http://$ALB_DNS"
echo "Health Check: http://$ALB_DNS/health"
```

---

## Configuration Management

### Environment Variables

The application uses the following environment variables:

| Variable | Description | Example |
|----------|-------------|----------|
| `ASPNETCORE_ENVIRONMENT` | Application environment | `Production` |
| `ASPNETCORE_URLS` | Application listening URLs | `http://+:8080` |
| `DB_SERVER` | Database server endpoint | `mydb.us-east-1.rds.amazonaws.com` |
| `DB_NAME` | Database name | `TourManagementDB` |
| `DB_USER` | Database username | `admin` |
| `DB_PASSWORD` | Database password | `SecurePassword123!` |
| `DOTNET_RUNNING_IN_CONTAINER` | Container indicator | `true` |

### Using AWS Secrets Manager (Recommended)

#### Store Database Password
```bash
aws secretsmanager create-secret \
  --name tourmanagement/db-password \
  --secret-string '{"password":"YourSecurePassword"}' \
  --region us-east-1
```

#### Update Task Definition to Use Secrets
Modify `ecs/task-definition.json`:
```json
{
  "containerDefinitions": [
    {
      "secrets": [
        {
          "name": "DB_PASSWORD",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:tourmanagement/db-password"
        }
      ]
    }
  ]
}
```

#### Grant Task Execution Role Access
```bash
aws iam put-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-name SecretsManagerAccess \
  --policy-document '{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Action": [
          "secretsmanager:GetSecretValue"
        ],
        "Resource": "arn:aws:secretsmanager:us-east-1:*:secret:tourmanagement/*"
      }
    ]
  }'
```

---

## Monitoring and Logging

### CloudWatch Logs

#### View Logs
```bash
aws logs tail /ecs/tourmanagement \
  --follow \
  --region us-east-1
```

#### Query Logs
```bash
aws logs filter-log-events \
  --log-group-name /ecs/tourmanagement \
  --filter-pattern "ERROR" \
  --region us-east-1
```

#### Create CloudWatch Dashboard
```bash
aws cloudwatch put-dashboard \
  --dashboard-name TourManagement \
  --dashboard-body file://cloudwatch-dashboard.json \
  --region us-east-1
```

### CloudWatch Metrics

#### CPU Utilization
```bash
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average \
  --region us-east-1
```

#### Memory Utilization
```bash
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name MemoryUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average \
  --region us-east-1
```

### Application Insights Integration

For enhanced monitoring, integrate Azure Application Insights:

1. Add Application Insights package:
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

2. Update `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

3. Configure connection string via environment variable:
```json
{
  "environment": [
    {
      "name": "APPLICATIONINSIGHTS_CONNECTION_STRING",
      "value": "InstrumentationKey=your-key;..."
    }
  ]
}
```

---

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptoms**: Tasks transition from PENDING to STOPPED immediately

**Diagnosis**:
```bash
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks <TASK_ARN> \
  --region us-east-1
```

**Common Causes**:
- Invalid CPU/memory combination (must use valid Fargate combinations)
- Image pull failure (check ECR permissions)
- Invalid environment variables
- Missing execution role permissions

**Valid Fargate CPU/Memory Combinations**:
- CPU: 256 (.25 vCPU) → Memory: 512, 1024, 2048 MB
- CPU: 512 (.5 vCPU) → Memory: 1024, 2048, 3072, 4096 MB
- CPU: 1024 (1 vCPU) → Memory: 2048-8192 MB (1GB increments)
- CPU: 2048 (2 vCPU) → Memory: 4096-16384 MB (1GB increments)
- CPU: 4096 (4 vCPU) → Memory: 8192-30720 MB (1GB increments)

**Solution**:
```bash
# Check task stopped reason
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks <TASK_ARN> \
  --query 'tasks[0].stoppedReason' \
  --output text

# Check CloudWatch logs for errors
aws logs tail /ecs/tourmanagement --region us-east-1
```

#### 2. Health Check Failures

**Symptoms**: Tasks fail health checks and restart continuously

**Diagnosis**:
```bash
# Check target group health
aws elbv2 describe-target-health \
  --target-group-arn <TARGET_GROUP_ARN> \
  --region us-east-1
```

**Common Causes**:
- Application not responding on correct port
- Health endpoint returning non-200 status
- Security group blocking traffic
- Database connection issues

**Solution**:
```bash
# Test health endpoint from within VPC
aws ecs execute-command \
  --cluster tourmanagement-cluster \
  --task <TASK_ARN> \
  --container tourmanagement \
  --interactive \
  --command "/bin/sh"

# Inside container:
curl http://localhost:8080/health
```

#### 3. Database Connection Errors

**Symptoms**: Application starts but cannot connect to database

**Common Causes**:
- Incorrect database endpoint
- Invalid credentials
- Security group not allowing traffic from ECS tasks
- Database not in same VPC or no VPC peering

**Solution**:
```bash
# Verify security group rules
aws ec2 describe-security-groups \
  --group-ids <DB_SECURITY_GROUP_ID> \
  --region us-east-1

# Add inbound rule for ECS security group
aws ec2 authorize-security-group-ingress \
  --group-id <DB_SECURITY_GROUP_ID> \
  --protocol tcp \
  --port 1433 \
  --source-group <ECS_SECURITY_GROUP_ID> \
  --region us-east-1
```

#### 4. Service Not Scaling

**Symptoms**: Service doesn't scale despite high load

**Solution**: Configure Service Auto Scaling
```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1

# Create scaling policy
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --policy-name tourmanagement-cpu-scaling \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 70.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
    },
    "ScaleInCooldown": 300,
    "ScaleOutCooldown": 60
  }' \
  --region us-east-1
```

#### 5. Load Balancer 502/503 Errors

**Symptoms**: Load balancer returns 502 Bad Gateway or 503 Service Unavailable

**Common Causes**:
- Target group using wrong target type (must be `ip` for Fargate)
- Security group blocking traffic between ALB and tasks
- Tasks not passing health checks
- Application crashed or not responding

**Solution**:
```bash
# Verify target group target type
aws elbv2 describe-target-groups \
  --target-group-arns <TARGET_GROUP_ARN> \
  --query 'TargetGroups[0].TargetType' \
  --output text
# Should return: ip

# Check registered targets
aws elbv2 describe-target-health \
  --target-group-arn <TARGET_GROUP_ARN> \
  --region us-east-1

# Verify security group allows ALB -> ECS traffic
aws ec2 authorize-security-group-ingress \
  --group-id <ECS_SECURITY_GROUP_ID> \
  --protocol tcp \
  --port 8080 \
  --source-group <ALB_SECURITY_GROUP_ID> \
  --region us-east-1
```

### Debugging Commands

#### Enable ECS Exec for interactive debugging
```bash
# Update service to enable ECS Exec
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --enable-execute-command \
  --region us-east-1

# Connect to running container
TASK_ID=$(aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-service \
  --query 'taskArns[0]' \
  --output text \
  --region us-east-1)

aws ecs execute-command \
  --cluster tourmanagement-cluster \
  --task $TASK_ID \
  --container tourmanagement \
  --interactive \
  --command "/bin/bash" \
  --region us-east-1
```

---

## Security Best Practices

### 1. Use Secrets Manager for Sensitive Data
- Store database passwords, API keys, and connection strings in AWS Secrets Manager
- Reference secrets in task definition using `secrets` parameter
- Rotate secrets regularly

### 2. Implement Least Privilege IAM Roles
- Task Execution Role: Only permissions to pull images and write logs
- Task Role: Only permissions required by application
- Avoid using wildcard (*) permissions

### 3. Network Security
- Use private subnets for tasks when possible
- Configure security groups with minimal required access
- Enable VPC Flow Logs for network monitoring
- Use AWS PrivateLink for accessing AWS services

### 4. Container Security
- Use official Microsoft base images
- Run containers as non-root user
- Scan images for vulnerabilities (AWS ECR provides scanning)
- Keep base images updated

### 5. Enable CloudWatch Logs
- All application logs should go to CloudWatch
- Set up log retention policies
- Create CloudWatch alarms for critical errors

### 6. HTTPS/TLS Configuration
- Use Application Load Balancer with HTTPS listener
- Obtain SSL/TLS certificate from ACM (AWS Certificate Manager)
- Configure security policies (minimum TLS 1.2)

```bash
# Request certificate
aws acm request-certificate \
  --domain-name tourmanagement.example.com \
  --validation-method DNS \
  --region us-east-1

# Add HTTPS listener to ALB
aws elbv2 create-listener \
  --load-balancer-arn <ALB_ARN> \
  --protocol HTTPS \
  --port 443 \
  --certificates CertificateArn=<CERTIFICATE_ARN> \
  --default-actions Type=forward,TargetGroupArn=<TARGET_GROUP_ARN> \
  --region us-east-1
```

### 7. Regular Security Audits
- Review IAM policies quarterly
- Scan containers for vulnerabilities
- Monitor CloudWatch logs for suspicious activity
- Keep dependencies updated

---

## Scaling and Performance

### Service Auto Scaling

Configure auto scaling based on CPU or memory utilization:

```bash
# CPU-based scaling
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --policy-name cpu-scaling \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 70.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
    }
  }'

# Memory-based scaling
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --policy-name memory-scaling \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 80.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageMemoryUtilization"
    }
  }'
```

### .NET Performance Optimization

#### 1. Enable ReadyToRun (R2R) for faster startup
Update `.csproj`:
```xml
<PropertyGroup>
  <PublishReadyToRun>true</PublishReadyToRun>
</PropertyGroup>
```

#### 2. Configure Garbage Collection
Set environment variables in task definition:
```json
{
  "environment": [
    {
      "name": "DOTNET_gcServer",
      "value": "1"
    },
    {
      "name": "DOTNET_GCConserveMemory",
      "value": "9"
    }
  ]
}
```

#### 3. Use Response Caching
In `Program.cs`:
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

---

## Blue/Green Deployments

ECS supports blue/green deployments with AWS CodeDeploy:

```bash
# Create CodeDeploy application
aws deploy create-application \
  --application-name tourmanagement-app \
  --compute-platform ECS

# Create deployment group
aws deploy create-deployment-group \
  --application-name tourmanagement-app \
  --deployment-group-name tourmanagement-dg \
  --service-role-arn arn:aws:iam::ACCOUNT_ID:role/CodeDeployServiceRole \
  --deployment-config-name CodeDeployDefault.ECSAllAtOnce \
  --ecs-services clusterName=tourmanagement-cluster,serviceName=tourmanagement-service \
  --load-balancer-info targetGroupPairInfoList=[{...}]
```

---

## Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/core/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [AWS CLI Reference](https://docs.aws.amazon.com/cli/latest/reference/)

---

## Support and Feedback

For issues, questions, or contributions, please contact your DevOps team or open an issue in the project repository.

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-02  
**Application**: TourManagement  
**Platform**: AWS ECS Fargate  
**Framework**: .NET 8.0 (ASP.NET Core)
