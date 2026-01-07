# Tour Management Application - AWS ECS Fargate Deployment Guide

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Application Architecture](#application-architecture)
4. [Local Development](#local-development)
5. [Docker Deployment](#docker-deployment)
6. [AWS ECS Fargate Deployment](#aws-ecs-fargate-deployment)
7. [Configuration Management](#configuration-management)
8. [Security Considerations](#security-considerations)
9. [Troubleshooting](#troubleshooting)
10. [Monitoring and Logging](#monitoring-and-logging)

## Overview

Tour Management is an ASP.NET Core 8.0 Razor Pages application designed for managing tours. This guide covers containerization and deployment to AWS ECS Fargate.

**Technology Stack:**
- .NET 8.0
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0 with SQL Server
- Serilog for structured logging
- BCrypt.Net for password hashing

**Application Port:** 8080  
**Health Check Endpoints:** `/health`, `/ready`

## Prerequisites

### Required Tools

1. **Docker Desktop** (v20.10 or later)
   - Download: https://www.docker.com/products/docker-desktop
   - Verify: `docker --version`

2. **AWS CLI** (v2.x)
   - Download: https://aws.amazon.com/cli/
   - Verify: `aws --version`
   - Configure: `aws configure`

3. **.NET SDK 8.0** (for local development)
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify: `dotnet --version`

### AWS Requirements

1. **AWS Account** with appropriate permissions
2. **IAM Roles:**
   - `ecsTaskExecutionRole` - For ECS to pull images and write logs
   - `ecsTaskRole` - For application to access AWS services (optional)

3. **VPC Configuration:**
   - VPC with at least 2 subnets (preferably in different AZs)
   - Security Group allowing:
     - Inbound: HTTP (80), Application Port (8080)
     - Outbound: All traffic (or specific database/service ports)

4. **Database:**
   - SQL Server instance (RDS, EC2, or on-premises)
   - Connection string parameters stored in AWS Secrets Manager

## Application Architecture

### Project Structure

```
TourManagement/
├── src/
│   ├── TourManagement.Web/          # ASP.NET Core Web Application
│   ├── TourManagement.Application/  # Application Logic Layer
│   ├── TourManagement.Domain/       # Domain Models
│   └── TourManagement.Infrastructure/ # Data Access and Infrastructure
├── tests/
│   └── TourManagement.UnitTests/    # Unit Tests
├── Dockerfile                        # Multi-stage Docker build
├── docker-compose.yml                # Local development compose
├── scripts/
│   ├── build-push.sh                 # Build and push script (Linux/macOS)
│   ├── build-push.bat                # Build and push script (Windows)
│   ├── deploy-image.sh               # ECS deployment script (Linux/macOS)
│   └── deploy-image.bat              # ECS deployment script (Windows)
└── ecs/
    ├── task-definition.json          # ECS Task Definition
    └── service-definition.json       # ECS Service Definition
```

### Multi-Stage Dockerfile

The Dockerfile uses a multi-stage build:

1. **Builder Stage:** Uses `mcr.microsoft.com/dotnet/sdk:8.0`
   - Restores NuGet packages
   - Builds the solution in Release mode
   - Publishes the application

2. **Runtime Stage:** Uses `mcr.microsoft.com/dotnet/aspnet:8.0`
   - Creates non-root user for security
   - Copies published artifacts
   - Sets environment variables
   - Exposes port 8080

## Local Development

### Running with .NET CLI

```bash
cd src/TourManagement.Web
dotnet restore
dotnet build
dotnet run
```

Access the application at: `http://localhost:5000` or `https://localhost:5001`

### Running with Docker Compose

```bash
# Build and start the application
docker-compose up --build

# Access at http://localhost:8080

# Stop the application
docker-compose down
```

### Environment Variables

Create a `.env` file for local development:

```env
ASPNETCORE_ENVIRONMENT=Development
DB_SERVER=your-db-server.database.windows.net
DB_NAME=TourManagement
DB_USER=your-db-user
DB_PASSWORD=your-db-password
```

## Docker Deployment

### Building the Docker Image

**Linux/macOS:**
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows:**
```cmd
scripts\build-push.bat
```

The script will:
1. Prompt for registry selection (AWS ECR or Docker Hub)
2. Sanitize image names and tags
3. Authenticate with the selected registry
4. Build the Docker image
5. Push to the registry

### Manual Docker Build

```bash
# Build the image
docker build -t tourmanagement-web:latest .

# Run the container
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e DB_SERVER=your-db-server \
  -e DB_NAME=TourManagement \
  -e DB_USER=your-user \
  -e DB_PASSWORD=your-password \
  --name tourmanagement \
  tourmanagement-web:latest

# Check logs
docker logs -f tourmanagement

# Stop and remove
docker stop tourmanagement
docker rm tourmanagement
```

## AWS ECS Fargate Deployment

### Step 1: Prepare AWS Resources

#### Create IAM Roles

**Task Execution Role:**
```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document file://trust-policy.json

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

**Task Role (optional):**
```bash
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document file://trust-policy.json

# Attach policies as needed for your application
```

#### Store Database Credentials in Secrets Manager

```bash
aws secretsmanager create-secret \
  --name tourmanagement/db-server \
  --secret-string "your-db-server.database.windows.net" \
  --region us-east-1

aws secretsmanager create-secret \
  --name tourmanagement/db-name \
  --secret-string "TourManagement" \
  --region us-east-1

aws secretsmanager create-secret \
  --name tourmanagement/db-user \
  --secret-string "your-db-user" \
  --region us-east-1

aws secretsmanager create-secret \
  --name tourmanagement/db-password \
  --secret-string "your-db-password" \
  --region us-east-1
```

#### Configure Security Group

```bash
# Create security group
SG_ID=$(aws ec2 create-security-group \
  --group-name tourmanagement-sg \
  --description "Security group for Tour Management application" \
  --vpc-id vpc-xxxxx \
  --query 'GroupId' \
  --output text)

# Allow HTTP traffic
aws ec2 authorize-security-group-ingress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 80 \
  --cidr 0.0.0.0/0

# Allow application port
aws ec2 authorize-security-group-ingress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0

# Allow outbound to database (example for SQL Server)
aws ec2 authorize-security-group-egress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 1433 \
  --cidr 10.0.0.0/16
```

### Step 2: Build and Push Docker Image

Run the build-push script to create and upload your Docker image to ECR:

**Linux/macOS:**
```bash
./scripts/build-push.sh
```

**Windows:**
```cmd
scripts\build-push.bat
```

Select option 1 for AWS ECR and provide:
- AWS Region (e.g., us-east-1)
- AWS Account ID
- ECR Repository Name (e.g., tourmanagement-web)
- Image Tag (e.g., latest or v1.0.0)

### Step 3: Deploy to ECS Fargate

Run the deployment script:

**Linux/macOS:**
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

**Windows:**
```cmd
scripts\deploy-image.bat
```

Provide the following information when prompted:
- AWS Region
- ECS Cluster Name (will be created if doesn't exist)
- VPC ID
- Subnet IDs (comma-separated, at least 2)
- Security Group ID
- Docker Image URI (from Step 2)
- Load Balancer (y/n) - script will auto-create ALB and Target Group if yes

The script will:
1. Create ECS cluster (if needed)
2. Create Application Load Balancer and Target Group (if requested)
3. Register task definition
4. Create or update ECS service
5. Wait for service stability
6. Display deployment status and access information

### Understanding ECS Task Definition

**Key Configuration:**

```json
{
  "family": "tourmanagement-web-task",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "512",
  "memory": "1024",
  "executionRoleArn": "arn:aws:iam::ACCOUNT_ID:role/ecsTaskExecutionRole",
  "taskRoleArn": "arn:aws:iam::ACCOUNT_ID:role/ecsTaskRole"
}
```

**Valid Fargate CPU/Memory Combinations:**
- CPU: 256 (.25 vCPU) → Memory: 512, 1024, 2048 MB
- CPU: 512 (.5 vCPU) → Memory: 1024, 2048, 3072, 4096 MB
- CPU: 1024 (1 vCPU) → Memory: 2048-8192 MB (1024 MB increments)
- CPU: 2048 (2 vCPU) → Memory: 4096-16384 MB
- CPU: 4096 (4 vCPU) → Memory: 8192-30720 MB

**Container Health Check:**

The task definition includes a health check:
```json
"healthCheck": {
  "command": ["CMD-SHELL", "wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1"],
  "interval": 30,
  "timeout": 5,
  "retries": 3,
  "startPeriod": 60
}
```

### Understanding ECS Service Definition

**Key Configuration:**

```json
{
  "serviceName": "tourmanagement-web-service",
  "desiredCount": 2,
  "launchType": "FARGATE",
  "networkConfiguration": {
    "awsvpcConfiguration": {
      "subnets": ["subnet-xxx", "subnet-yyy"],
      "securityGroups": ["sg-xxx"],
      "assignPublicIp": "ENABLED"
    }
  },
  "loadBalancers": [
    {
      "targetGroupArn": "arn:aws:elasticloadbalancing:...",
      "containerName": "tourmanagement-web",
      "containerPort": 8080
    }
  ]
}
```

**Deployment Strategy:**
- `maximumPercent`: 200 - Allows 2x desired count during deployment
- `minimumHealthyPercent`: 50 - Keeps at least 50% of tasks running
- `deploymentCircuitBreaker` with rollback enabled

## Configuration Management

### Environment-Specific Configuration

The application uses `appsettings.json` and `appsettings.{Environment}.json`:

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

### Secrets Management

**AWS Secrets Manager** is used for sensitive data. The task definition references secrets:

```json
"secrets": [
  {
    "name": "DB_SERVER",
    "valueFrom": "arn:aws:secretsmanager:REGION:ACCOUNT_ID:secret:tourmanagement/db-server"
  }
]
```

**Best Practices:**
- Never commit secrets to version control
- Use AWS Secrets Manager or Parameter Store
- Rotate credentials regularly
- Use IAM roles for authentication when possible

## Security Considerations

### Container Security

1. **Non-Root User:** The Dockerfile creates and uses a non-root user
2. **Minimal Base Image:** Uses official Microsoft ASP.NET runtime image
3. **No Unnecessary Tools:** Runtime image doesn't include build tools
4. **Read-Only Filesystem:** Consider using read-only root filesystem

### Network Security

1. **Security Groups:**
   - Restrict inbound traffic to necessary ports only
   - Use specific CIDR blocks instead of 0.0.0.0/0 when possible
   - Separate security groups for ALB and ECS tasks

2. **VPC Configuration:**
   - Use private subnets for ECS tasks when possible
   - Use NAT Gateway for outbound internet access from private subnets
   - Enable VPC Flow Logs for network monitoring

3. **HTTPS/TLS:**
   - Configure ALB with HTTPS listener and SSL certificate
   - Redirect HTTP to HTTPS
   - Use AWS Certificate Manager (ACM) for SSL certificates

### Application Security

1. **Authentication:** Application uses BCrypt for password hashing
2. **Database Security:** Use SSL/TLS for database connections
3. **CORS:** Configure CORS policies appropriately
4. **Security Headers:** Add security headers middleware

## Troubleshooting

### Common ECS Issues

#### Task Fails to Start

**Check Task Stopped Reason:**
```bash
aws ecs describe-tasks \
  --cluster my-cluster \
  --tasks TASK_ARN \
  --query 'tasks[0].stoppedReason'
```

**Common Causes:**
- Invalid CPU/memory combination
- Image pull errors (check ECR permissions)
- Missing or incorrect execution role
- Health check failures

#### Cannot Pull Image from ECR

**Verify ECR Permissions:**
```bash
# Check execution role has AmazonECSTaskExecutionRolePolicy
aws iam list-attached-role-policies --role-name ecsTaskExecutionRole

# Verify ECR repository exists
aws ecr describe-repositories --repository-names tourmanagement-web
```

#### Service Not Reaching Healthy State

**Check Target Group Health:**
```bash
aws elbv2 describe-target-health \
  --target-group-arn TARGET_GROUP_ARN
```

**Common Causes:**
- Health check path incorrect
- Security group blocking traffic
- Application not listening on correct port
- Health check timeout too short for application startup

### Viewing Logs

**CloudWatch Logs:**
```bash
# Tail logs in real-time
aws logs tail /ecs/tourmanagement-web --follow --region us-east-1

# View specific time range
aws logs tail /ecs/tourmanagement-web \
  --since 1h \
  --region us-east-1

# Filter logs
aws logs tail /ecs/tourmanagement-web \
  --follow \
  --filter-pattern "ERROR" \
  --region us-east-1
```

**Application Logs:**

The application uses Serilog for structured logging. Logs are written to:
- Console (captured by CloudWatch)
- File (if configured)

### Database Connection Issues

**Verify Connectivity:**
```bash
# Run a test task with SQL Server tools
aws ecs run-task \
  --cluster my-cluster \
  --task-definition test-connectivity \
  --launch-type FARGATE
```

**Check Connection String:**
- Verify DB_SERVER, DB_NAME, DB_USER, DB_PASSWORD secrets
- Ensure security group allows outbound traffic to database port
- Verify database server allows connections from ECS tasks
- Check if TrustServerCertificate is needed

## Monitoring and Logging

### CloudWatch Metrics

**ECS Service Metrics:**
- CPUUtilization
- MemoryUtilization
- RunningTaskCount
- DesiredTaskCount

**Application Load Balancer Metrics:**
- RequestCount
- TargetResponseTime
- HTTPCode_Target_2XX_Count
- HTTPCode_Target_5XX_Count
- HealthyHostCount
- UnHealthyHostCount

**Create CloudWatch Dashboard:**
```bash
aws cloudwatch put-dashboard \
  --dashboard-name tourmanagement-dashboard \
  --dashboard-body file://dashboard.json
```

### Application Insights (Optional)

For advanced monitoring, consider integrating Application Insights:

1. Add NuGet package:
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

2. Configure in Program.cs:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

3. Set environment variable:
```json
{
  "name": "APPLICATIONINSIGHTS_CONNECTION_STRING",
  "value": "InstrumentationKey=xxx;..."
}
```

### Auto Scaling

**Configure Service Auto Scaling:**

```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/my-cluster/tourmanagement-web-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10

# Create scaling policy based on CPU
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/my-cluster/tourmanagement-web-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json
```

**scaling-policy.json:**
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

### Blue/Green Deployments

For zero-downtime deployments, consider using AWS CodeDeploy:

1. Create CodeDeploy application and deployment group
2. Configure deployment strategy (Linear, Canary, or All-at-once)
3. Integrate with CI/CD pipeline

## Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [AWS Well-Architected Framework](https://aws.amazon.com/architecture/well-architected/)

## Support and Maintenance

### Regular Maintenance Tasks

1. **Update Base Images:**
```bash
# Pull latest base images
docker pull mcr.microsoft.com/dotnet/sdk:8.0
docker pull mcr.microsoft.com/dotnet/aspnet:8.0

# Rebuild and redeploy
./scripts/build-push.sh
./scripts/deploy-image.sh
```

2. **Review CloudWatch Logs:** Regularly check for errors and warnings
3. **Monitor Costs:** Use AWS Cost Explorer to track ECS and related service costs
4. **Security Updates:** Keep .NET runtime and dependencies updated
5. **Backup Database:** Ensure regular automated backups of the SQL Server database

### Scaling Considerations

**Vertical Scaling:** Increase CPU/memory in task definition  
**Horizontal Scaling:** Increase desired count or configure auto-scaling  
**Database Scaling:** Consider read replicas or database sharding for high load

---

**Last Updated:** 2026-01-07  
**Version:** 1.0.0