# TourManagement - AWS ECS Fargate Deployment Guide

## Overview

This guide provides comprehensive instructions for deploying the TourManagement ASP.NET Core 8.0 application to AWS ECS Fargate.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Docker Deployment](#docker-deployment)
4. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
5. [ECS Fargate Setup](#ecs-fargate-setup)
6. [Building and Pushing Docker Image](#building-and-pushing-docker-image)
7. [ECS Task Definition Explained](#ecs-task-definition-explained)
8. [ECS Service Configuration](#ecs-service-configuration)
9. [Deployment Walkthrough](#deployment-walkthrough)
10. [Troubleshooting](#troubleshooting)
11. [Scaling and Management](#scaling-and-management)
12. [Security Considerations](#security-considerations)

## Prerequisites

### Required Tools

- **.NET 8.0 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop**: [Download](https://www.docker.com/products/docker-desktop)
- **AWS CLI v2**: [Installation Guide](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
- **Git**: For version control

### AWS Account Requirements

- Active AWS account with appropriate permissions
- IAM user with permissions for:
  - ECS (Elastic Container Service)
  - ECR (Elastic Container Registry)
  - EC2 (for VPC, subnets, security groups)
  - IAM (for role management)
  - CloudWatch Logs
  - Application Load Balancer (optional)

### AWS CLI Configuration

Configure AWS CLI with your credentials:

```bash
aws configure
```

Provide:
- AWS Access Key ID
- AWS Secret Access Key
- Default region (e.g., us-east-1)
- Default output format (json)

Verify configuration:

```bash
aws sts get-caller-identity
```

## Local Development Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd TourManagement
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Application

```bash
dotnet build
```

### 4. Configure Database Connection

Update `src/TourManagement.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TourManagementDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
  }
}
```

### 5. Run Database Migrations

```bash
cd src/TourManagement.Web
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run --project src/TourManagement.Web
```

Access the application at `https://localhost:5001` or `http://localhost:5000`.

## Docker Deployment

### Build Docker Image Locally

```bash
docker build -t tourmanagement:latest .
```

### Run with Docker Compose

```bash
docker-compose up -d
```

Access the application at `http://localhost:8080`.

### Test Health Endpoint

```bash
curl http://localhost:8080/health
```

Expected response: `Healthy`

## AWS ECS Fargate Prerequisites

### 1. VPC Configuration

Ensure you have a VPC with:
- At least 2 public subnets in different availability zones
- Internet Gateway attached
- Route table configured for internet access

Create VPC (if needed):

```bash
aws ec2 create-vpc --cidr-block 10.0.0.0/16 --region us-east-1
```

### 2. Security Groups

Create a security group for ECS tasks:

```bash
aws ec2 create-security-group \
  --group-name tourmanagement-sg \
  --description "Security group for TourManagement ECS tasks" \
  --vpc-id vpc-xxxxx \
  --region us-east-1
```

Add inbound rules:

```bash
# Allow HTTP traffic
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxx \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0 \
  --region us-east-1

# Allow health checks
aws ec2 authorize-security-group-ingress \
  --group-id sg-xxxxx \
  --protocol tcp \
  --port 8080 \
  --source-group sg-xxxxx \
  --region us-east-1
```

### 3. IAM Roles

#### ECS Task Execution Role

Create role for ECS to pull images and write logs:

```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'
```

Attach managed policy:

```bash
aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (Optional)

Create role for application to access AWS services:

```bash
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'
```

Attach policies as needed (S3, DynamoDB, etc.).

### 4. RDS Database Setup (Recommended)

Create RDS SQL Server instance:

```bash
aws rds create-db-instance \
  --db-instance-identifier tourmanagement-db \
  --db-instance-class db.t3.micro \
  --engine sqlserver-ex \
  --master-username admin \
  --master-user-password YourPassword123! \
  --allocated-storage 20 \
  --vpc-security-group-ids sg-xxxxx \
  --region us-east-1
```

Wait for instance to be available:

```bash
aws rds wait db-instance-available --db-instance-identifier tourmanagement-db
```

Get endpoint:

```bash
aws rds describe-db-instances \
  --db-instance-identifier tourmanagement-db \
  --query 'DBInstances[0].Endpoint.Address' \
  --output text
```

## ECS Fargate Setup

### 1. Create ECS Cluster

```bash
aws ecs create-cluster --cluster-name tourmanagement-cluster --region us-east-1
```

### 2. Create CloudWatch Log Group

```bash
aws logs create-log-group --log-group-name /ecs/tourmanagement --region us-east-1
```

### 3. Create ECR Repository

```bash
aws ecr create-repository --repository-name tourmanagement --region us-east-1
```

## Building and Pushing Docker Image

### Linux/macOS

Run the build and push script:

```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

Follow the prompts:
1. Select registry (AWS ECR or Docker Hub)
2. Provide registry details
3. Enter image tag (default: latest)

### Windows

Run the build and push script:

```cmd
scripts\build-push.bat
```

Follow the same prompts as Linux/macOS.

### Manual Build and Push

If you prefer manual commands:

```bash
# Authenticate with ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

# Build image
docker build -t tourmanagement:latest .

# Tag image
docker tag tourmanagement:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest

# Push image
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest
```

## ECS Task Definition Explained

The task definition (`ecs/task-definition.json`) specifies:

### Fargate Configuration

```json
{
  "requiresCompatibilities": ["FARGATE"],
  "networkMode": "awsvpc",
  "cpu": "512",
  "memory": "1024"
}
```

**Valid CPU/Memory Combinations for Fargate:**

| CPU (vCPU) | Memory (MB) |
|------------|-------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

### Container Configuration

- **Image**: ECR image URI
- **Port**: 8080 (ASP.NET Core application port)
- **Environment Variables**: Database connection, ASP.NET Core settings
- **Health Check**: Checks `/health` endpoint every 30 seconds
- **Logging**: CloudWatch Logs integration

### IAM Roles

- **executionRoleArn**: Allows ECS to pull images and write logs
- **taskRoleArn**: Allows application to access AWS services

## ECS Service Configuration

The service definition (`ecs/service-definition.json`) specifies:

### Service Settings

```json
{
  "desiredCount": 2,
  "launchType": "FARGATE"
}
```

- **desiredCount**: Number of task instances to run (2 for high availability)
- **launchType**: FARGATE for serverless deployment

### Network Configuration

```json
{
  "networkConfiguration": {
    "awsvpcConfiguration": {
      "subnets": ["subnet-xxxxx", "subnet-yyyyy"],
      "securityGroups": ["sg-xxxxx"],
      "assignPublicIp": "ENABLED"
    }
  }
}
```

- **subnets**: At least 2 subnets in different AZs
- **securityGroups**: Security group allowing traffic on port 8080
- **assignPublicIp**: ENABLED for public access (use ALB for production)

### Load Balancer Integration

```json
{
  "loadBalancers": [
    {
      "targetGroupArn": "arn:aws:elasticloadbalancing:...",
      "containerName": "tourmanagement-web",
      "containerPort": 8080
    }
  ],
  "healthCheckGracePeriodSeconds": 300
}
```

- **targetGroupArn**: ALB target group ARN
- **healthCheckGracePeriodSeconds**: Time to wait before health checks start

### Deployment Configuration

```json
{
  "deploymentConfiguration": {
    "maximumPercent": 200,
    "minimumHealthyPercent": 50,
    "deploymentCircuitBreaker": {
      "enable": true,
      "rollback": true
    }
  }
}
```

- **maximumPercent**: Maximum tasks during deployment (200% = 2x desired count)
- **minimumHealthyPercent**: Minimum healthy tasks during deployment (50%)
- **deploymentCircuitBreaker**: Automatically rollback failed deployments

## Deployment Walkthrough

### Step 1: Prepare Infrastructure

Ensure all prerequisites are met:
- VPC with subnets
- Security groups
- IAM roles
- RDS database (if using)

### Step 2: Build and Push Docker Image

```bash
# Linux/macOS
./scripts/build-push.sh

# Windows
scripts\build-push.bat
```

Note the image URI (e.g., `123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest`).

### Step 3: Deploy to ECS Fargate

```bash
# Linux/macOS
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh

# Windows
scripts\deploy-image.bat
```

Provide the following when prompted:

1. **AWS Region**: us-east-1
2. **ECS Cluster Name**: tourmanagement-cluster
3. **VPC ID**: vpc-xxxxx
4. **Subnet IDs**: subnet-xxxxx,subnet-yyyyy
5. **Security Group ID**: sg-xxxxx
6. **Database Server**: your-db.abc123.us-east-1.rds.amazonaws.com
7. **Database Name**: TourManagementDb
8. **Database User**: admin
9. **Database Password**: (enter securely)
10. **Docker Image URI**: (from Step 2)
11. **Load Balancer**: y/n

### Step 4: Verify Deployment

Check service status:

```bash
aws ecs describe-services \
  --cluster tourmanagement-cluster \
  --services tourmanagement-service \
  --region us-east-1
```

Check running tasks:

```bash
aws ecs list-tasks \
  --cluster tourmanagement-cluster \
  --service-name tourmanagement-service \
  --region us-east-1
```

### Step 5: Access the Application

If using load balancer, get DNS name:

```bash
aws elbv2 describe-load-balancers \
  --names tourmanagement-alb \
  --query 'LoadBalancers[0].DNSName' \
  --output text
```

Access application:
```
http://<alb-dns-name>/
```

Test health endpoint:
```
http://<alb-dns-name>/health
```

### Step 6: Monitor Logs

View CloudWatch logs:

```bash
aws logs tail /ecs/tourmanagement --follow --region us-east-1
```

## Troubleshooting

### Task Fails to Start

**Symptoms**: Tasks repeatedly stop or fail to start.

**Solutions**:

1. Check CloudWatch logs:
   ```bash
   aws logs tail /ecs/tourmanagement --region us-east-1
   ```

2. Verify IAM roles:
   - Ensure `ecsTaskExecutionRole` exists
   - Check role has `AmazonECSTaskExecutionRolePolicy`

3. Check task definition:
   - Verify CPU/memory combination is valid
   - Ensure image URI is correct
   - Confirm environment variables are set

### Health Checks Failing

**Symptoms**: Tasks marked unhealthy and replaced.

**Solutions**:

1. Test health endpoint locally:
   ```bash
   docker run -p 8080:8080 tourmanagement:latest
   curl http://localhost:8080/health
   ```

2. Increase health check grace period:
   - Edit `healthCheckGracePeriodSeconds` in service definition
   - Recommended: 300 seconds for .NET applications

3. Check database connectivity:
   - Verify RDS security group allows ECS tasks
   - Test connection string

### Cannot Pull Image from ECR

**Symptoms**: Error: "CannotPullContainerError"

**Solutions**:

1. Verify ECR permissions:
   ```bash
   aws ecr describe-repositories --region us-east-1
   ```

2. Check `ecsTaskExecutionRole` has ECR permissions:
   ```bash
   aws iam list-attached-role-policies --role-name ecsTaskExecutionRole
   ```

3. Ensure image exists:
   ```bash
   aws ecr describe-images --repository-name tourmanagement --region us-east-1
   ```

### Network Connectivity Issues

**Symptoms**: Tasks cannot connect to database or external services.

**Solutions**:

1. Verify security groups:
   - ECS task security group allows outbound traffic
   - RDS security group allows inbound from ECS security group

2. Check subnet routing:
   - Subnets have route to internet gateway (for public access)
   - Subnets have route to NAT gateway (for private access)

3. Verify DNS resolution:
   - Check VPC DNS settings
   - Ensure `enableDnsHostnames` and `enableDnsSupport` are enabled

### High CPU/Memory Usage

**Symptoms**: Tasks using high CPU or memory, potential OOM kills.

**Solutions**:

1. Review application performance:
   - Check for memory leaks
   - Optimize database queries
   - Review logging verbosity

2. Increase task resources:
   - Update CPU/memory in task definition
   - Valid combinations: cpu: "1024", memory: "2048"

3. Enable detailed monitoring:
   - Use CloudWatch Container Insights
   - Review metrics for bottlenecks

## Scaling and Management

### Manual Scaling

Update desired count:

```bash
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --desired-count 3 \
  --region us-east-1
```

### Auto Scaling

Create auto-scaling target:

```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1
```

Create scaling policy (CPU-based):

```bash
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
    },
    "ScaleInCooldown": 300,
    "ScaleOutCooldown": 60
  }' \
  --region us-east-1
```

### Blue/Green Deployments

ECS supports blue/green deployments with CodeDeploy:

1. Create CodeDeploy application:
   ```bash
   aws deploy create-application \
     --application-name tourmanagement \
     --compute-platform ECS
   ```

2. Create deployment group:
   ```bash
   aws deploy create-deployment-group \
     --application-name tourmanagement \
     --deployment-group-name production \
     --deployment-config-name CodeDeployDefault.ECSAllAtOnce \
     --service-role-arn arn:aws:iam::ACCOUNT_ID:role/CodeDeployRole \
     --ecs-services clusterName=tourmanagement-cluster,serviceName=tourmanagement-service \
     --load-balancer-info targetGroupPairInfoList=[
       {targetGroups=[{name=tourmanagement-tg-blue},{name=tourmanagement-tg-green}],
        prodTrafficRoute={listenerArns=[arn:aws:elasticloadbalancing:...]}}
     ]
   ```

### Rolling Updates

By default, ECS performs rolling updates:

1. Register new task definition revision
2. Update service with new task definition:
   ```bash
   aws ecs update-service \
     --cluster tourmanagement-cluster \
     --service tourmanagement-service \
     --task-definition tourmanagement-task:2 \
     --region us-east-1
   ```

3. ECS replaces tasks gradually based on deployment configuration

## Security Considerations

### 1. Use Secrets Manager for Sensitive Data

Store database credentials in AWS Secrets Manager:

```bash
aws secretsmanager create-secret \
  --name tourmanagement/db-password \
  --secret-string '{"password":"YourPassword123!"}' \
  --region us-east-1
```

Reference in task definition:

```json
{
  "secrets": [
    {
      "name": "DB_PASSWORD",
      "valueFrom": "arn:aws:secretsmanager:us-east-1:ACCOUNT_ID:secret:tourmanagement/db-password"
    }
  ]
}
```

### 2. Use Private Subnets with NAT Gateway

For production, deploy ECS tasks in private subnets:

- Remove `assignPublicIp: ENABLED`
- Ensure NAT gateway exists for outbound internet access
- Use VPC endpoints for AWS services (ECR, Secrets Manager, CloudWatch)

### 3. Enable ECS Exec for Debugging

Add to task definition:

```json
{
  "enableExecuteCommand": true
}
```

Connect to running task:

```bash
aws ecs execute-command \
  --cluster tourmanagement-cluster \
  --task task-id \
  --container tourmanagement-web \
  --interactive \
  --command "/bin/bash"
```

### 4. Implement WAF (Web Application Firewall)

Protect ALB with AWS WAF:

```bash
aws wafv2 create-web-acl \
  --name tourmanagement-waf \
  --scope REGIONAL \
  --default-action Block={} \
  --rules file://waf-rules.json \
  --region us-east-1
```

### 5. Enable Container Insights

Monitor performance and security:

```bash
aws ecs update-cluster-settings \
  --cluster tourmanagement-cluster \
  --settings name=containerInsights,value=enabled \
  --region us-east-1
```

## Performance Optimization

### .NET-Specific Optimizations

1. **ReadyToRun Images**: Use AOT compilation for faster startup
2. **Tiered Compilation**: Enable for better performance
3. **Memory Management**: Configure GC settings for containerized environments

Add to Dockerfile:

```dockerfile
ENV DOTNET_TieredCompilation=1 \
    DOTNET_GCHeapHardLimit=0x40000000
```

### Application Insights Integration

Add to `appsettings.json`:

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key"
  }
}
```

### Distributed Caching with Redis

For multi-instance deployments, use Redis for session state:

1. Create ElastiCache Redis cluster
2. Update connection string in task definition:
   ```json
   {
     "name": "ConnectionStrings__Redis",
     "value": "redis-endpoint:6379"
   }
   ```

## Cost Optimization

### 1. Use Fargate Spot

Reduce costs by 70% with Fargate Spot:

```json
{
  "capacityProviderStrategy": [
    {
      "capacityProvider": "FARGATE_SPOT",
      "weight": 1
    },
    {
      "capacityProvider": "FARGATE",
      "weight": 1,
      "base": 1
    }
  ]
}
```

### 2. Right-Size Tasks

Monitor CPU/memory usage and adjust:
- Start with cpu: "256", memory: "512" for small workloads
- Scale up only when needed

### 3. Use Compute Savings Plans

Commit to 1-year or 3-year usage for discounts.

## Monitoring and Observability

### CloudWatch Dashboards

Create dashboard:

```bash
aws cloudwatch put-dashboard \
  --dashboard-name TourManagement \
  --dashboard-body file://dashboard.json
```

### CloudWatch Alarms

Create alarm for high CPU:

```bash
aws cloudwatch put-metric-alarm \
  --alarm-name tourmanagement-high-cpu \
  --alarm-description "Alert when CPU exceeds 80%" \
  --metric-name CPUUtilization \
  --namespace AWS/ECS \
  --statistic Average \
  --period 300 \
  --threshold 80 \
  --comparison-operator GreaterThanThreshold \
  --evaluation-periods 2 \
  --dimensions Name=ClusterName,Value=tourmanagement-cluster Name=ServiceName,Value=tourmanagement-service
```

### X-Ray Tracing

Enable distributed tracing:

1. Add X-Ray daemon as sidecar in task definition
2. Install AWS X-Ray SDK in application
3. View traces in AWS X-Ray console

## Maintenance

### Update Application

1. Build and push new image with updated tag
2. Update task definition with new image
3. Update service to trigger deployment

### Database Migrations

Run migrations before deployment:

1. Create one-off task for migrations
2. Use ECS Run Task with migration command
3. Wait for completion before updating service

### Backup and Disaster Recovery

1. Enable automated RDS backups
2. Configure Multi-AZ deployment for RDS
3. Use ECS service deployment circuit breaker
4. Maintain infrastructure as code (CloudFormation/Terraform)

## Support and Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/fargate/)
- [.NET on AWS](https://aws.amazon.com/developer/language/net/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)

## Conclusion

This guide provides a comprehensive walkthrough for deploying the TourManagement ASP.NET Core application to AWS ECS Fargate. Follow the steps carefully, and refer to the troubleshooting section for common issues.

For questions or issues, consult the AWS documentation or open a support ticket.