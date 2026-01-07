# Tour Management Application - AWS ECS Fargate Deployment Guide

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Local Development Setup](#local-development-setup)
4. [Docker Deployment](#docker-deployment)
5. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
6. [ECS Fargate Setup](#ecs-fargate-setup)
7. [ECS Task Definition Explained](#ecs-task-definition-explained)
8. [ECS Service Configuration](#ecs-service-configuration)
9. [ECS Fargate Deployment Walkthrough](#ecs-fargate-deployment-walkthrough)
10. [Environment Variables](#environment-variables)
11. [Health Checks](#health-checks)
12. [Monitoring and Logging](#monitoring-and-logging)
13. [Troubleshooting](#troubleshooting)
14. [Security Considerations](#security-considerations)
15. [Scaling and Performance](#scaling-and-performance)

---

## Overview

The Tour Management application is a .NET 8.0 ASP.NET Core Razor Pages application designed for managing tours and related activities. This guide provides comprehensive instructions for deploying the application to AWS ECS Fargate.

### Technology Stack

- **Framework**: .NET 8.0
- **Application Type**: ASP.NET Core Razor Pages
- **Database**: SQL Server (via Entity Framework Core)
- **Authentication**: Cookie-based authentication
- **Logging**: Serilog with console output
- **Health Checks**: ASP.NET Core health checks with EF Core database context check

### Application Architecture

The application follows a clean architecture pattern with the following layers:

- **TourManagement.Web**: Presentation layer (Razor Pages)
- **TourManagement.Application**: Application logic and services
- **TourManagement.Domain**: Domain models and business rules
- **TourManagement.Infrastructure**: Data access and external services

---

## Prerequisites

### Required Tools

1. **Docker Desktop** (version 20.10 or later)
   - Download: https://www.docker.com/products/docker-desktop
   - Verify installation: `docker --version`

2. **AWS CLI** (version 2.x)
   - Download: https://aws.amazon.com/cli/
   - Verify installation: `aws --version`
   - Configure credentials: `aws configure`

3. **.NET 8.0 SDK** (for local development)
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version`

### AWS Account Requirements

1. **Active AWS Account** with appropriate permissions
2. **IAM User** with the following policies:
   - AmazonECS_FullAccess
   - AmazonEC2ContainerRegistryFullAccess
   - IAMReadOnlyAccess (to verify roles)
   - ElasticLoadBalancingFullAccess (if using ALB)
   - CloudWatchLogsFullAccess

3. **AWS Credentials** configured locally:
   ```bash
   aws configure
   # Enter your AWS Access Key ID
   # Enter your AWS Secret Access Key
   # Enter default region (e.g., us-east-1)
   # Enter default output format (json)
   ```

### Database Requirements

- **SQL Server Instance** (AWS RDS recommended)
- Database connection details:
  - Server address
  - Database name
  - Username and password
  - Ensure security groups allow connections from ECS tasks

---

## Local Development Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd TourMgmtContain07cmp
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Update Configuration

Edit `src/TourManagement.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TourManagement;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### 4. Run Database Migrations

```bash
cd src/TourManagement.Web
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run --project src/TourManagement.Web/TourManagement.Web.csproj
```

Access the application at: `https://localhost:5001` or `http://localhost:5000`

---

## Docker Deployment

### Build Docker Image Locally

```bash
# From repository root
docker build -t tourmanagement:latest -f Dockerfile .
```

### Run Container Locally

```bash
docker run -d \
  --name tourmanagement \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e DB_SERVER=your-db-server \
  -e DB_NAME=TourManagement \
  -e DB_USER=your-db-user \
  -e DB_PASSWORD=your-db-password \
  tourmanagement:latest
```

Access the application at: `http://localhost:8080`

### Test Health Endpoints

```bash
curl http://localhost:8080/health
curl http://localhost:8080/ready
```

Expected response: `Healthy`

---

## AWS ECS Fargate Prerequisites

### 1. VPC and Networking Setup

**Create or Identify VPC:**

```bash
# List existing VPCs
aws ec2 describe-vpcs --query 'Vpcs[*].[VpcId,CidrBlock,Tags[?Key==`Name`].Value|[0]]' --output table

# Create new VPC (if needed)
aws ec2 create-vpc --cidr-block 10.0.0.0/16 --tag-specifications 'ResourceType=vpc,Tags=[{Key=Name,Value=TourManagement-VPC}]'
```

**Create Subnets (minimum 2 for high availability):**

```bash
# Subnet 1 (Availability Zone 1)
aws ec2 create-subnet \
  --vpc-id vpc-xxxxxxxxx \
  --cidr-block 10.0.1.0/24 \
  --availability-zone us-east-1a \
  --tag-specifications 'ResourceType=subnet,Tags=[{Key=Name,Value=TourManagement-Subnet-1}]'

# Subnet 2 (Availability Zone 2)
aws ec2 create-subnet \
  --vpc-id vpc-xxxxxxxxx \
  --cidr-block 10.0.2.0/24 \
  --availability-zone us-east-1b \
  --tag-specifications 'ResourceType=subnet,Tags=[{Key=Name,Value=TourManagement-Subnet-2}]'

# Enable auto-assign public IP
aws ec2 modify-subnet-attribute --subnet-id subnet-xxxxxxxxx --map-public-ip-on-launch
```

**Create Internet Gateway:**

```bash
aws ec2 create-internet-gateway --tag-specifications 'ResourceType=internet-gateway,Tags=[{Key=Name,Value=TourManagement-IGW}]'
aws ec2 attach-internet-gateway --vpc-id vpc-xxxxxxxxx --internet-gateway-id igw-xxxxxxxxx
```

**Update Route Table:**

```bash
# Get route table ID
aws ec2 describe-route-tables --filters "Name=vpc-id,Values=vpc-xxxxxxxxx" --query 'RouteTables[0].RouteTableId' --output text

# Add route to internet gateway
aws ec2 create-route --route-table-id rtb-xxxxxxxxx --destination-cidr-block 0.0.0.0/0 --gateway-id igw-xxxxxxxxx
```

### 2. Security Group Configuration

**Create Security Group:**

```bash
aws ec2 create-security-group \
  --group-name tourmanagement-sg \
  --description "Security group for Tour Management ECS tasks" \
  --vpc-id vpc-xxxxxxxxx
```

**Add Inbound Rules:**

```bash
# Allow HTTP traffic (port 8080) from anywhere
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxxxxxx \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0

# Allow HTTP traffic (port 80) for load balancer
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxxxxxx \
  --protocol tcp \
  --port 80 \
  --cidr 0.0.0.0/0

# Allow database connections (if database is in same VPC)
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxxxxxx \
  --protocol tcp \
  --port 1433 \
  --source-group sg-xxxxxxxxx
```

### 3. IAM Roles Setup

**ECS Task Execution Role** (required for Fargate):

```bash
# Create trust policy file
cat > ecs-task-execution-trust-policy.json <<EOF
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
EOF

# Create role
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document file://ecs-task-execution-trust-policy.json

# Attach AWS managed policy
aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

**ECS Task Role** (optional, for application permissions):

```bash
# Create role
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document file://ecs-task-execution-trust-policy.json

# Attach policies as needed (e.g., for S3 access)
aws iam attach-role-policy \
  --role-name ecsTaskRole \
  --policy-arn arn:aws:iam::aws:policy/AmazonS3ReadOnlyAccess
```

### 4. CloudWatch Log Group

```bash
# Create log group
aws logs create-log-group --log-group-name /ecs/tourmanagement

# Set retention policy (optional, e.g., 7 days)
aws logs put-retention-policy --log-group-name /ecs/tourmanagement --retention-in-days 7
```

---

## ECS Fargate Setup

### 1. Create ECS Cluster

```bash
aws ecs create-cluster --cluster-name tourmanagement-cluster
```

### 2. Create ECR Repository (if using ECR)

```bash
# Create repository
aws ecr create-repository --repository-name tourmanagement

# Get repository URI
aws ecr describe-repositories --repository-names tourmanagement --query 'repositories[0].repositoryUri' --output text
```

### 3. Build and Push Docker Image

**Using provided scripts:**

**Linux/macOS:**
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows:**
```cmd
scripts\build-push.bat
```

The scripts will:
1. Prompt for registry selection (ECR or Docker Hub)
2. Request registry credentials and details
3. Build the Docker image
4. Tag and push to the selected registry

**Manual ECR push:**

```bash
# Authenticate Docker to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789.dkr.ecr.us-east-1.amazonaws.com

# Build image
docker build -t tourmanagement:latest .

# Tag image
docker tag tourmanagement:latest 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest

# Push image
docker push 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest
```

---

## ECS Task Definition Explained

### Task Definition Structure

The task definition (`ecs/task-definition.json`) defines how your container runs:

**Key Components:**

1. **Launch Type**: `FARGATE` - serverless container execution
2. **Network Mode**: `awsvpc` - each task gets its own ENI and private IP
3. **CPU and Memory**: Must use valid Fargate combinations

**Valid Fargate CPU/Memory Combinations:**

| CPU (vCPU) | Memory (MB) Options |
|------------|---------------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

**Default Configuration**: `cpu: "512"`, `memory: "1024"` (recommended for .NET applications)

### Container Definition

**Port Mappings:**
```json
"portMappings": [
  {
    "containerPort": 8080,
    "protocol": "tcp"
  }
]
```

**Note**: Fargate uses `awsvpc` network mode, so only `containerPort` is needed (no `hostPort`).

**Environment Variables:**
```json
"environment": [
  {"name": "ASPNETCORE_ENVIRONMENT", "value": "Production"},
  {"name": "ASPNETCORE_URLS", "value": "http://+:8080"},
  {"name": "DB_SERVER", "value": "your-rds-endpoint"},
  {"name": "DB_NAME", "value": "TourManagement"},
  {"name": "DB_USER", "value": "dbuser"},
  {"name": "DB_PASSWORD", "value": "dbpassword"}
]
```

**Logging Configuration:**
```json
"logConfiguration": {
  "logDriver": "awslogs",
  "options": {
    "awslogs-group": "/ecs/tourmanagement",
    "awslogs-region": "us-east-1",
    "awslogs-stream-prefix": "ecs",
    "awslogs-create-group": "true"
  }
}
```

### Execution and Task Roles

**executionRoleArn**: Allows ECS to pull images from ECR and send logs to CloudWatch
**taskRoleArn**: Grants permissions to your application (e.g., access to S3, DynamoDB)

---

## ECS Service Configuration

### Service Definition Structure

The service definition (`ecs/service-definition.json`) manages task deployment:

**Key Components:**

1. **Desired Count**: Number of tasks to run (default: 2 for high availability)
2. **Launch Type**: `FARGATE`
3. **Network Configuration**: Subnets, security groups, public IP assignment
4. **Load Balancer**: Optional ALB integration for traffic distribution
5. **Deployment Configuration**: Rolling update strategy

**Deployment Strategy:**
```json
"deploymentConfiguration": {
  "maximumPercent": 200,           // Can temporarily run 2x tasks during deployment
  "minimumHealthyPercent": 50,     // Must maintain at least 50% healthy tasks
  "deploymentCircuitBreaker": {
    "enable": true,                // Automatically rollback failed deployments
    "rollback": true
  }
}
```

**Network Configuration:**
```json
"networkConfiguration": {
  "awsvpcConfiguration": {
    "subnets": ["subnet-xxx", "subnet-yyy"],
    "securityGroups": ["sg-xxx"],
    "assignPublicIp": "ENABLED"     // Required if using public subnets without NAT
  }
}
```

**Load Balancer Integration (Optional):**
```json
"loadBalancers": [
  {
    "targetGroupArn": "arn:aws:elasticloadbalancing:...",
    "containerName": "tourmanagement",
    "containerPort": 8080
  }
],
"healthCheckGracePeriodSeconds": 300
```

---

## ECS Fargate Deployment Walkthrough

### Step 1: Prepare Configuration Files

Ensure you have:
- `ecs/task-definition.json` - Task configuration
- `ecs/service-definition.json` - Service configuration
- Docker image pushed to ECR or Docker Hub

### Step 2: Deploy Using Automation Script

**Linux/macOS:**
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

**Windows:**
```cmd
scripts\deploy-image.bat
```

**The script will prompt for:**
1. AWS Region (e.g., us-east-1)
2. ECS Cluster Name
3. VPC ID
4. Subnet IDs (comma-separated)
5. Security Group ID
6. Docker Image URI
7. Database connection details
8. Load balancer preference (y/n)

**Script Actions:**
1. Validates AWS credentials and region
2. Creates ECS cluster if it doesn't exist
3. Optionally creates Application Load Balancer and Target Group
4. Registers ECS task definition with provided configuration
5. Creates or updates ECS service
6. Waits for service to stabilize
7. Displays deployment status and access URLs

### Step 3: Manual Deployment (Alternative)

**Register Task Definition:**
```bash
aws ecs register-task-definition \
  --cli-input-json file://ecs/task-definition.json \
  --region us-east-1
```

**Create Service:**
```bash
aws ecs create-service \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-service \
  --task-definition tourmanagement-task \
  --desired-count 2 \
  --launch-type FARGATE \
  --platform-version LATEST \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxx,subnet-yyy],securityGroups=[sg-xxx],assignPublicIp=ENABLED}" \
  --region us-east-1
```

**Update Existing Service:**
```bash
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --task-definition tourmanagement-task:2 \
  --force-new-deployment \
  --region us-east-1
```

### Step 4: Verify Deployment

**Check Service Status:**
```bash
aws ecs describe-services \
  --cluster tourmanagement-cluster \
  --services tourmanagement-service \
  --region us-east-1
```

**List Running Tasks:**
```bash
aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-service \
  --region us-east-1
```

**Get Task Details:**
```bash
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks task-id \
  --region us-east-1
```

### Step 5: Access Application

**With Load Balancer:**
```bash
# Get ALB DNS name
aws elbv2 describe-load-balancers \
  --names tourmanagement-alb \
  --query 'LoadBalancers[0].DNSName' \
  --output text

# Access application
http://<alb-dns-name>
```

**Without Load Balancer:**
```bash
# Get task public IP
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks task-id \
  --query 'tasks[0].attachments[0].details[?name==`networkInterfaceId`].value' \
  --output text

# Get ENI public IP
aws ec2 describe-network-interfaces \
  --network-interface-ids eni-xxx \
  --query 'NetworkInterfaces[0].Association.PublicIp' \
  --output text

# Access application
http://<task-public-ip>:8080
```

---

## Environment Variables

### Required Environment Variables

| Variable | Description | Example |
|----------|-------------|--------|
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Production` |
| `ASPNETCORE_URLS` | Kestrel listening URLs | `http://+:8080` |
| `DB_SERVER` | Database server address | `mydb.region.rds.amazonaws.com` |
| `DB_NAME` | Database name | `TourManagement` |
| `DB_USER` | Database username | `dbadmin` |
| `DB_PASSWORD` | Database password | `SecurePassword123!` |

### Optional Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `TZ` | Timezone | `UTC` |
| `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` | Globalization mode | `false` |
| `Serilog__MinimumLevel__Default` | Logging level | `Information` |

### Managing Secrets

**Use AWS Secrets Manager or Parameter Store for sensitive data:**

**Create Secret:**
```bash
aws secretsmanager create-secret \
  --name tourmanagement/database \
  --secret-string '{"username":"dbadmin","password":"SecurePassword123!"}' \
  --region us-east-1
```

**Reference in Task Definition:**
```json
"secrets": [
  {
    "name": "DB_USER",
    "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:tourmanagement/database:username::"
  },
  {
    "name": "DB_PASSWORD",
    "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:tourmanagement/database:password::"
  }
]
```

**Update Task Execution Role Policy:**
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "secretsmanager:GetSecretValue"
      ],
      "Resource": "arn:aws:secretsmanager:us-east-1:123456789:secret:tourmanagement/database*"
    }
  ]
}
```

---

## Health Checks

### Application Health Endpoints

The application exposes two health check endpoints:

1. **`/health`** - Basic health check
   - Checks if application is running
   - Validates database connectivity

2. **`/ready`** - Readiness check
   - More comprehensive check
   - Ensures all dependencies are available

### Testing Health Endpoints

```bash
# Basic health check
curl http://your-alb-dns/health

# Readiness check
curl http://your-alb-dns/ready
```

**Expected Response:**
```
Healthy
```

### ECS Service Health Checks

When using an Application Load Balancer, configure Target Group health checks:

```bash
aws elbv2 modify-target-group \
  --target-group-arn arn:aws:elasticloadbalancing:... \
  --health-check-protocol HTTP \
  --health-check-path /health \
  --health-check-interval-seconds 30 \
  --health-check-timeout-seconds 5 \
  --healthy-threshold-count 2 \
  --unhealthy-threshold-count 3
```

**Health Check Configuration:**
- **Protocol**: HTTP
- **Path**: `/health`
- **Interval**: 30 seconds
- **Timeout**: 5 seconds
- **Healthy Threshold**: 2 consecutive successes
- **Unhealthy Threshold**: 3 consecutive failures

---

## Monitoring and Logging

### CloudWatch Logs

**View Logs:**
```bash
# Tail logs in real-time
aws logs tail /ecs/tourmanagement --follow --region us-east-1

# Filter logs by time range
aws logs tail /ecs/tourmanagement \
  --since 1h \
  --format short \
  --region us-east-1

# Search logs
aws logs filter-log-events \
  --log-group-name /ecs/tourmanagement \
  --filter-pattern "ERROR" \
  --region us-east-1
```

### CloudWatch Metrics

**Monitor ECS Service Metrics:**
```bash
# CPU Utilization
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-01T23:59:59Z \
  --period 3600 \
  --statistics Average \
  --region us-east-1

# Memory Utilization
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name MemoryUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-01T23:59:59Z \
  --period 3600 \
  --statistics Average \
  --region us-east-1
```

### Application Insights (Optional)

For advanced monitoring, integrate Application Insights:

**Add NuGet Package:**
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**Configure in Program.cs:**
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

**Set Environment Variable:**
```json
{
  "name": "APPLICATIONINSIGHTS_CONNECTION_STRING",
  "value": "InstrumentationKey=your-key;IngestionEndpoint=https://..."
}
```

---

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptoms:**
- Tasks continuously start and stop
- Status shows `STOPPED` or `DEPROVISIONING`

**Diagnosis:**
```bash
# Get stopped task details
aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --desired-status STOPPED \
  --region us-east-1

# Describe stopped task
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks task-id \
  --region us-east-1
```

**Common Causes:**
- Invalid CPU/memory combination
- Image not found or pull failed
- Execution role lacks ECR permissions
- Application crashes on startup

**Solutions:**
- Verify CPU/memory combination is valid for Fargate
- Check image URI and ECR permissions
- Review CloudWatch logs for application errors
- Ensure database connectivity

#### 2. Cannot Pull Container Image

**Symptoms:**
- Task fails with "CannotPullContainerError"

**Solutions:**
```bash
# Verify image exists in ECR
aws ecr describe-images \
  --repository-name tourmanagement \
  --region us-east-1

# Check execution role has ECR permissions
aws iam get-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-name AmazonECSTaskExecutionRolePolicy

# Verify image URI format
# Correct: 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest
```

#### 3. Network Connectivity Issues

**Symptoms:**
- Cannot access application
- Tasks cannot connect to database

**Solutions:**
```bash
# Verify security group rules
aws ec2 describe-security-groups \
  --group-ids sg-xxx \
  --region us-east-1

# Check if subnets have internet gateway route (for public access)
aws ec2 describe-route-tables \
  --filters "Name=association.subnet-id,Values=subnet-xxx" \
  --region us-east-1

# Verify assignPublicIp is ENABLED if using public subnets
aws ecs describe-services \
  --cluster tourmanagement-cluster \
  --services tourmanagement-service \
  --query 'services[0].networkConfiguration' \
  --region us-east-1
```

**Security Group Checklist:**
- Inbound rule for port 8080 (or ALB port 80)
- Outbound rule for database port (1433)
- Outbound rule for HTTPS (443) to pull images

#### 4. Health Check Failures

**Symptoms:**
- ALB marks targets as unhealthy
- Tasks restart frequently

**Diagnosis:**
```bash
# Check target health
aws elbv2 describe-target-health \
  --target-group-arn arn:aws:elasticloadbalancing:... \
  --region us-east-1

# Test health endpoint directly
curl http://task-public-ip:8080/health
```

**Solutions:**
- Increase `healthCheckGracePeriodSeconds` (e.g., 300)
- Verify health endpoint returns HTTP 200
- Check application logs for startup errors
- Ensure database migrations complete before health checks

#### 5. Database Connection Failures

**Symptoms:**
- Application logs show "Cannot connect to database"
- Health checks fail

**Solutions:**
```bash
# Test database connectivity from task
aws ecs execute-command \
  --cluster tourmanagement-cluster \
  --task task-id \
  --container tourmanagement \
  --command "/bin/bash" \
  --interactive

# Inside container:
apt-get update && apt-get install -y telnet
telnet your-rds-endpoint 1433
```

**Checklist:**
- Database security group allows connections from ECS security group
- Connection string is correct in environment variables
- Database credentials are valid
- RDS instance is in same VPC or accessible via VPC peering

#### 6. High CPU or Memory Usage

**Symptoms:**
- Tasks are killed with "OutOfMemory" error
- CPU utilization consistently at 100%

**Solutions:**
```bash
# Review metrics
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name MemoryUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average,Maximum \
  --region us-east-1

# Update task definition with higher resources
# Change cpu: "512" to "1024"
# Change memory: "1024" to "2048"
```

### Debugging Tools

**Enable ECS Exec for SSH-like access:**

```bash
# Update service to enable execute command
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --enable-execute-command \
  --region us-east-1

# Connect to running task
aws ecs execute-command \
  --cluster tourmanagement-cluster \
  --task task-id \
  --container tourmanagement \
  --command "/bin/bash" \
  --interactive \
  --region us-east-1
```

**View detailed task logs:**

```bash
# Get log stream name
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks task-id \
  --region us-east-1 \
  --query 'tasks[0].containers[0].name' \
  --output text

# View logs
aws logs get-log-events \
  --log-group-name /ecs/tourmanagement \
  --log-stream-name ecs/tourmanagement/task-id \
  --region us-east-1
```

---

## Security Considerations

### 1. Container Security

**Use Non-Root User:**
The Dockerfile creates and uses a non-root user (`appuser`) for running the application:

```dockerfile
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser
```

**Minimal Base Image:**
Use official Microsoft ASP.NET runtime images (not SDK) for production.

### 2. Network Security

**Security Group Best Practices:**
- Restrict inbound traffic to necessary ports only
- Use private subnets with NAT Gateway for production
- Limit database access to ECS security group only

**Example Security Group Configuration:**
```bash
# Application security group
aws ec2 authorize-security-group-ingress \
  --group-id sg-app \
  --protocol tcp \
  --port 8080 \
  --source-group sg-alb

# Database security group
aws ec2 authorize-security-group-ingress \
  --group-id sg-database \
  --protocol tcp \
  --port 1433 \
  --source-group sg-app
```

### 3. Secrets Management

**Never hardcode secrets in:**
- Dockerfile
- Task definition (use AWS Secrets Manager)
- Application code
- Environment variables (visible in console)

**Use AWS Secrets Manager:**
```bash
# Create secret
aws secretsmanager create-secret \
  --name tourmanagement/database \
  --secret-string '{"username":"admin","password":"SecurePass123!"}'

# Grant task execution role access
aws iam put-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-name SecretsManagerAccess \
  --policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Action": "secretsmanager:GetSecretValue",
      "Resource": "arn:aws:secretsmanager:us-east-1:123456789:secret:tourmanagement/database*"
    }]
  }'
```

### 4. IAM Best Practices

**Principle of Least Privilege:**
- Task execution role: Only ECR pull and CloudWatch logs write
- Task role: Only specific AWS services the application needs

**Example Task Role Policy:**
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:PutObject"
      ],
      "Resource": "arn:aws:s3:::tourmanagement-uploads/*"
    }
  ]
}
```

### 5. Logging and Auditing

**Enable CloudTrail:**
```bash
aws cloudtrail create-trail \
  --name tourmanagement-trail \
  --s3-bucket-name my-cloudtrail-bucket

aws cloudtrail start-logging --name tourmanagement-trail
```

**Enable VPC Flow Logs:**
```bash
aws ec2 create-flow-logs \
  --resource-type VPC \
  --resource-ids vpc-xxx \
  --traffic-type ALL \
  --log-destination-type cloud-watch-logs \
  --log-group-name /aws/vpc/flowlogs
```

### 6. HTTPS Configuration

**Use ACM Certificate with ALB:**
```bash
# Request certificate
aws acm request-certificate \
  --domain-name tourmanagement.example.com \
  --validation-method DNS \
  --region us-east-1

# Create HTTPS listener
aws elbv2 create-listener \
  --load-balancer-arn arn:aws:elasticloadbalancing:... \
  --protocol HTTPS \
  --port 443 \
  --certificates CertificateArn=arn:aws:acm:... \
  --default-actions Type=forward,TargetGroupArn=arn:aws:elasticloadbalancing:...
```

---

## Scaling and Performance

### Auto Scaling Configuration

**Create Auto Scaling Target:**
```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1
```

**CPU-Based Auto Scaling:**
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
    "ScaleInCooldown": 300,
    "ScaleOutCooldown": 60
  }' \
  --region us-east-1
```

**Memory-Based Auto Scaling:**
```bash
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name memory-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 80.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageMemoryUtilization"
    },
    "ScaleInCooldown": 300,
    "ScaleOutCooldown": 60
  }' \
  --region us-east-1
```

### Performance Tuning

**1. .NET Runtime Optimizations:**

**Enable ReadyToRun (R2R):**
```xml
<!-- Add to .csproj -->
<PropertyGroup>
  <PublishReadyToRun>true</PublishReadyToRun>
</PropertyGroup>
```

**Configure Garbage Collection:**
```json
// appsettings.Production.json
{
  "System.GC.Server": true,
  "System.GC.Concurrent": true
}
```

**2. Database Connection Pooling:**

```csharp
// Ensure connection string includes pooling settings
"Server=...;Pooling=true;Min Pool Size=5;Max Pool Size=100;Connect Timeout=30;"
```

**3. Response Caching:**

```csharp
// Program.cs
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

**4. Application Insights Performance Monitoring:**

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Blue/Green Deployments

**Using ECS Service Deployment:**

```bash
# Update service with new task definition
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --task-definition tourmanagement-task:2 \
  --deployment-configuration "maximumPercent=200,minimumHealthyPercent=100" \
  --force-new-deployment \
  --region us-east-1
```

**Deployment Circuit Breaker** (automatically rolls back failed deployments):

```json
"deploymentConfiguration": {
  "deploymentCircuitBreaker": {
    "enable": true,
    "rollback": true
  }
}
```

### Cost Optimization

**1. Use Fargate Spot for non-critical workloads:**

```bash
aws ecs create-service \
  --capacity-provider-strategy capacityProvider=FARGATE_SPOT,weight=1,base=0 \
  ...
```

**2. Right-size CPU and Memory:**
- Monitor actual usage with CloudWatch
- Start with smaller sizes and scale up as needed
- Use auto-scaling to handle peaks

**3. Optimize Container Image Size:**
- Use multi-stage builds (already implemented)
- Remove unnecessary dependencies
- Use .dockerignore to exclude build artifacts

**4. Schedule scaling for predictable workloads:**

```bash
aws application-autoscaling put-scheduled-action \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --schedule "cron(0 8 * * ? *)" \
  --scalable-target-action MinCapacity=5,MaxCapacity=10
```

---

## Additional Resources

### Official Documentation

- [AWS ECS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [AWS ECR Documentation](https://docs.aws.amazon.com/AmazonECR/latest/userguide/what-is-ecr.html)

### Useful AWS CLI Commands

```bash
# List all ECS clusters
aws ecs list-clusters --region us-east-1

# List services in cluster
aws ecs list-services --cluster tourmanagement-cluster --region us-east-1

# Scale service manually
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --desired-count 5 \
  --region us-east-1

# Stop a task
aws ecs stop-task \
  --cluster tourmanagement-cluster \
  --task task-id \
  --reason "Manual intervention" \
  --region us-east-1

# Delete service (must scale to 0 first)
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --desired-count 0 \
  --region us-east-1

aws ecs delete-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --region us-east-1

# Delete cluster
aws ecs delete-cluster --cluster tourmanagement-cluster --region us-east-1
```

---

## Support and Feedback

For issues, questions, or contributions:

1. **Application Issues**: Check CloudWatch logs and application health endpoints
2. **Deployment Issues**: Review ECS service events and task stopped reasons
3. **AWS Support**: Contact AWS Support for infrastructure-related issues
4. **Documentation Updates**: Submit pull requests for improvements

---

**Last Updated**: January 2026
**Version**: 1.0.0
**Maintained By**: DevOps Team