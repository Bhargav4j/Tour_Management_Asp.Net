# TourManagement Web Application - AWS ECS Fargate Deployment Guide

## Overview

This guide provides comprehensive instructions for deploying the TourManagement ASP.NET Core 8.0 web application to AWS ECS Fargate. The application is a Razor Pages-based tour management system with PostgreSQL database integration, Serilog logging, and health check endpoints.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Architecture Overview](#architecture-overview)
3. [Local Development Setup](#local-development-setup)
4. [Docker Deployment](#docker-deployment)
5. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
6. [ECS Fargate Setup](#ecs-fargate-setup)
7. [ECS Task Definition Explained](#ecs-task-definition-explained)
8. [ECS Service Configuration](#ecs-service-configuration)
9. [Deployment Process](#deployment-process)
10. [Configuration Management](#configuration-management)
11. [Monitoring and Logging](#monitoring-and-logging)
12. [Troubleshooting](#troubleshooting)
13. [Scaling and Performance](#scaling-and-performance)
14. [Security Considerations](#security-considerations)

---

## Prerequisites

### Required Tools

- **.NET 8.0 SDK**: For local development and building
- **Docker**: Version 20.10 or later
- **AWS CLI**: Version 2.x configured with appropriate credentials
- **Git**: For version control

### Required Knowledge

- Basic understanding of ASP.NET Core and Razor Pages
- Docker containerization concepts
- AWS ECS Fargate and networking fundamentals
- PostgreSQL database administration

### AWS Account Requirements

- AWS account with appropriate permissions
- IAM user or role with ECS, ECR, EC2, VPC, and CloudWatch permissions
- Configured AWS CLI with valid credentials

---

## Architecture Overview

### Application Stack

- **Framework**: ASP.NET Core 8.0
- **Application Type**: Razor Pages Web Application
- **Database**: PostgreSQL (external, not containerized)
- **Logging**: Serilog with console output
- **ORM**: Entity Framework Core with Npgsql provider
- **Health Checks**: ASP.NET Core health check middleware
- **Session Management**: In-memory distributed cache

### Container Architecture

- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:8.0`
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:8.0`
- **Application Port**: 8080 (HTTP)
- **Health Endpoint**: `/health` and `/health/ready`
- **User**: Non-root user (appuser) for security

### AWS ECS Fargate Architecture

- **Launch Type**: AWS Fargate (serverless)
- **Network Mode**: awsvpc (required for Fargate)
- **CPU**: 512 (.5 vCPU)
- **Memory**: 1024 MB (1 GB)
- **Load Balancer**: Application Load Balancer (optional)
- **Target Type**: IP (required for Fargate awsvpc mode)

---

## Local Development Setup

### Clone Repository

```bash
git clone <repository-url>
cd TourContainer0302Cmp
```

### Configure Database

1. Set up PostgreSQL locally or use a managed instance
2. Update connection string in `src/TourManagement.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tourmanagementdb;Username=postgres;Password=yourpassword"
  }
}
```

### Run Locally

```bash
cd src/TourManagement.Web
dotnet restore
dotnet build
dotnet run
```

Access the application at `http://localhost:5000` (or configured port).

---

## Docker Deployment

### Build Docker Image

```bash
# From repository root
docker build -f Dockerfile -t tourmanagement-web:latest .
```

### Run with Docker Compose

```bash
# Set environment variables
export DB_HOST=your-db-host
export DB_PORT=5432
export DB_NAME=tourmanagementdb
export DB_USER=postgres
export DB_PASSWORD=yourpassword

# Start application
docker-compose up -d
```

Access the application at `http://localhost:8080`.

### Verify Health

```bash
curl http://localhost:8080/health
```

---

## AWS ECS Fargate Prerequisites

### 1. VPC Configuration

Ensure you have a VPC with:
- At least 2 subnets in different availability zones
- Internet Gateway attached (for public access)
- Route table with route to Internet Gateway

```bash
# List VPCs
aws ec2 describe-vpcs --region us-east-1

# List subnets
aws ec2 describe-subnets --region us-east-1
```

### 2. Security Group

Create a security group allowing:
- Inbound: Port 8080 (application) from ALB security group or 0.0.0.0/0
- Inbound: Port 5432 (PostgreSQL) to database security group
- Outbound: All traffic

```bash
aws ec2 create-security-group \
  --group-name tourmanagement-sg \
  --description "Security group for TourManagement ECS tasks" \
  --vpc-id vpc-xxxxxx \
  --region us-east-1

aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxxx \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0 \
  --region us-east-1
```

### 3. IAM Roles

#### ECS Task Execution Role

Create `ecsTaskExecutionRole` with policies:
- `AmazonECSTaskExecutionRolePolicy` (AWS managed)
- ECR pull permissions
- CloudWatch Logs write permissions

```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document file://ecs-task-execution-role-trust-policy.json

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

**Trust Policy** (`ecs-task-execution-role-trust-policy.json`):

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

#### ECS Task Role (Optional)

Create `ecsTaskRole` for application-level permissions (e.g., S3, SQS access).

### 4. Amazon ECR Repository

Create ECR repository for Docker images:

```bash
aws ecr create-repository \
  --repository-name tourmanagement-web \
  --region us-east-1
```

### 5. CloudWatch Log Group

Create log group for application logs:

```bash
aws logs create-log-group \
  --log-group-name /ecs/tourmanagement-web \
  --region us-east-1
```

### 6. Database Setup

Provision PostgreSQL database:
- **AWS RDS PostgreSQL**: Recommended for production
- **Aurora PostgreSQL**: For high availability
- **Self-managed EC2**: For custom requirements

Ensure:
- Database is accessible from ECS tasks (security group rules)
- Connection string credentials are available
- Database schema is initialized

---

## ECS Fargate Setup

### 1. Create ECS Cluster

```bash
aws ecs create-cluster \
  --cluster-name tourmanagement-cluster \
  --region us-east-1
```

### 2. Build and Push Docker Image

Use the provided scripts:

**Linux/macOS**:
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows**:
```cmd
scripts\build-push.bat
```

The script will:
1. Prompt for registry type (ECR or Docker Hub)
2. Authenticate with selected registry
3. Build Docker image
4. Tag and push image

---

## ECS Task Definition Explained

### Fargate-Specific Configuration

```json
{
  "family": "tourmanagement-web-task",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "512",
  "memory": "1024"
}
```

**Key Points**:
- `networkMode: awsvpc` - Only valid mode for Fargate
- `requiresCompatibilities: ["FARGATE"]` - Ensures Fargate compatibility
- CPU/Memory must use valid Fargate combinations

### Valid Fargate CPU/Memory Combinations

| CPU (vCPU) | Memory (MB) |
|------------|-------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

### Container Definition

```json
{
  "name": "tourmanagement-web",
  "image": "123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement-web:latest",
  "essential": true,
  "portMappings": [
    {
      "containerPort": 8080,
      "protocol": "tcp"
    }
  ]
}
```

**Key Points**:
- No `hostPort` needed for Fargate (automatically managed)
- `essential: true` - Container failure causes task to stop

### Environment Variables

Database connection configured via environment variables:

```json
"environment": [
  {"name": "ASPNETCORE_ENVIRONMENT", "value": "Production"},
  {"name": "ASPNETCORE_URLS", "value": "http://+:8080"},
  {"name": "DB_HOST", "value": "db.example.com"},
  {"name": "DB_PORT", "value": "5432"},
  {"name": "DB_NAME", "value": "tourmanagementdb"},
  {"name": "DB_USER", "value": "postgres"},
  {"name": "DB_PASSWORD", "value": "secret"}
]
```

**Security Note**: Use AWS Secrets Manager or SSM Parameter Store for sensitive values in production.

### Health Check

```json
"healthCheck": {
  "command": ["CMD-SHELL", "wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1"],
  "interval": 30,
  "timeout": 5,
  "retries": 3,
  "startPeriod": 60
}
```

**Configuration**:
- `interval`: Check every 30 seconds
- `timeout`: 5 seconds per check
- `retries`: 3 consecutive failures before unhealthy
- `startPeriod`: 60 seconds grace period for startup

### CloudWatch Logging

```json
"logConfiguration": {
  "logDriver": "awslogs",
  "options": {
    "awslogs-group": "/ecs/tourmanagement-web",
    "awslogs-region": "us-east-1",
    "awslogs-stream-prefix": "ecs"
  }
}
```

---

## ECS Service Configuration

### Fargate Launch Type

```json
{
  "serviceName": "tourmanagement-web-service",
  "cluster": "tourmanagement-cluster",
  "taskDefinition": "tourmanagement-web-task",
  "desiredCount": 2,
  "launchType": "FARGATE"
}
```

### Network Configuration

```json
"networkConfiguration": {
  "awsvpcConfiguration": {
    "subnets": ["subnet-abc123", "subnet-def456"],
    "securityGroups": ["sg-xyz789"],
    "assignPublicIp": "ENABLED"
  }
}
```

**Key Points**:
- `assignPublicIp: ENABLED` - Required if tasks need internet access without NAT Gateway
- Use private subnets with NAT Gateway for production

### Deployment Configuration

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

**Rolling Deployment**:
- Starts new tasks before stopping old ones
- Ensures at least 50% healthy during deployment
- Circuit breaker automatically rolls back failed deployments

### Load Balancer Integration

```json
"loadBalancers": [
  {
    "targetGroupArn": "arn:aws:elasticloadbalancing:...",
    "containerName": "tourmanagement-web",
    "containerPort": 8080
  }
],
"healthCheckGracePeriodSeconds": 300
```

**Important**:
- Target Group must use `target-type: ip` (required for Fargate awsvpc mode)
- Health check grace period prevents premature task termination during startup

---

## Deployment Process

### Step 1: Build and Push Image

```bash
# Linux/macOS
./scripts/build-push.sh

# Windows
scripts\build-push.bat
```

### Step 2: Deploy to ECS

```bash
# Linux/macOS
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh

# Windows
scripts\deploy-image.bat
```

### Step 3: Deployment Script Workflow

The deployment script will:

1. **Prompt for Configuration**:
   - AWS region
   - ECS cluster name
   - VPC ID
   - Subnet IDs (comma-separated)
   - Security Group ID
   - Docker image URI
   - Database configuration

2. **Load Balancer Setup** (optional):
   - Creates Application Load Balancer
   - Creates Target Group with `target-type: ip`
   - Configures listener on port 80
   - Returns ALB DNS name for access

3. **Task Definition Registration**:
   - Replaces placeholders with provided values
   - Registers task definition with ECS
   - Returns task definition ARN

4. **Service Creation/Update**:
   - Checks if service exists
   - Creates new service or updates existing
   - Waits for service stability

5. **Verification**:
   - Displays service status
   - Shows running/desired task counts
   - Provides application URL (if ALB enabled)
   - Shows CloudWatch log group

### Step 4: Verify Deployment

```bash
# Check service status
aws ecs describe-services \
  --cluster tourmanagement-cluster \
  --services tourmanagement-web-service \
  --region us-east-1

# List running tasks
aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-web-service \
  --region us-east-1

# Check task health
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks <task-id> \
  --region us-east-1
```

### Step 5: Access Application

**With Load Balancer**:
```bash
http://<alb-dns-name>
```

**Without Load Balancer** (direct task access):
```bash
# Get task public IP
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks <task-id> \
  --region us-east-1 \
  --query 'tasks[0].attachments[0].details[?name==`networkInterfaceId`].value' \
  --output text

aws ec2 describe-network-interfaces \
  --network-interface-ids <eni-id> \
  --query 'NetworkInterfaces[0].Association.PublicIp' \
  --output text

http://<public-ip>:8080
```

---

## Configuration Management

### Environment-Specific Settings

The application uses environment variables for configuration, supporting multiple environments:

- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.json` + environment variables

### Database Connection String

Connection string uses environment variable substitution:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
}
```

### Using AWS Secrets Manager

For production, store sensitive values in Secrets Manager:

```bash
# Create secret
aws secretsmanager create-secret \
  --name tourmanagement/database \
  --secret-string '{"username":"postgres","password":"secret"}' \
  --region us-east-1
```

**Task Definition with Secrets**:

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

---

## Monitoring and Logging

### CloudWatch Logs

Application logs are sent to CloudWatch Logs:

**Log Group**: `/ecs/tourmanagement-web`

**View Logs**:
```bash
aws logs tail /ecs/tourmanagement-web --follow --region us-east-1
```

**Filter Logs**:
```bash
aws logs filter-log-events \
  --log-group-name /ecs/tourmanagement-web \
  --filter-pattern "ERROR" \
  --region us-east-1
```

### CloudWatch Metrics

ECS automatically publishes metrics:
- CPU utilization
- Memory utilization
- Network in/out
- Task count

**View Metrics**:
```bash
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-web-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time 2023-01-01T00:00:00Z \
  --end-time 2023-01-01T23:59:59Z \
  --period 3600 \
  --statistics Average \
  --region us-east-1
```

### Application Insights (Optional)

For advanced monitoring, integrate Application Insights:

1. Install NuGet package:
   ```bash
   dotnet add package Microsoft.ApplicationInsights.AspNetCore
   ```

2. Configure in `Program.cs`:
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

3. Set instrumentation key via environment variable:
   ```json
   {"name": "APPLICATIONINSIGHTS_CONNECTION_STRING", "value": "..."}
   ```

---

## Troubleshooting

### Task Fails to Start

**Symptom**: Tasks repeatedly fail with "Essential container exited"

**Possible Causes**:
1. Invalid Docker image URI
2. Database connection failure
3. Missing environment variables
4. Application startup error

**Solution**:
```bash
# Check task stopped reason
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks <task-id> \
  --region us-east-1 \
  --query 'tasks[0].stoppedReason'

# Check CloudWatch logs
aws logs tail /ecs/tourmanagement-web --follow --region us-east-1
```

### Network Connectivity Issues

**Symptom**: Tasks cannot connect to database or external services

**Possible Causes**:
1. Incorrect security group rules
2. Subnet routing issues
3. No internet access (if public IP not assigned and no NAT Gateway)

**Solution**:
```bash
# Verify security group rules
aws ec2 describe-security-groups \
  --group-ids sg-xxxxxx \
  --region us-east-1

# Check subnet route table
aws ec2 describe-route-tables \
  --filters "Name=association.subnet-id,Values=subnet-xxxxxx" \
  --region us-east-1
```

### Invalid CPU/Memory Combination

**Symptom**: Task definition registration fails with "Invalid CPU or memory value"

**Solution**: Ensure CPU/memory combination is valid for Fargate (see [ECS Task Definition Explained](#ecs-task-definition-explained)).

### Health Check Failures

**Symptom**: Tasks marked unhealthy and restarted

**Possible Causes**:
1. Application startup too slow
2. Health endpoint not responding
3. Database connection issues

**Solution**:
```bash
# Increase startPeriod in health check
"startPeriod": 120  # Increase to 120 seconds

# Test health endpoint locally
curl http://<task-public-ip>:8080/health
```

### Load Balancer Target Unhealthy

**Symptom**: ALB marks targets as unhealthy

**Possible Causes**:
1. Health check path incorrect
2. Security group blocking ALB traffic
3. Application not listening on correct port

**Solution**:
```bash
# Check target health
aws elbv2 describe-target-health \
  --target-group-arn <target-group-arn> \
  --region us-east-1

# Verify target group health check configuration
aws elbv2 describe-target-groups \
  --target-group-arns <target-group-arn> \
  --region us-east-1
```

---

## Scaling and Performance

### Service Auto Scaling

Configure auto scaling based on metrics:

```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-web-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1

# Create scaling policy (CPU-based)
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-web-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json \
  --region us-east-1
```

**scaling-policy.json**:
```json
{
  "TargetValue": 70.0,
  "PredefinedMetricSpecification": {
    "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
  },
  "ScaleInCooldown": 300,
  "ScaleOutCooldown": 60
}
```

### Task Size Optimization

For better performance, consider increasing task resources:

```json
{
  "cpu": "1024",
  "memory": "2048"
}
```

### .NET Performance Tuning

**Enable ReadyToRun (R2R)**:

Add to `.csproj`:
```xml
<PropertyGroup>
  <PublishReadyToRun>true</PublishReadyToRun>
</PropertyGroup>
```

**Optimize Garbage Collection**:

Set environment variables:
```json
{"name": "DOTNET_gcServer", "value": "1"},
{"name": "DOTNET_GCConserveMemory", "value": "5"}
```

---

## Security Considerations

### 1. Use Non-Root User

Dockerfile already creates non-root user:
```dockerfile
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser
```

### 2. Secrets Management

**Never** hardcode secrets in:
- Dockerfiles
- Environment variables in task definitions
- Source code

Use AWS Secrets Manager or SSM Parameter Store.

### 3. Network Security

- Use private subnets with NAT Gateway for production
- Restrict security group rules to minimum required
- Enable VPC Flow Logs for auditing

### 4. Image Security

- Scan images for vulnerabilities:
  ```bash
  aws ecr start-image-scan \
    --repository-name tourmanagement-web \
    --image-id imageTag=latest \
    --region us-east-1
  ```

- Use immutable tags in ECR:
  ```bash
  aws ecr put-image-tag-mutability \
    --repository-name tourmanagement-web \
    --image-tag-mutability IMMUTABLE \
    --region us-east-1
  ```

### 5. IAM Least Privilege

- Use task roles for application permissions
- Limit execution role to ECR pull and CloudWatch write

### 6. Enable ECS Exec for Debugging

```bash
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-web-service \
  --enable-execute-command \
  --region us-east-1

# Connect to running task
aws ecs execute-command \
  --cluster tourmanagement-cluster \
  --task <task-id> \
  --container tourmanagement-web \
  --interactive \
  --command "/bin/bash"
```

---

## Blue/Green Deployment

For zero-downtime deployments, use AWS CodeDeploy:

```bash
# Update service to use CODE_DEPLOY deployment controller
aws ecs create-service \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-web-service \
  --deployment-controller type=CODE_DEPLOY \
  --cli-input-json file://service-definition.json
```

---

## Summary

This guide covered:

✅ Prerequisites and AWS account setup
✅ Local development with .NET 8.0
✅ Docker containerization and multi-stage builds
✅ AWS ECS Fargate deployment architecture
✅ Task definition and service configuration
✅ Automated deployment scripts
✅ Monitoring with CloudWatch
✅ Troubleshooting common issues
✅ Auto scaling and performance optimization
✅ Security best practices

For additional support:
- **AWS ECS Documentation**: https://docs.aws.amazon.com/ecs/
- **ASP.NET Core Documentation**: https://docs.microsoft.com/aspnet/core/
- **.NET Container Documentation**: https://learn.microsoft.com/dotnet/core/docker/

---

**Version**: 1.0  
**Last Updated**: 2026-02-03  
**Maintained By**: DevOps Team