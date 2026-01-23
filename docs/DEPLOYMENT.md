# TMSContainerize - AWS ECS Fargate Deployment Guide

## Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Local Development Setup](#local-development-setup)
4. [Docker Deployment](#docker-deployment)
5. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
6. [ECS Fargate Setup](#ecs-fargate-setup)
7. [ECS Task Definition Explained](#ecs-task-definition-explained)
8. [ECS Service Configuration](#ecs-service-configuration)
9. [Deployment Walkthrough](#deployment-walkthrough)
10. [Monitoring and Logging](#monitoring-and-logging)
11. [Troubleshooting](#troubleshooting)
12. [Scaling and Management](#scaling-and-management)
13. [Security Considerations](#security-considerations)
14. [Configuration Management](#configuration-management)

## Overview

This guide provides comprehensive instructions for deploying the TMSContainerize .NET 8.0 ASP.NET Core application to AWS ECS Fargate. The application is containerized using Docker and deployed to a serverless container platform.

**Application Details:**
- Framework: .NET 8.0 ASP.NET Core
- Application Type: Web API
- Port: 8080
- Health Endpoint: /health
- Target Platform: AWS ECS Fargate

## Prerequisites

### Required Software

1. **Docker Desktop** (version 20.10 or later)
   - Download: https://www.docker.com/products/docker-desktop
   - Verify installation: `docker --version`

2. **AWS CLI** (version 2.x)
   - Download: https://aws.amazon.com/cli/
   - Verify installation: `aws --version`
   - Configure credentials: `aws configure`

3. **.NET SDK 8.0** (for local development)
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version`

4. **Git** (for version control)
   - Download: https://git-scm.com/

### AWS Account Requirements

- Active AWS account with appropriate permissions
- AWS credentials configured locally
- Access to create ECS clusters, task definitions, and services
- Access to create ECR repositories
- Access to create VPC, subnets, and security groups
- Access to create IAM roles and policies

## Local Development Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd TMSContainerize
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Application

```bash
dotnet build -c Release
```

### 4. Run Locally

```bash
dotnet run --project TMSContainerize.csproj
```

The application will start on `http://localhost:8080`.

### 5. Test Health Endpoint

```bash
curl http://localhost:8080/health
```

Expected response: `200 OK` with health status.

## Docker Deployment

### Build Docker Image Locally

```bash
docker build -t tmscontainerize:latest .
```

### Run Container Locally

```bash
docker run -d -p 8080:8080 --name tmscontainerize tmscontainerize:latest
```

### Test Containerized Application

```bash
curl http://localhost:8080/health
```

### Using Docker Compose

```bash
docker-compose up -d
```

This starts the application with all configured volumes and environment variables.

### Stop and Remove Containers

```bash
docker-compose down
```

## AWS ECS Fargate Prerequisites

### 1. IAM Roles

#### ECS Task Execution Role

Create an IAM role named `ecsTaskExecutionRole` with the following managed policy:
- `AmazonECSTaskExecutionRolePolicy`

This role allows ECS to:
- Pull images from ECR
- Write logs to CloudWatch
- Access Secrets Manager (if needed)

**Create via AWS CLI:**

```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document '{"Version":"2012-10-17","Statement":[{"Effect":"Allow","Principal":{"Service":"ecs-tasks.amazonaws.com"},"Action":"sts:AssumeRole"}]}'

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (Optional)

Create an IAM role named `ecsTaskRole` for application-level permissions:
- Access to S3 buckets
- Access to DynamoDB tables
- Access to other AWS services

### 2. VPC and Networking

#### Create VPC (if not exists)

```bash
VPC_ID=$(aws ec2 create-vpc --cidr-block 10.0.0.0/16 --query 'Vpc.VpcId' --output text)
```

#### Create Public Subnets

```bash
SUBNET_1=$(aws ec2 create-subnet --vpc-id $VPC_ID --cidr-block 10.0.1.0/24 --availability-zone us-east-1a --query 'Subnet.SubnetId' --output text)

SUBNET_2=$(aws ec2 create-subnet --vpc-id $VPC_ID --cidr-block 10.0.2.0/24 --availability-zone us-east-1b --query 'Subnet.SubnetId' --output text)
```

#### Create Internet Gateway

```bash
IGW_ID=$(aws ec2 create-internet-gateway --query 'InternetGateway.InternetGatewayId' --output text)

aws ec2 attach-internet-gateway --vpc-id $VPC_ID --internet-gateway-id $IGW_ID
```

#### Create Route Table

```bash
ROUTE_TABLE_ID=$(aws ec2 create-route-table --vpc-id $VPC_ID --query 'RouteTable.RouteTableId' --output text)

aws ec2 create-route --route-table-id $ROUTE_TABLE_ID --destination-cidr-block 0.0.0.0/0 --gateway-id $IGW_ID

aws ec2 associate-route-table --route-table-id $ROUTE_TABLE_ID --subnet-id $SUBNET_1
aws ec2 associate-route-table --route-table-id $ROUTE_TABLE_ID --subnet-id $SUBNET_2
```

#### Create Security Group

```bash
SG_ID=$(aws ec2 create-security-group --group-name tmscontainerize-sg --description "Security group for TMSContainerize" --vpc-id $VPC_ID --query 'GroupId' --output text)

aws ec2 authorize-security-group-ingress --group-id $SG_ID --protocol tcp --port 8080 --cidr 0.0.0.0/0
aws ec2 authorize-security-group-ingress --group-id $SG_ID --protocol tcp --port 80 --cidr 0.0.0.0/0
```

### 3. CloudWatch Log Group

```bash
aws logs create-log-group --log-group-name /ecs/tmscontainerize
```

## ECS Fargate Setup

### 1. Create ECS Cluster

```bash
aws ecs create-cluster --cluster-name tmscontainerize-cluster
```

### 2. Create ECR Repository

```bash
aws ecr create-repository --repository-name tmscontainerize
```

### 3. Build and Push Docker Image

#### Linux/macOS:

```bash
cd scripts
chmod +x build-push.sh
./build-push.sh
```

#### Windows:

```cmd
cd scripts
build-push.bat
```

Follow the prompts to:
1. Select registry type (AWS ECR recommended)
2. Enter AWS region
3. Enter AWS account ID
4. Enter repository name
5. Enter image tag (default: latest)

The script will:
- Authenticate with the selected registry
- Build the Docker image
- Push to the registry
- Display the full image URI

## ECS Task Definition Explained

### Key Components

1. **Launch Type: FARGATE**
   - Serverless container platform
   - No EC2 instance management required

2. **Network Mode: awsvpc**
   - Each task gets its own elastic network interface
   - Required for Fargate launch type

3. **CPU and Memory**
   - CPU: 512 (.5 vCPU)
   - Memory: 1024 MB (1 GB)
   - Valid Fargate combination

4. **Execution Role**
   - ARN: `arn:aws:iam::{{ACCOUNT_ID}}:role/ecsTaskExecutionRole`
   - Allows ECS to pull images and write logs

5. **Task Role** (Optional)
   - ARN: `arn:aws:iam::{{ACCOUNT_ID}}:role/ecsTaskRole`
   - Grants application-level AWS permissions

6. **Container Definition**
   - Name: tmscontainerize
   - Image: {{IMAGE_URI}} (replaced during deployment)
   - Port: 8080
   - Essential: true (task stops if container stops)

7. **Environment Variables**
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ASPNETCORE_URLS=http://+:8080`
   - `DOTNET_RUNNING_IN_CONTAINER=true`
   - `TZ=UTC`

8. **Logging**
   - Driver: awslogs
   - Log Group: /ecs/tmscontainerize
   - Stream Prefix: ecs

### CPU/Memory Valid Combinations

| CPU (vCPU) | Memory (MB) |
|------------|-------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

## ECS Service Configuration

### Key Components

1. **Service Name**: tmscontainerize-service
2. **Desired Count**: 2 (for high availability)
3. **Launch Type**: FARGATE
4. **Platform Version**: LATEST

5. **Network Configuration**
   - Subnets: 2 public subnets in different AZs
   - Security Groups: Allow inbound on port 8080 and 80
   - Public IP: ENABLED (for internet access)

6. **Deployment Configuration**
   - Maximum Percent: 200 (allows double capacity during deployment)
   - Minimum Healthy Percent: 50 (maintains at least half capacity)
   - Circuit Breaker: Enabled with rollback

7. **Load Balancer** (Optional)
   - Target Group ARN: {{TARGET_GROUP_ARN}}
   - Container Name: tmscontainerize
   - Container Port: 8080
   - Health Check Grace Period: 300 seconds

8. **Tags**
   - Application: TMSContainerize
   - Environment: Production
   - ManagedBy: ECS
   - DeploymentType: Fargate

## Deployment Walkthrough

### Step 1: Build and Push Docker Image

#### Linux/macOS:

```bash
cd scripts
chmod +x build-push.sh
./build-push.sh
```

#### Windows:

```cmd
cd scripts
build-push.bat
```

**Note the image URI** displayed at the end (e.g., `123456789.dkr.ecr.us-east-1.amazonaws.com/tmscontainerize:latest`).

### Step 2: Deploy to ECS Fargate

#### Linux/macOS:

```bash
chmod +x deploy-image.sh
./deploy-image.sh
```

#### Windows:

```cmd
deploy-image.bat
```

### Step 3: Deployment Prompts

The script will prompt for:

1. **AWS Region** (e.g., us-east-1)
2. **ECS Cluster Name** (e.g., tmscontainerize-cluster)
3. **VPC ID** (e.g., vpc-0abc123def456)
4. **Subnet IDs** (comma-separated, e.g., subnet-0abc123,subnet-0def456)
5. **Security Group ID** (e.g., sg-0abc123def)
6. **Docker Image URI** (from Step 1)
7. **Load Balancer** (y/n)

### Step 4: Automated Deployment Steps

The script will:

1. Get AWS Account ID
2. Check/create ECS cluster
3. Create CloudWatch log group
4. If load balancer requested:
   - Create Application Load Balancer
   - Create Target Group (with `ip` target type for Fargate)
   - Create ALB Listener
   - Configure health checks
5. Register task definition with replaced placeholders
6. Create or update ECS service
7. Wait for service stability (may take several minutes)
8. Display deployment status and access information

### Step 5: Verify Deployment

```bash
aws ecs describe-services \
  --cluster tmscontainerize-cluster \
  --services tmscontainerize-service \
  --region us-east-1
```

Check:
- `runningCount` should equal `desiredCount` (2)
- `status` should be ACTIVE
- `deployments` should show PRIMARY deployment

### Step 6: Access Application

**If using Load Balancer:**

Access via the Load Balancer DNS name provided by the deployment script:

```
http://<load-balancer-dns>/health
```

**Without Load Balancer:**

Get task public IPs:

```bash
aws ecs list-tasks --cluster tmscontainerize-cluster --service-name tmscontainerize-service

aws ecs describe-tasks --cluster tmscontainerize-cluster --tasks <task-arn>
```

Access directly via task IP:

```
http://<task-public-ip>:8080/health
```

## Monitoring and Logging

### CloudWatch Logs

#### View Logs in Console

1. Navigate to CloudWatch Console
2. Select Log Groups
3. Find `/ecs/tmscontainerize`
4. View log streams for each task

#### Tail Logs via CLI

```bash
aws logs tail /ecs/tmscontainerize --follow --region us-east-1
```

#### Query Logs

```bash
aws logs filter-log-events \
  --log-group-name /ecs/tmscontainerize \
  --filter-pattern "ERROR" \
  --region us-east-1
```

### ECS Service Metrics

#### View in Console

1. Navigate to ECS Console
2. Select Clusters → tmscontainerize-cluster
3. Select Services → tmscontainerize-service
4. View Metrics tab

#### Key Metrics

- CPU Utilization
- Memory Utilization
- Running Task Count
- Desired Task Count
- Pending Task Count

### Application Insights (Optional)

For .NET applications, consider integrating Application Insights:

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

Configure in `Program.cs`:

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptom**: Tasks stop immediately after starting

**Check:**
- CloudWatch logs for error messages
- Task definition CPU/memory values are valid combinations
- Image URI is correct and accessible
- Execution role has permissions to pull from ECR

**Solution:**

```bash
aws ecs describe-tasks --cluster tmscontainerize-cluster --tasks <task-arn>
```

Look for `stoppedReason` and `containers[].reason`.

#### 2. Cannot Pull Image from ECR

**Symptom**: Error "CannotPullContainerError"

**Check:**
- ECR repository exists
- Image with specified tag exists
- Execution role has `AmazonECSTaskExecutionRolePolicy`
- ECR and ECS are in the same region

**Solution:**

```bash
aws ecr describe-images --repository-name tmscontainerize --region us-east-1
```

#### 3. Service Stuck in "DRAINING" State

**Symptom**: Old tasks won't stop

**Check:**
- Application handles SIGTERM signal properly
- No long-running requests blocking shutdown
- Stop timeout is sufficient (default 30 seconds)

**Solution:**

Force new deployment:

```bash
aws ecs update-service \
  --cluster tmscontainerize-cluster \
  --service tmscontainerize-service \
  --force-new-deployment
```

#### 4. Health Check Failures

**Symptom**: Tasks start but fail health checks

**Check:**
- Health endpoint `/health` is accessible
- Application starts within grace period (300 seconds)
- Security group allows inbound traffic on port 8080
- Target group health check settings are appropriate

**Solution:**

Test health endpoint from within VPC:

```bash
curl http://<task-ip>:8080/health
```

#### 5. Invalid CPU/Memory Combination

**Symptom**: "Invalid CPU or memory value specified"

**Solution:**

Use valid Fargate combinations (see table above). Default safe values:
- CPU: "512"
- Memory: "1024"

#### 6. Network Issues

**Symptom**: Tasks can't connect to external services

**Check:**
- Subnets have route to internet gateway
- Security group allows outbound traffic
- NAT gateway configured (if using private subnets)
- DNS resolution works

**Solution:**

Verify route table:

```bash
aws ec2 describe-route-tables --filters "Name=vpc-id,Values=<vpc-id>"
```

## Scaling and Management

### Manual Scaling

#### Update Desired Count

```bash
aws ecs update-service \
  --cluster tmscontainerize-cluster \
  --service tmscontainerize-service \
  --desired-count 4
```

### Auto Scaling

#### Create Scaling Target

```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tmscontainerize-cluster/tmscontainerize-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10
```

#### Create Scaling Policy (CPU-based)

```bash
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tmscontainerize-cluster/tmscontainerize-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 75.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
    },
    "ScaleOutCooldown": 60,
    "ScaleInCooldown": 60
  }'
```

### Blue/Green Deployments

For zero-downtime deployments, configure CodeDeploy:

1. Create CodeDeploy application
2. Create deployment group with ECS blue/green configuration
3. Configure traffic shifting (linear, canary, or all-at-once)
4. Deploy using CodeDeploy

### Rolling Updates

Update task definition and force new deployment:

```bash
aws ecs update-service \
  --cluster tmscontainerize-cluster \
  --service tmscontainerize-service \
  --task-definition tmscontainerize-task:2 \
  --force-new-deployment
```

## Security Considerations

### 1. Use Non-Root User

The Dockerfile creates and uses a non-root user `appuser` for security.

### 2. Secrets Management

Store sensitive data in AWS Secrets Manager or Systems Manager Parameter Store:

```bash
aws secretsmanager create-secret \
  --name tmscontainerize/db-password \
  --secret-string "your-secret-password"
```

Reference in task definition:

```json
{
  "secrets": [
    {
      "name": "DB_PASSWORD",
      "valueFrom": "arn:aws:secretsmanager:region:account-id:secret:tmscontainerize/db-password"
    }
  ]
}
```

### 3. Network Security

- Use private subnets with NAT gateway for production
- Restrict security group rules to minimum required
- Enable VPC Flow Logs for network monitoring
- Use AWS WAF with Application Load Balancer

### 4. Image Scanning

Enable ECR image scanning:

```bash
aws ecr put-image-scanning-configuration \
  --repository-name tmscontainerize \
  --image-scanning-configuration scanOnPush=true
```

### 5. IAM Best Practices

- Use least privilege principle for task role
- Rotate credentials regularly
- Enable MFA for AWS account access
- Use IAM roles instead of access keys

### 6. HTTPS/TLS

Configure ALB with SSL/TLS certificate:

```bash
aws elbv2 create-listener \
  --load-balancer-arn <alb-arn> \
  --protocol HTTPS \
  --port 443 \
  --certificates CertificateArn=<certificate-arn> \
  --default-actions Type=forward,TargetGroupArn=<target-group-arn>
```

## Configuration Management

### Environment-Specific Settings

Use environment variables or AWS Systems Manager Parameter Store:

```bash
aws ssm put-parameter \
  --name /tmscontainerize/production/database-url \
  --value "Server=prod-db;Database=tms;" \
  --type String
```

### Application Settings

Update `appsettings.Production.json` for production-specific configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Feature Flags

Implement feature flags for gradual rollouts:

```bash
dotnet add package Microsoft.FeatureManagement.AspNetCore
```

### Configuration Reload

ASP.NET Core automatically reloads configuration when files change. For ECS, redeploy tasks to pick up new configuration.

## Support and Resources

### AWS Documentation

- [ECS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [ECS Task Definitions](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/task_definitions.html)
- [ECS Service Management](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/ecs_services.html)

### .NET Documentation

- [ASP.NET Core in Containers](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)

### Monitoring Tools

- AWS CloudWatch
- Application Insights
- Datadog
- New Relic

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-23  
**Maintained By**: DevOps Team
