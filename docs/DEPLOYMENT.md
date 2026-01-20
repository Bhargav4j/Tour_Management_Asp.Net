# TourMgmtContainercmp - AWS ECS Fargate Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Docker Deployment](#docker-deployment)
4. [AWS ECS Fargate Deployment](#aws-ecs-fargate-deployment)
5. [Configuration Management](#configuration-management)
6. [Monitoring and Logging](#monitoring-and-logging)
7. [Troubleshooting](#troubleshooting)
8. [Security Considerations](#security-considerations)

---

## Prerequisites

### Required Tools
- **.NET SDK 8.0** or higher
- **Docker Desktop** (for local containerization)
- **AWS CLI v2** (for ECS deployment)
- **Git** (for version control)
- **jq** (for JSON processing in scripts - Linux/macOS only)

### AWS Account Requirements
- Active AWS account with appropriate permissions
- IAM user or role with ECS, ECR, EC2, and CloudWatch permissions
- VPC with at least 2 subnets in different availability zones
- Security group configured for application traffic

### AWS IAM Permissions Required
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ecs:*",
        "ecr:*",
        "ec2:DescribeSubnets",
        "ec2:DescribeSecurityGroups",
        "ec2:DescribeVpcs",
        "elasticloadbalancing:*",
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents",
        "logs:DescribeLogGroups",
        "iam:PassRole",
        "sts:GetCallerIdentity"
      ],
      "Resource": "*"
    }
  ]
}
```

### AWS ECS IAM Roles

#### 1. ECS Task Execution Role
Create an IAM role named `ecsTaskExecutionRole` with the following trust policy:

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

Attach the AWS managed policy: `arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy`

#### 2. ECS Task Role (Optional)
Create an IAM role named `ecsTaskRole` for application-specific permissions (e.g., S3 access, DynamoDB access).

---

## Local Development Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd TourMgmtContainercmp
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Application
```bash
dotnet build -c Release
```

### 4. Run the Application Locally
```bash
dotnet run --project TourMgmtContainercmp.csproj
```

The application will be available at: `http://localhost:8080`

### 5. Test the Application
```bash
curl http://localhost:8080/health
```

---

## Docker Deployment

### Building the Docker Image Locally

```bash
# Build the image
docker build -t tourmgmt-container-app:latest -f Dockerfile .

# Run the container
docker run -d -p 8080:8080 --name tourmgmt-app tourmgmt-container-app:latest

# Check logs
docker logs -f tourmgmt-app

# Test the application
curl http://localhost:8080/health
```

### Using Docker Compose

```bash
# Start the application
docker-compose up -d

# View logs
docker-compose logs -f

# Stop the application
docker-compose down
```

---

## AWS ECS Fargate Deployment

### Step 1: Configure AWS CLI

```bash
aws configure
# Enter your AWS Access Key ID
# Enter your AWS Secret Access Key
# Enter default region (e.g., us-east-1)
# Enter default output format (json)
```

### Step 2: Prepare AWS Infrastructure

#### Create VPC (if not exists)
```bash
aws ec2 create-vpc --cidr-block 10.0.0.0/16 --region us-east-1
```

#### Create Subnets
```bash
# Subnet 1
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.1.0/24 --availability-zone us-east-1a

# Subnet 2
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.2.0/24 --availability-zone us-east-1b
```

#### Create Security Group
```bash
aws ec2 create-security-group --group-name tourmgmt-sg --description "Security group for TourMgmt app" --vpc-id <vpc-id>

# Allow inbound traffic on port 8080
aws ec2 authorize-security-group-ingress --group-id <sg-id> --protocol tcp --port 8080 --cidr 0.0.0.0/0

# Allow inbound traffic on port 80 (for ALB)
aws ec2 authorize-security-group-ingress --group-id <sg-id> --protocol tcp --port 80 --cidr 0.0.0.0/0
```

### Step 3: Build and Push Docker Image

#### Using AWS ECR

**Linux/macOS:**
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows:**
```cmd
scripts\build-push.bat
```

Follow the prompts:
1. Select registry type: `1` (AWS ECR)
2. Enter AWS region: `us-east-1`
3. Enter AWS account ID: `123456789012`
4. Enter ECR repository name: `tourmgmt-container-app`
5. Enter image tag: `latest`

The script will:
- Authenticate with AWS ECR
- Create ECR repository if it doesn't exist
- Build the Docker image
- Push the image to ECR

### Step 4: Deploy to ECS Fargate

**Linux/macOS:**
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

**Windows:**
```cmd
scripts\deploy-image.bat
```

Follow the prompts:
1. Enter AWS region: `us-east-1`
2. Enter ECS cluster name: `tourmgmt-cluster`
3. Enter VPC ID: `vpc-0abc123def456`
4. Enter Subnet IDs: `subnet-0abc123,subnet-0def456`
5. Enter Security Group ID: `sg-0abc123def`
6. Enter Docker image URI: `123456789012.dkr.ecr.us-east-1.amazonaws.com/tourmgmt-container-app:latest`
7. Load balancer needed? `y` or `n`

The script will:
- Create or verify ECS cluster
- Create Application Load Balancer and Target Group (if requested)
- Create CloudWatch log group
- Register ECS task definition
- Create or update ECS service
- Wait for service stability
- Display deployment status

### Step 5: Verify Deployment

#### Check Service Status
```bash
aws ecs describe-services --cluster tourmgmt-cluster --services tourmgmt-container-service --region us-east-1
```

#### List Running Tasks
```bash
aws ecs list-tasks --cluster tourmgmt-cluster --service-name tourmgmt-container-service --region us-east-1
```

#### View Application Logs
```bash
aws logs tail /ecs/tourmgmt-container --follow --region us-east-1
```

#### Test the Application
If using load balancer:
```bash
curl http://<alb-dns-name>/health
```

---

## ECS Task Definition Explained

### Key Configuration Parameters

#### Fargate CPU and Memory Combinations

ECS Fargate requires specific CPU/memory combinations:

| CPU (vCPU) | Memory (MB) Options |
|------------|---------------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

**Default Configuration:** `cpu: "512", memory: "1024"`

#### Container Definition

```json
{
  "name": "tourmgmt-container",
  "image": "<ecr-image-uri>",
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
      "awslogs-group": "/ecs/tourmgmt-container",
      "awslogs-region": "us-east-1",
      "awslogs-stream-prefix": "ecs"
    }
  }
}
```

---

## ECS Service Configuration

### Network Configuration (awsvpc mode)

Fargate requires `awsvpc` network mode, which assigns each task an elastic network interface (ENI):

```json
{
  "networkConfiguration": {
    "awsvpcConfiguration": {
      "subnets": ["subnet-xxx", "subnet-yyy"],
      "securityGroups": ["sg-xxx"],
      "assignPublicIp": "ENABLED"
    }
  }
}
```

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

- **maximumPercent:** Maximum percentage of tasks that can run during deployment (200% = rolling update)
- **minimumHealthyPercent:** Minimum percentage of healthy tasks during deployment
- **deploymentCircuitBreaker:** Automatically rolls back failed deployments

### Load Balancer Integration

When using Application Load Balancer:

```json
{
  "loadBalancers": [
    {
      "targetGroupArn": "<target-group-arn>",
      "containerName": "tourmgmt-container",
      "containerPort": 8080
    }
  ],
  "healthCheckGracePeriodSeconds": 300
}
```

**Important:** Target Group must use `target-type: ip` for Fargate awsvpc mode.

---

## Configuration Management

### Environment Variables

Configure application settings via environment variables in the task definition:

```json
"environment": [
  {"name": "ASPNETCORE_ENVIRONMENT", "value": "Production"},
  {"name": "ASPNETCORE_URLS", "value": "http://+:8080"},
  {"name": "ConnectionStrings__DefaultConnection", "value": "<db-connection-string>"},
  {"name": "Logging__LogLevel__Default", "value": "Information"}
]
```

### Using AWS Secrets Manager

For sensitive data, use Secrets Manager:

```json
"secrets": [
  {
    "name": "DB_PASSWORD",
    "valueFrom": "arn:aws:secretsmanager:region:account-id:secret:db-password"
  }
]
```

---

## Monitoring and Logging

### CloudWatch Logs

View application logs:
```bash
aws logs tail /ecs/tourmgmt-container --follow --region us-east-1
```

### CloudWatch Metrics

Monitor ECS service metrics in CloudWatch:
- CPUUtilization
- MemoryUtilization
- RunningTaskCount
- DesiredTaskCount

### Application Insights (Optional)

For advanced monitoring, integrate Application Insights:

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

---

## Troubleshooting

### Common ECS Fargate Issues

#### 1. Task Fails to Start

**Symptoms:** Tasks transition from PENDING to STOPPED

**Solutions:**
- Check CloudWatch logs for error messages
- Verify IAM roles have correct permissions
- Ensure image URI is correct and accessible
- Verify CPU/memory combination is valid

```bash
aws ecs describe-tasks --cluster <cluster> --tasks <task-id> --region us-east-1
```

#### 2. Network Issues

**Symptoms:** Cannot connect to service or external resources

**Solutions:**
- Verify security group allows inbound/outbound traffic
- Check subnet route tables for internet gateway
- Ensure `assignPublicIp: ENABLED` if using public subnets
- Verify NAT gateway if using private subnets

#### 3. Health Check Failures

**Symptoms:** Load balancer marks targets as unhealthy

**Solutions:**
- Verify health check endpoint responds with 200 OK
- Check health check path in target group settings
- Increase health check grace period in service definition
- Review application logs for startup errors

#### 4. CPU/Memory Errors

**Symptoms:** Tasks stop with "OutOfMemory" or CPU throttling

**Solutions:**
- Increase task memory allocation
- Profile application memory usage
- Optimize .NET runtime settings
- Use CPU/memory reservations

```json
"cpu": "1024",
"memory": "2048"
```

#### 5. Image Pull Errors

**Symptoms:** "CannotPullContainerError"

**Solutions:**
- Verify executionRoleArn has ECR permissions
- Check ECR repository policy
- Ensure image exists in specified region

### .NET-Specific Troubleshooting

#### Application Won't Start

```bash
# Check if DLL exists
docker run --rm <image> ls -la /app

# Test entry point
docker run --rm <image> dotnet TourMgmtContainercmp.dll --help
```

#### Port Binding Issues

```bash
# Verify ASPNETCORE_URLS
docker run --rm -e ASPNETCORE_URLS=http://+:8080 <image>
```

---

## ECS Fargate Scaling and Management

### Service Auto Scaling

Enable auto scaling based on CPU utilization:

```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tourmgmt-cluster/tourmgmt-container-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10

# Create scaling policy
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tourmgmt-cluster/tourmgmt-container-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json
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

For zero-downtime deployments:

1. Update task definition with new image
2. ECS creates new tasks with updated definition
3. Load balancer routes traffic to new tasks
4. Old tasks are drained and terminated

---

## Security Considerations

### 1. Container Security
- Use non-root user in Dockerfile
- Scan images for vulnerabilities
- Use minimal base images
- Keep .NET runtime updated

### 2. Network Security
- Use private subnets with NAT gateway
- Restrict security group rules
- Enable VPC Flow Logs
- Use AWS PrivateLink for AWS services

### 3. IAM Security
- Follow principle of least privilege
- Use separate task and execution roles
- Rotate credentials regularly
- Enable CloudTrail logging

### 4. Data Security
- Encrypt data at rest and in transit
- Use Secrets Manager for sensitive data
- Enable ECS exec only when needed
- Use KMS for encryption keys

---

## Additional Resources

- [AWS ECS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [.NET on Docker Documentation](https://docs.microsoft.com/en-us/dotnet/core/docker/introduction)
- [ASP.NET Core Health Checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [AWS CLI ECS Reference](https://docs.aws.amazon.com/cli/latest/reference/ecs/)

---

## Support

For issues or questions:
1. Check CloudWatch logs first
2. Review AWS ECS service events
3. Consult this deployment guide
4. Contact DevOps team

---

**Document Version:** 1.0  
**Last Updated:** 2026-01-20  
**Target Platform:** AWS ECS Fargate  
**Application:** TourMgmtContainercmp (.NET 8.0)
