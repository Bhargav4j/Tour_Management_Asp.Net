# TourManagement Application - AWS ECS Fargate Deployment Guide

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Overview](#project-overview)
3. [Local Development Setup](#local-development-setup)
4. [Docker Build and Push](#docker-build-and-push)
5. [AWS ECS Fargate Setup](#aws-ecs-fargate-setup)
6. [ECS Task Definition](#ecs-task-definition)
7. [ECS Service Configuration](#ecs-service-configuration)
8. [Deployment Process](#deployment-process)
9. [Monitoring and Logging](#monitoring-and-logging)
10. [Troubleshooting](#troubleshooting)
11. [Scaling and Management](#scaling-and-management)
12. [Security Considerations](#security-considerations)

---

## Prerequisites

### Required Tools

- **Docker**: Version 20.10 or later
- **AWS CLI**: Version 2.x configured with appropriate credentials
- **Git**: For cloning the repository
- **.NET 8.0 SDK**: For local development (optional)

### AWS Resources Required

1. **AWS Account** with appropriate permissions
2. **VPC** with at least 2 subnets in different availability zones
3. **Security Groups** configured to allow:
   - Inbound HTTP/HTTPS traffic (ports 80/443) from ALB
   - Outbound traffic to internet for pulling Docker images
   - Database connectivity (port 1433 for SQL Server)
4. **IAM Roles**:
   - `ecsTaskExecutionRole` - For ECS to pull images and write logs
   - `ecsTaskRole` - For application to access AWS services (optional)
5. **RDS SQL Server Instance** or external SQL Server database
6. **ElastiCache Redis** (optional, for distributed session storage)

### AWS IAM Permissions

Your AWS CLI user/role needs permissions for:
- ECS (create/update clusters, services, task definitions)
- ECR (push/pull Docker images)
- CloudWatch Logs (create log groups)
- ELB (create/manage load balancers and target groups)
- IAM (pass role to ECS tasks)
- VPC (describe subnets, security groups)

---

## Project Overview

### Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Application Type**: Razor Pages Web Application
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Web layers)
- **Database**: SQL Server with Entity Framework Core 8.0
- **Caching**: Redis (optional) or in-memory distributed cache
- **Logging**: Serilog with console and file sinks
- **Authentication**: BCrypt for password hashing

### Application Features

- Tour management system
- Database migrations on startup
- Health check endpoints: `/health` and `/ready`
- Session management with Redis support
- Structured logging with Serilog

### Port Configuration

- **Application Port**: 8080 (HTTP)
- **Health Endpoints**: `/health`, `/ready`

---

## Local Development Setup

### Clone the Repository

```bash
git clone <repository-url>
cd TMS27
```

### Configure Environment Variables

Create an `appsettings.Development.json` file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TourManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### Run Locally with Docker Compose

```bash
# Build and run the application
docker-compose up --build

# Access the application
http://localhost:8080

# Check health
http://localhost:8080/health
```

### Run Locally with .NET CLI

```bash
cd src/TourManagement.Web
dotnet restore
dotnet build
dotnet run
```

---

## Docker Build and Push

### Build Docker Image

The project includes automated build scripts for both Linux and Windows.

#### Linux/macOS

```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

#### Windows

```cmd
scripts\build-push.bat
```

### Script Features

- Interactive registry selection (AWS ECR or Docker Hub)
- Automatic ECR repository creation
- Tag sanitization and validation
- Credential management
- Build progress tracking

### Manual Docker Build

```bash
# Build the image
docker build -t tourmanagement:latest .

# Tag for ECR
docker tag tourmanagement:latest 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest

# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789.dkr.ecr.us-east-1.amazonaws.com

# Push to ECR
docker push 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest
```

---

## AWS ECS Fargate Setup

### Step 1: Create IAM Roles

#### ECS Task Execution Role

```bash
# Create the role
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document file://trust-policy.json

# Attach AWS managed policy
aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

**trust-policy.json**:
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

### Step 2: Configure VPC and Security Groups

#### Create Security Group for ECS Tasks

```bash
aws ec2 create-security-group \
  --group-name tourmanagement-ecs-sg \
  --description "Security group for TourManagement ECS tasks" \
  --vpc-id vpc-xxxxx

# Allow inbound HTTP from ALB
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxx \
  --protocol tcp \
  --port 8080 \
  --source-group sg-alb-xxxxx

# Allow outbound to database
aws ec2 authorize-security-group-egress \
  --group-id sg-xxxxx \
  --protocol tcp \
  --port 1433 \
  --cidr 10.0.0.0/16
```

### Step 3: Create CloudWatch Log Group

```bash
aws logs create-log-group \
  --log-group-name /ecs/tourmanagement \
  --region us-east-1

# Set retention policy (optional)
aws logs put-retention-policy \
  --log-group-name /ecs/tourmanagement \
  --retention-in-days 30
```

### Step 4: Set Up RDS SQL Server

```bash
aws rds create-db-instance \
  --db-instance-identifier tourmanagement-db \
  --db-instance-class db.t3.small \
  --engine sqlserver-ex \
  --master-username admin \
  --master-user-password YourSecurePassword \
  --allocated-storage 20 \
  --vpc-security-group-ids sg-database-xxxxx \
  --db-subnet-group-name my-db-subnet-group
```

---

## ECS Task Definition

### Understanding the Task Definition

The task definition (`ecs/task-definition.json`) specifies:

#### Launch Type Configuration
```json
"requiresCompatibilities": ["FARGATE"],
"networkMode": "awsvpc"
```

#### CPU and Memory

**Valid Fargate Combinations**:
- CPU: 256 (.25 vCPU) → Memory: 512, 1024, 2048 MB
- CPU: 512 (.5 vCPU) → Memory: 1024, 2048, 3072, 4096 MB
- CPU: 1024 (1 vCPU) → Memory: 2048-8192 MB
- CPU: 2048 (2 vCPU) → Memory: 4096-16384 MB

**Default Configuration**:
```json
"cpu": "512",
"memory": "1024"
```

#### Container Definition

```json
"containerDefinitions": [
  {
    "name": "tourmanagement",
    "image": "<IMAGE_URI>",
    "essential": true,
    "portMappings": [
      {
        "containerPort": 8080,
        "protocol": "tcp"
      }
    ],
    "environment": [
      {"name": "ASPNETCORE_ENVIRONMENT", "value": "Production"},
      {"name": "ASPNETCORE_URLS", "value": "http://+:8080"}
    ],
    "logConfiguration": {
      "logDriver": "awslogs",
      "options": {
        "awslogs-group": "/ecs/tourmanagement",
        "awslogs-region": "us-east-1",
        "awslogs-stream-prefix": "ecs"
      }
    }
  }
]
```

### Register Task Definition Manually

```bash
aws ecs register-task-definition \
  --cli-input-json file://ecs/task-definition.json \
  --region us-east-1
```

---

## ECS Service Configuration

### Understanding the Service Definition

The service definition (`ecs/service-definition.json`) specifies:

#### Network Configuration
```json
"networkConfiguration": {
  "awsvpcConfiguration": {
    "subnets": ["subnet-xxxxx", "subnet-yyyyy"],
    "securityGroups": ["sg-xxxxx"],
    "assignPublicIp": "ENABLED"
  }
}
```

#### Load Balancer Integration
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

### Create Service Manually

```bash
aws ecs create-service \
  --cli-input-json file://ecs/service-definition.json \
  --region us-east-1
```

---

## Deployment Process

### Automated Deployment

Use the provided deployment scripts for automated deployment.

#### Linux/macOS

```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

#### Windows

```cmd
scripts\deploy-image.bat
```

### Deployment Script Features

1. **Cluster Management**: Automatically creates cluster if it doesn't exist
2. **Load Balancer Setup**: Optional ALB and Target Group creation
3. **Task Definition Registration**: Updates task definition with new image
4. **Service Management**: Creates or updates ECS service
5. **Health Monitoring**: Waits for service stability
6. **Verification**: Confirms deployment success

### Manual Deployment Steps

#### Step 1: Build and Push Image

```bash
./scripts/build-push.sh
# Note the image URI: 123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:v1.0
```

#### Step 2: Update Task Definition

Edit `ecs/task-definition.json` with the new image URI.

#### Step 3: Register Task Definition

```bash
aws ecs register-task-definition \
  --cli-input-json file://ecs/task-definition.json \
  --region us-east-1
```

#### Step 4: Update Service

```bash
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --task-definition tourmanagement-task \
  --force-new-deployment \
  --region us-east-1
```

#### Step 5: Monitor Deployment

```bash
aws ecs wait services-stable \
  --cluster tourmanagement-cluster \
  --services tourmanagement-service \
  --region us-east-1
```

---

## Monitoring and Logging

### CloudWatch Logs

#### View Logs

```bash
# Tail logs in real-time
aws logs tail /ecs/tourmanagement --follow --region us-east-1

# View specific time range
aws logs tail /ecs/tourmanagement \
  --since 1h \
  --format short \
  --region us-east-1

# Filter logs
aws logs filter-log-events \
  --log-group-name /ecs/tourmanagement \
  --filter-pattern "ERROR" \
  --region us-east-1
```

#### Log Insights Queries

```sql
# Error analysis
fields @timestamp, @message
| filter @message like /ERROR/
| sort @timestamp desc
| limit 100

# Request latency
fields @timestamp, duration
| filter @message like /Request completed/
| stats avg(duration), max(duration), min(duration)

# Health check monitoring
fields @timestamp, @message
| filter @message like /health/
| stats count() by bin(5m)
```

### ECS Service Metrics

```bash
# CPU utilization
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-01T23:59:59Z \
  --period 3600 \
  --statistics Average \
  --region us-east-1

# Memory utilization
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

### Application Health Monitoring

```bash
# Check health endpoint
curl http://<ALB-DNS>/health

# Check readiness endpoint
curl http://<ALB-DNS>/ready
```

---

## Troubleshooting

### Common Issues and Solutions

#### 1. Task Fails to Start

**Symptoms**: Tasks transition from PENDING to STOPPED

**Possible Causes**:
- Invalid CPU/memory combination
- Image pull failures
- Container health check failures
- Database connection issues

**Solution**:
```bash
# Describe stopped tasks
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks <task-id> \
  --region us-east-1

# Check stopped reason
aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --desired-status STOPPED \
  --region us-east-1
```

#### 2. Database Migration Errors

**Symptoms**: Application logs show migration failures

**Solution**:
- Verify database connectivity from ECS tasks
- Check security group rules
- Ensure database credentials are correct
- Manually run migrations if needed

```bash
# Execute command in running task
aws ecs execute-command \
  --cluster tourmanagement-cluster \
  --task <task-id> \
  --container tourmanagement \
  --interactive \
  --command "/bin/bash"
```

#### 3. Load Balancer Target Unhealthy

**Symptoms**: Target group shows unhealthy targets

**Solution**:
- Verify health check path (`/health`)
- Check container port mapping (8080)
- Ensure application is listening on 0.0.0.0:8080
- Increase health check grace period

```bash
# Describe target health
aws elbv2 describe-target-health \
  --target-group-arn <target-group-arn> \
  --region us-east-1
```

#### 4. Service Not Stabilizing

**Symptoms**: Deployment stuck, tasks continuously restarting

**Solution**:
- Check CloudWatch logs for application errors
- Verify environment variables
- Review task definition configuration
- Check deployment circuit breaker events

```bash
# Describe service events
aws ecs describe-services \
  --cluster tourmanagement-cluster \
  --services tourmanagement-service \
  --region us-east-1 \
  --query 'services[0].events[0:10]'
```

#### 5. Image Pull Errors

**Symptoms**: "CannotPullContainerError"

**Solution**:
- Verify ECR permissions on task execution role
- Ensure image exists in ECR
- Check image URI is correct

```bash
# Verify image exists
aws ecr describe-images \
  --repository-name tourmanagement \
  --region us-east-1

# Check task execution role permissions
aws iam get-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-name ECRAccessPolicy
```

---

## Scaling and Management

### Manual Scaling

```bash
# Scale service to 5 tasks
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --desired-count 5 \
  --region us-east-1
```

### Auto Scaling

#### Configure Target Tracking Scaling

```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1

# Create scaling policy (CPU-based)
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
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

### Blue/Green Deployments

```bash
# Create new task definition revision
aws ecs register-task-definition \
  --cli-input-json file://ecs/task-definition-v2.json

# Update service with deployment controller
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --task-definition tourmanagement-task:2 \
  --deployment-configuration "deploymentCircuitBreaker={enable=true,rollback=true},maximumPercent=200,minimumHealthyPercent=100" \
  --region us-east-1
```

### Rolling Updates

Fargate supports rolling updates by default:
- `maximumPercent: 200` - Allows double capacity during deployment
- `minimumHealthyPercent: 50` - Maintains at least 50% capacity

---

## Security Considerations

### 1. Secrets Management

**Use AWS Secrets Manager for sensitive data**:

```bash
# Create secret
aws secretsmanager create-secret \
  --name tourmanagement/db-password \
  --secret-string "MySecurePassword" \
  --region us-east-1

# Reference in task definition
"secrets": [
  {
    "name": "DB_PASSWORD",
    "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:tourmanagement/db-password"
  }
]
```

### 2. Network Security

- Use private subnets for ECS tasks when possible
- Restrict security group rules to minimum required
- Enable VPC Flow Logs for network monitoring
- Use AWS PrivateLink for AWS service access

### 3. IAM Best Practices

- Use separate task and execution roles
- Follow principle of least privilege
- Rotate credentials regularly
- Enable CloudTrail for API auditing

### 4. Container Security

- Use minimal base images
- Scan images for vulnerabilities
- Don't run as root user
- Keep .NET runtime updated

### 5. Application Security

- Enable HTTPS in production
- Use strong connection strings
- Implement rate limiting
- Enable CORS policies
- Use security headers middleware

---

## Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/fargate/)
- [.NET 8.0 Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)

---

## Support and Maintenance

For issues and questions:
1. Check CloudWatch logs for application errors
2. Review ECS service events
3. Consult AWS documentation
4. Contact your DevOps team

---

**Last Updated**: January 2026  
**Version**: 1.0  
**Platform**: AWS ECS Fargate  
**Framework**: .NET 8.0