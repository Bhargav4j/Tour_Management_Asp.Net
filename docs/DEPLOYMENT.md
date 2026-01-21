# TourManagement Application - AWS ECS Fargate Deployment Guide

This guide provides comprehensive instructions for deploying the TourManagement ASP.NET Core 8.0 application to AWS ECS Fargate.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Docker Deployment](#docker-deployment)
4. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
5. [AWS ECS Fargate Setup](#aws-ecs-fargate-setup)
6. [Building and Pushing Docker Images](#building-and-pushing-docker-images)
7. [ECS Task Definition Explained](#ecs-task-definition-explained)
8. [ECS Service Configuration](#ecs-service-configuration)
9. [Deploying to ECS Fargate](#deploying-to-ecs-fargate)
10. [Monitoring and Logging](#monitoring-and-logging)
11. [Troubleshooting](#troubleshooting)
12. [Scaling and Management](#scaling-and-management)
13. [Security Best Practices](#security-best-practices)

---

## Prerequisites

### Required Software

- **.NET 8.0 SDK**: [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker**: [Install Docker](https://docs.docker.com/get-docker/)
- **AWS CLI v2**: [Install AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
- **Git**: For version control

### AWS Account Requirements

- Active AWS account with appropriate permissions
- IAM user or role with permissions for:
  - ECS (Fargate)
  - ECR (Elastic Container Registry)
  - VPC and networking
  - CloudWatch Logs
  - IAM role creation
  - Application Load Balancer (optional)

### External Services

- **PostgreSQL Database**: Amazon RDS for PostgreSQL or compatible database
- **Redis Cache** (optional): Amazon ElastiCache for Redis or compatible cache

---

## Local Development Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Tourmgmt21ContainerCmp
```

### 2. Configure Application Settings

Update `src/TourManagement.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tourmanagementdb;Username=postgres;Password=yourpassword"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Application

```bash
dotnet build
```

### 5. Run the Application Locally

```bash
cd src/TourManagement.Web
dotnet run
```

The application will be available at `http://localhost:5000` or `https://localhost:5001`.

### 6. Run Tests

```bash
dotnet test
```

---

## Docker Deployment

### Build Docker Image Locally

```bash
docker build -t tourmanagement-app:latest -f Dockerfile .
```

### Run with Docker Compose

```bash
docker-compose up -d
```

The application will be available at `http://localhost:8080`.

### Test Health Endpoints

```bash
curl http://localhost:8080/health
curl http://localhost:8080/ready
```

### Stop Containers

```bash
docker-compose down
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
- Default output format (`json`)

### 2. Create VPC and Networking Resources

#### Option A: Use Default VPC

Most AWS accounts have a default VPC with subnets and security groups.

```bash
# Get default VPC ID
aws ec2 describe-vpcs --filters "Name=isDefault,Values=true" --query "Vpcs[0].VpcId" --output text

# Get default subnets
aws ec2 describe-subnets --filters "Name=vpc-id,Values=<vpc-id>" --query "Subnets[*].SubnetId" --output text
```

#### Option B: Create New VPC

```bash
# Create VPC
aws ec2 create-vpc --cidr-block 10.0.0.0/16

# Create subnets in different availability zones
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.1.0/24 --availability-zone us-east-1a
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.2.0/24 --availability-zone us-east-1b
```

### 3. Create Security Group

```bash
# Create security group
aws ec2 create-security-group \
  --group-name tourmanagement-sg \
  --description "Security group for TourManagement ECS tasks" \
  --vpc-id <vpc-id>

# Allow inbound traffic on port 8080
aws ec2 authorize-security-group-ingress \
  --group-id <security-group-id> \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0

# Allow outbound traffic (default: all allowed)
```

### 4. Create IAM Roles

#### ECS Task Execution Role

This role allows ECS to pull images from ECR and write logs to CloudWatch.

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

# Attach managed policy
aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (Optional)

This role allows your application to access AWS services (e.g., S3, DynamoDB).

```bash
# Create role
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document file://ecs-task-execution-trust-policy.json

# Attach policies as needed (example: S3 read access)
aws iam attach-role-policy \
  --role-name ecsTaskRole \
  --policy-arn arn:aws:iam::aws:policy/AmazonS3ReadOnlyAccess
```

### 5. Create CloudWatch Log Group

```bash
aws logs create-log-group --log-group-name /ecs/tourmanagement-app
```

### 6. Set Up Database (Amazon RDS)

```bash
# Create RDS PostgreSQL instance
aws rds create-db-instance \
  --db-instance-identifier tourmanagement-db \
  --db-instance-class db.t3.micro \
  --engine postgres \
  --master-username postgres \
  --master-user-password <your-password> \
  --allocated-storage 20 \
  --vpc-security-group-ids <security-group-id> \
  --db-subnet-group-name <subnet-group-name>

# Wait for database to be available
aws rds wait db-instance-available --db-instance-identifier tourmanagement-db

# Get database endpoint
aws rds describe-db-instances \
  --db-instance-identifier tourmanagement-db \
  --query "DBInstances[0].Endpoint.Address" \
  --output text
```

### 7. Set Up Redis (Amazon ElastiCache) - Optional

```bash
# Create ElastiCache Redis cluster
aws elasticache create-cache-cluster \
  --cache-cluster-id tourmanagement-redis \
  --cache-node-type cache.t3.micro \
  --engine redis \
  --num-cache-nodes 1 \
  --security-group-ids <security-group-id>

# Get Redis endpoint
aws elasticache describe-cache-clusters \
  --cache-cluster-id tourmanagement-redis \
  --show-cache-node-info \
  --query "CacheClusters[0].CacheNodes[0].Endpoint.Address" \
  --output text
```

---

## AWS ECS Fargate Setup

### Understanding ECS Components

- **Cluster**: Logical grouping of tasks and services
- **Task Definition**: Blueprint for your application (like a Docker Compose file)
- **Task**: Running instance of a task definition
- **Service**: Ensures a specified number of tasks are running

### CloudWatch Logging Configuration

ECS tasks automatically send logs to CloudWatch Logs when configured:

```json
"logConfiguration": {
  "logDriver": "awslogs",
  "options": {
    "awslogs-group": "/ecs/tourmanagement-app",
    "awslogs-region": "us-east-1",
    "awslogs-stream-prefix": "ecs"
  }
}
```

View logs:

```bash
aws logs tail /ecs/tourmanagement-app --follow --region us-east-1
```

---

## Building and Pushing Docker Images

### Using ECR (Recommended for ECS)

#### Step 1: Run the Build Script

**Linux/macOS:**

```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows:**

```cmd
scripts\build-push.bat
```

#### Step 2: Follow Interactive Prompts

1. Select registry type: **1** (AWS ECR)
2. Enter AWS Region: `us-east-1`
3. Enter AWS Account ID: `123456789012`
4. Enter ECR repository name: `tourmanagement`
5. Enter image tag: `v1.0.0` or `latest`

The script will:
- Authenticate with ECR
- Create ECR repository if it doesn't exist
- Build the Docker image
- Push the image to ECR

#### Manual ECR Push

```bash
# Get ECR login credentials
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

# Create ECR repository
aws ecr create-repository --repository-name tourmanagement --region us-east-1

# Build image
docker build -t tourmanagement:latest .

# Tag image
docker tag tourmanagement:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest

# Push image
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest
```

---

## ECS Task Definition Explained

### Fargate CPU and Memory Combinations

ECS Fargate requires valid CPU/memory combinations:

| CPU (vCPU) | Memory (MB) Options |
|------------|---------------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

**Default Configuration:**
- CPU: `512` (0.5 vCPU)
- Memory: `1024` MB (1 GB)

### Key Task Definition Fields

```json
{
  "family": "tourmanagement-task",
  "requiresCompatibilities": ["FARGATE"],
  "networkMode": "awsvpc",
  "cpu": "512",
  "memory": "1024",
  "executionRoleArn": "arn:aws:iam::<account-id>:role/ecsTaskExecutionRole",
  "containerDefinitions": [
    {
      "name": "tourmanagement-app",
      "image": "<account-id>.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest",
      "essential": true,
      "portMappings": [{"containerPort": 8080, "protocol": "tcp"}],
      "environment": [...],
      "logConfiguration": {...}
    }
  ]
}
```

---

## ECS Service Configuration

### Service Definition Fields

- **desiredCount**: Number of tasks to run (default: 2 for high availability)
- **launchType**: `FARGATE` for serverless container deployment
- **networkConfiguration**: VPC, subnets, security groups
- **deploymentConfiguration**: Rolling update settings
- **loadBalancers**: Application Load Balancer integration (optional)

### Deployment Configuration

```json
"deploymentConfiguration": {
  "maximumPercent": 200,
  "minimumHealthyPercent": 50
}
```

- **maximumPercent**: Maximum tasks during deployment (200% = 2x desired count)
- **minimumHealthyPercent**: Minimum healthy tasks during deployment (50% = half of desired count)

This allows rolling deployments without downtime.

---

## Deploying to ECS Fargate

### Using the Deployment Script

**Linux/macOS:**

```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

**Windows:**

```cmd
scripts\deploy-image.bat
```

### Interactive Prompts

1. **AWS Region**: `us-east-1`
2. **ECS Cluster Name**: `tourmanagement-cluster`
3. **VPC ID**: `vpc-0abc123def456`
4. **Subnet IDs**: `subnet-0abc123,subnet-0def456`
5. **Security Group ID**: `sg-0abc123def`
6. **Docker Image URI**: `123456789.dkr.ecr.us-east-1.amazonaws.com/tourmanagement:latest`
7. **Database Configuration**:
   - Host: `tourmanagement-db.abc123.us-east-1.rds.amazonaws.com`
   - Port: `5432`
   - Name: `tourmanagementdb`
   - Username: `postgres`
   - Password: `<your-password>`
8. **Redis Connection String**: `tourmanagement-redis.abc123.cache.amazonaws.com:6379`
9. **Load Balancer**: `y` (yes) or `n` (no)

### Manual Deployment Steps

#### 1. Register Task Definition

```bash
aws ecs register-task-definition --cli-input-json file://ecs/task-definition.json --region us-east-1
```

#### 2. Create ECS Service

```bash
aws ecs create-service --cli-input-json file://ecs/service-definition.json --region us-east-1
```

#### 3. Wait for Service Stability

```bash
aws ecs wait services-stable --cluster tourmanagement-cluster --services tourmanagement-service --region us-east-1
```

#### 4. Update Existing Service

```bash
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --task-definition tourmanagement-task \
  --force-new-deployment \
  --region us-east-1
```

---

## Monitoring and Logging

### CloudWatch Logs

**View Live Logs:**

```bash
aws logs tail /ecs/tourmanagement-app --follow --region us-east-1
```

**Filter Logs:**

```bash
aws logs filter-log-events \
  --log-group-name /ecs/tourmanagement-app \
  --filter-pattern "ERROR" \
  --region us-east-1
```

### ECS Service Monitoring

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

**Describe Task:**

```bash
aws ecs describe-tasks \
  --cluster tourmanagement-cluster \
  --tasks <task-arn> \
  --region us-east-1
```

### CloudWatch Metrics

ECS automatically publishes metrics:
- CPUUtilization
- MemoryUtilization
- NetworkRxBytes
- NetworkTxBytes

**View Metrics:**

```bash
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=tourmanagement-service Name=ClusterName,Value=tourmanagement-cluster \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-02T00:00:00Z \
  --period 3600 \
  --statistics Average \
  --region us-east-1
```

---

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptoms:** Tasks continuously stop and restart.

**Solutions:**
- Check CloudWatch logs for application errors
- Verify environment variables are correct
- Ensure security group allows outbound traffic
- Check database connectivity

```bash
# Get stopped task details
aws ecs describe-tasks --cluster tourmanagement-cluster --tasks <task-id> --region us-east-1
```

#### 2. Cannot Pull Image from ECR

**Symptoms:** "CannotPullContainerError"

**Solutions:**
- Verify ECR image URI is correct
- Ensure `ecsTaskExecutionRole` has `AmazonECSTaskExecutionRolePolicy` attached
- Check ECR repository permissions

```bash
# Test ECR authentication
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com
```

#### 3. Health Check Failures

**Symptoms:** Tasks are marked unhealthy and replaced.

**Solutions:**
- Test health endpoint locally: `curl http://localhost:8080/health`
- Increase `healthCheckGracePeriodSeconds` in service definition
- Verify security group allows traffic on port 8080
- Check application startup time

#### 4. Invalid CPU/Memory Combination

**Symptoms:** "InvalidParameterException: No Fargate configuration exists"

**Solutions:**
- Use valid Fargate CPU/memory combinations (see table above)
- Update task definition with correct values

#### 5. Database Connection Issues

**Symptoms:** Application logs show database connection errors.

**Solutions:**
- Verify database endpoint is correct
- Check security group allows traffic from ECS tasks to RDS
- Test connection string format
- Ensure database credentials are correct

### Debugging Commands

```bash
# Get task logs
aws logs tail /ecs/tourmanagement-app --follow --region us-east-1

# Describe service events
aws ecs describe-services --cluster tourmanagement-cluster --services tourmanagement-service --region us-east-1 --query 'services[0].events'

# Get task stopped reason
aws ecs describe-tasks --cluster tourmanagement-cluster --tasks <task-id> --region us-east-1 --query 'tasks[0].stoppedReason'

# Check security group rules
aws ec2 describe-security-groups --group-ids <security-group-id> --region us-east-1
```

---

## Scaling and Management

### Manual Scaling

```bash
# Scale to 5 tasks
aws ecs update-service \
  --cluster tourmanagement-cluster \
  --service tourmanagement-service \
  --desired-count 5 \
  --region us-east-1
```

### Auto Scaling

#### Create Auto Scaling Target

```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1
```

#### Create Scaling Policy (Target Tracking)

```bash
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tourmanagement-cluster/tourmanagement-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-target-tracking-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json \
  --region us-east-1
```

**scaling-policy.json:**

```json
{
  "TargetValue": 75.0,
  "PredefinedMetricSpecification": {
    "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
  },
  "ScaleInCooldown": 300,
  "ScaleOutCooldown": 60
}
```

### Blue/Green Deployments

For zero-downtime deployments, use AWS CodeDeploy with ECS:

1. Create CodeDeploy application and deployment group
2. Configure blue/green deployment settings
3. Deploy new task definition revision
4. CodeDeploy shifts traffic from blue to green

---

## Security Best Practices

### 1. Use Secrets Management

Store sensitive data in AWS Secrets Manager or SSM Parameter Store:

```json
"secrets": [
  {
    "name": "DB_PASSWORD",
    "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:tourmanagement/db-password"
  }
]
```

### 2. Enable VPC Endpoints

Use VPC endpoints for private connectivity to AWS services (ECR, CloudWatch, Secrets Manager).

### 3. Use Non-Root User

The Dockerfile creates a non-root user for enhanced security:

```dockerfile
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser
```

### 4. Implement Network Segmentation

- Use private subnets for ECS tasks
- Route internet traffic through NAT Gateway
- Restrict security group rules to minimum required

### 5. Enable Container Insights

```bash
aws ecs update-cluster-settings \
  --cluster tourmanagement-cluster \
  --settings name=containerInsights,value=enabled \
  --region us-east-1
```

### 6. Regular Image Scanning

Enable ECR image scanning:

```bash
aws ecr put-image-scanning-configuration \
  --repository-name tourmanagement \
  --image-scanning-configuration scanOnPush=true \
  --region us-east-1
```

---

## Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [ASP.NET Core Deployment Guide](https://docs.microsoft.com/aspnet/core/host-and-deploy/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)

---

## Support and Feedback

For issues or questions:
- Check CloudWatch logs for application errors
- Review ECS service events
- Consult AWS documentation
- Contact your DevOps team

---

**Document Version:** 1.0  
**Last Updated:** January 2026  
**Application:** TourManagement ASP.NET Core 8.0  
**Target Platform:** AWS ECS Fargate