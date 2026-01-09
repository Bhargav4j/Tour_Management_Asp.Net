# Deployment Guide: tour-management-cont

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
- **.NET 8.0 SDK** or later
- **Docker** (version 20.10 or later)
- **AWS CLI** (version 2.x)
- **Git** (for version control)
- **PowerShell** (Windows) or **Bash** (Linux/macOS)

### AWS Requirements
- **AWS Account** with appropriate permissions
- **IAM User** with the following policies:
  - `AmazonECS_FullAccess`
  - `AmazonEC2ContainerRegistryFullAccess`
  - `IAMReadOnlyAccess`
  - `AmazonVPCFullAccess`
  - `ElasticLoadBalancingFullAccess`
- **AWS CLI** configured with credentials:
  ```bash
  aws configure
  ```

### VPC and Network Setup
- **VPC** with at least 2 subnets in different Availability Zones
- **Security Group** allowing:
  - Inbound: Port 8080 (application), Port 80 (ALB)
  - Outbound: All traffic (for ECR pulls and external APIs)
- **Internet Gateway** attached to VPC
- **Route Tables** configured for public subnet access

### IAM Roles Required

#### 1. ECS Task Execution Role
Create role `ecsTaskExecutionRole` with policy:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ecr:GetAuthorizationToken",
        "ecr:BatchCheckLayerAvailability",
        "ecr:GetDownloadUrlForLayer",
        "ecr:BatchGetImage",
        "logs:CreateLogStream",
        "logs:PutLogEvents",
        "logs:CreateLogGroup"
      ],
      "Resource": "*"
    }
  ]
}
```

#### 2. ECS Task Role (Optional)
Create role `ecsTaskRole` for application-specific permissions (e.g., S3, DynamoDB access).

---

## Local Development Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd tour-management-cont
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
dotnet run --project tour-management-cont.csproj
```

Access the application at: `http://localhost:8080`

### 5. Run with Docker Compose
```bash
docker-compose up --build
```

---

## Docker Deployment

### Build Docker Image
```bash
docker build -t tour-management-cont:latest .
```

### Run Docker Container
```bash
docker run -d \
  --name tour-management-cont \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  tour-management-cont:latest
```

### Push to Docker Registry

#### Using Build Script (Recommended)
**Linux/macOS:**
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows:**
```cmd
scripts\build-push.bat
```

The script will prompt for:
- Registry type (AWS ECR or Docker Hub)
- Registry credentials
- Image tag

---

## AWS ECS Fargate Deployment

### Overview
AWS ECS Fargate is a serverless container orchestration service that eliminates the need to manage EC2 instances. It automatically provisions, scales, and manages the infrastructure.

### Architecture Components

1. **ECS Cluster**: Logical grouping of tasks and services
2. **Task Definition**: Blueprint for your application (CPU, memory, container image)
3. **Service**: Maintains desired number of task instances
4. **Application Load Balancer (Optional)**: Distributes traffic across tasks
5. **CloudWatch Logs**: Centralized logging for containers

### Step-by-Step Deployment

#### Step 1: Build and Push Docker Image

Run the build script to create and push your Docker image:

**Linux/macOS:**
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

**Windows:**
```cmd
scripts\build-push.bat
```

**Example interaction:**
```
Select Docker Registry:
1. AWS ECR (Elastic Container Registry)
2. Docker Hub
Enter choice (1 or 2): 1

Enter AWS Region (e.g., us-east-1): us-east-1
Enter AWS Account ID: 123456789012
Enter ECR Repository Name (default: tour-management-cont): tour-management-cont
Enter image tag (default: latest): v1.0.0
```

**Note the image URI** for deployment:
```
123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management-cont:v1.0.0
```

#### Step 2: Prepare Network Infrastructure

Ensure you have:
- VPC ID (e.g., `vpc-0abc123def456`)
- At least 2 Subnet IDs in different AZs (e.g., `subnet-0abc123,subnet-0def456`)
- Security Group ID (e.g., `sg-0abc123def`)

**Create Security Group (if needed):**
```bash
aws ec2 create-security-group \
  --group-name tour-management-cont-sg \
  --description "Security group for tour-management-cont" \
  --vpc-id vpc-0abc123def456 \
  --region us-east-1

# Allow inbound traffic on port 8080
aws ec2 authorize-security-group-ingress \
  --group-id sg-0abc123def \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0 \
  --region us-east-1
```

#### Step 3: Deploy to ECS Fargate

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

**Example interaction:**
```
Enter AWS Region (e.g., us-east-1): us-east-1
Enter ECS Cluster Name (e.g., my-ecs-cluster): tour-management-cluster
Enter VPC ID (e.g., vpc-0abc123def456): vpc-0abc123def456
Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): subnet-0abc123,subnet-0def456
Enter Security Group ID (e.g., sg-0abc123def): sg-0abc123def
Enter Docker Image URI: 123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management-cont:v1.0.0
Do you need a load balancer for this service? (y/n): y
```

The script will:
1. Create ECS cluster (if not exists)
2. Create Application Load Balancer and Target Group (if requested)
3. Register task definition
4. Create or update ECS service
5. Wait for service to stabilize
6. Display deployment status and URLs

#### Step 4: Verify Deployment

**Check service status:**
```bash
aws ecs describe-services \
  --cluster tour-management-cluster \
  --services tour-management-cont-service \
  --region us-east-1
```

**List running tasks:**
```bash
aws ecs list-tasks \
  --cluster tour-management-cluster \
  --service-name tour-management-cont-service \
  --region us-east-1
```

**Access the application:**
- If load balancer was created: `http://<alb-dns-name>`
- Direct task access: `http://<task-public-ip>:8080`

---

## ECS Task Definition Explained

### Key Configuration Parameters

#### CPU and Memory (Fargate)

**Valid Combinations:**

| CPU (vCPU) | Memory (MB) |
|------------|-------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (increments of 1024) |
| 2048 (2)   | 4096-16384 (increments of 1024) |
| 4096 (4)   | 8192-30720 (increments of 1024) |

**Default Configuration:**
- CPU: `512` (.5 vCPU)
- Memory: `1024` MB

#### Container Definition

```json
{
  "name": "tour-management-cont",
  "image": "<image-uri>",
  "essential": true,
  "portMappings": [
    {
      "containerPort": 8080,
      "protocol": "tcp"
    }
  ]
}
```

#### Logging Configuration

```json
"logConfiguration": {
  "logDriver": "awslogs",
  "options": {
    "awslogs-group": "/ecs/tour-management-cont",
    "awslogs-region": "us-east-1",
    "awslogs-stream-prefix": "ecs",
    "awslogs-create-group": "true"
  }
}
```

---

## ECS Service Configuration

### Service Parameters

- **Desired Count**: Number of task instances (default: 2)
- **Launch Type**: FARGATE
- **Network Mode**: awsvpc (required for Fargate)
- **Deployment Configuration**:
  - Maximum Percent: 200 (allows rolling updates)
  - Minimum Healthy Percent: 50 (ensures availability)

### Service Auto Scaling

**Create scaling policy:**
```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tour-management-cluster/tour-management-cont-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1

aws application-autoscaling put-scaling-policy \
  --policy-name tour-management-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tour-management-cluster/tour-management-cont-service \
  --scalable-dimension ecs:service:DesiredCount \
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
  "ScaleOutCooldown": 60,
  "ScaleInCooldown": 120
}
```

---

## Configuration Management

### Environment Variables

Update task definition with environment variables:

```json
"environment": [
  {
    "name": "ASPNETCORE_ENVIRONMENT",
    "value": "Production"
  },
  {
    "name": "ConnectionStrings__DefaultConnection",
    "value": "Server=<db-host>;Database=<db-name>;User=<user>;Password=<password>"
  }
]
```

### Secrets Management

**Use AWS Secrets Manager:**

1. Create secret:
```bash
aws secretsmanager create-secret \
  --name tour-management-cont/db-password \
  --secret-string "my-secure-password" \
  --region us-east-1
```

2. Reference in task definition:
```json
"secrets": [
  {
    "name": "DB_PASSWORD",
    "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:tour-management-cont/db-password"
  }
]
```

---

## Monitoring and Logging

### CloudWatch Logs

**View logs:**
```bash
aws logs tail /ecs/tour-management-cont --follow --region us-east-1
```

**Query logs:**
```bash
aws logs filter-log-events \
  --log-group-name /ecs/tour-management-cont \
  --filter-pattern "ERROR" \
  --region us-east-1
```

### CloudWatch Metrics

**Key metrics to monitor:**
- `CPUUtilization`
- `MemoryUtilization`
- `TargetResponseTime`
- `HealthyHostCount`
- `UnHealthyHostCount`

**Create CloudWatch Dashboard:**
```bash
aws cloudwatch put-dashboard \
  --dashboard-name tour-management-cont-dashboard \
  --dashboard-body file://dashboard.json \
  --region us-east-1
```

---

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptoms:** Task status shows `STOPPED` immediately after creation.

**Solutions:**
- Check CloudWatch logs for startup errors
- Verify IAM execution role has ECR and CloudWatch permissions
- Ensure image URI is correct and accessible
- Verify CPU/memory configuration is valid

**Debug commands:**
```bash
# Describe task
aws ecs describe-tasks \
  --cluster tour-management-cluster \
  --tasks <task-arn> \
  --region us-east-1

# Check stopped reason
aws ecs describe-tasks \
  --cluster tour-management-cluster \
  --tasks <task-arn> \
  --query 'tasks[0].stoppedReason' \
  --region us-east-1
```

#### 2. Service Not Reaching Stable State

**Symptoms:** Service stuck in deployment, tasks repeatedly starting and stopping.

**Solutions:**
- Check health check configuration
- Verify application starts within `startPeriod` timeout
- Review application logs for startup errors
- Ensure security group allows traffic on application port

#### 3. Cannot Pull Image from ECR

**Symptoms:** Error: "CannotPullContainerError: pull image manifest has been retried"

**Solutions:**
- Verify execution role has `ecr:GetAuthorizationToken` permission
- Ensure subnets have internet access (via Internet Gateway or NAT Gateway)
- Check ECR repository policy allows access

#### 4. Out of Memory Errors

**Symptoms:** Tasks stop with "OutOfMemoryError" in CloudWatch logs.

**Solutions:**
- Increase task memory allocation
- Review .NET garbage collection settings
- Enable server GC mode for better performance:
  ```json
  "environment": [
    {"name": "COMPlus_gcServer", "value": "1"}
  ]
  ```

#### 5. High CPU Usage

**Solutions:**
- Enable tiered compilation for faster startup:
  ```json
  "environment": [
    {"name": "DOTNET_TieredCompilation", "value": "1"}
  ]
  ```
- Consider ReadyToRun images for improved performance
- Increase CPU allocation if consistently high

---

## Security Considerations

### 1. Container Security

- **Run as non-root user**: Dockerfile creates and uses `appuser`
- **Minimal base image**: Use official Microsoft runtime images
- **Regular updates**: Keep base images and dependencies updated

### 2. Network Security

- **Security Groups**: Restrict inbound traffic to necessary ports only
- **Private Subnets**: Deploy tasks in private subnets with NAT Gateway
- **TLS/HTTPS**: Use ALB with SSL certificate for encrypted traffic

**Create ALB with HTTPS listener:**
```bash
aws elbv2 create-listener \
  --load-balancer-arn <alb-arn> \
  --protocol HTTPS \
  --port 443 \
  --certificates CertificateArn=<certificate-arn> \
  --default-actions Type=forward,TargetGroupArn=<target-group-arn> \
  --region us-east-1
```

### 3. Secrets Management

- **Never hardcode secrets** in environment variables
- **Use AWS Secrets Manager** for sensitive data
- **Enable secret rotation** for database credentials

### 4. IAM Best Practices

- **Least privilege**: Grant only necessary permissions
- **Separate roles**: Use different roles for execution and task
- **Audit access**: Enable CloudTrail logging

### 5. Application Security

- **Enable HTTPS redirection** in ASP.NET Core
- **Configure CORS** properly for API endpoints
- **Implement authentication/authorization** middleware
- **Enable request validation** and input sanitization

---

## Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/fargate/)
- [.NET on AWS](https://aws.amazon.com/developer/language/net/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)

---

## Support

For issues or questions:
1. Check CloudWatch logs: `/ecs/tour-management-cont`
2. Review ECS service events
3. Consult AWS support or documentation
4. Contact your DevOps team

---

**Generated by Claude Code Containerization Expert**
**Last Updated:** 2026-01-09