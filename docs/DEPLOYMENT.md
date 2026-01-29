# Tour Management Application - Deployment Guide

## Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Local Development Setup](#local-development-setup)
4. [Docker Deployment](#docker-deployment)
5. [AWS ECS Fargate Deployment](#aws-ecs-fargate-deployment)
6. [Configuration Management](#configuration-management)
7. [Troubleshooting](#troubleshooting)
8. [Monitoring and Logging](#monitoring-and-logging)
9. [Security Considerations](#security-considerations)
10. [Scaling and Performance](#scaling-and-performance)

---

## Overview

This guide provides comprehensive instructions for deploying the Tour Management application, a .NET Framework 4.7.2 ASP.NET Web Forms application, to AWS ECS Fargate using Windows containers.

### Application Details
- **Framework**: .NET Framework 4.7.2
- **Application Type**: ASP.NET Web Forms
- **Runtime**: Windows Server Core 2022
- **Port**: 80 (HTTP)
- **Health Check Endpoint**: `/health.aspx`
- **Database**: SQL Server (external)

---

## Prerequisites

### Required Software
1. **Docker Desktop** (with Windows containers enabled)
   - Download: https://www.docker.com/products/docker-desktop
   - Minimum version: 20.10+
   - Enable Windows containers in Docker settings

2. **AWS CLI**
   - Download: https://aws.amazon.com/cli/
   - Minimum version: 2.0+
   - Configure credentials: `aws configure`

3. **PowerShell** (Windows)
   - Version 5.1 or higher
   - Pre-installed on Windows 10/11

4. **jq** (optional, for JSON processing)
   - Download: https://stedolan.github.io/jq/
   - Used for service definition manipulation

### AWS Prerequisites
1. **AWS Account** with appropriate permissions
2. **IAM Roles**:
   - `ecsTaskExecutionRole` - Allows ECS to pull images and write logs
   - `ecsTaskRole` - Grants permissions to the application (optional)
3. **VPC Configuration**:
   - VPC with public and/or private subnets
   - At least 2 subnets in different availability zones
   - Internet Gateway (for public subnets)
   - NAT Gateway (for private subnets with external access)
4. **Security Group**:
   - Inbound: Port 80 (HTTP) from ALB or 0.0.0.0/0
   - Outbound: All traffic (for database and external service access)
5. **SQL Server Database**:
   - Amazon RDS SQL Server instance
   - Or external SQL Server accessible from ECS tasks
   - Database name: `tourdb` (configurable)

### IAM Role Setup

#### ecsTaskExecutionRole
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

Attach managed policies:
- `AmazonECSTaskExecutionRolePolicy`
- `CloudWatchLogsFullAccess` (or custom policy for log group access)

#### ecsTaskRole (optional)
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

Attach policies based on application needs (S3, DynamoDB, etc.)

---

## Local Development Setup

### 1. Clone or Extract Project
```bash
cd /path/to/tour-management
```

### 2. Configure Environment Variables
Create a `.env` file in the project root:
```env
DB_SERVER=localhost,1433
DB_NAME=tourdb
DB_USER=sa
DB_PASSWORD=YourStrong@Passw0rd
```

### 3. Build Docker Image Locally
**Windows (PowerShell):**
```powershell
docker build -f Dockerfile -t tour-management:local .
```

**Linux/macOS (using Windows containers via Docker Desktop):**
```bash
docker build -f Dockerfile -t tour-management:local .
```

### 4. Run Container Locally
```bash
docker-compose up -d
```

Access the application:
- **Application**: http://localhost:8080
- **Health Check**: http://localhost:8080/health.aspx

### 5. View Logs
```bash
docker-compose logs -f tour-management-app
```

### 6. Stop and Remove
```bash
docker-compose down
```

---

## Docker Deployment

### Build and Push to Registry

Use the provided scripts to build and push your Docker image to AWS ECR or Docker Hub.

#### Linux/macOS
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

#### Windows
```cmd
.\scripts\build-push.bat
```

**Script Workflow**:
1. Prompts for registry type (AWS ECR or Docker Hub)
2. Collects registry credentials and configuration
3. Authenticates with the selected registry
4. Creates ECR repository if it doesn't exist (AWS ECR only)
5. Builds Docker image with sanitized tags
6. Pushes image to registry
7. Displays image URI for deployment

**Example Output**:
```
Image: 123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management:v1.0
Tag: v1.0

Next Steps:
1. Update ecs/task-definition.json with image URI
2. Run ./scripts/deploy-image.sh to deploy to ECS
```

---

## AWS ECS Fargate Deployment

### Architecture Overview

```
┌─────────────────┐
│  Internet       │
└────────┬────────┘
         │
┌────────▼────────┐
│ Application     │
│ Load Balancer   │
│ (Optional)      │
└────────┬────────┘
         │
┌────────▼────────────────────┐
│  ECS Service                │
│  ┌──────────────────────┐   │
│  │ Task (Container)     │   │
│  │ - tour-management    │   │
│  │ - Windows Server     │   │
│  │ - Port 80            │   │
│  └──────────────────────┘   │
└─────────────────────────────┘
         │
┌────────▼────────┐
│ RDS SQL Server  │
│ Database        │
└─────────────────┘
```

### ECS Task Definition Overview

The task definition (`ecs/task-definition.json`) specifies:

- **Launch Type**: FARGATE
- **Network Mode**: awsvpc (required for Fargate)
- **CPU**: 1024 (1 vCPU) - Windows containers require minimum 1 vCPU
- **Memory**: 2048 MB (2 GB) - Valid combination for Windows Fargate
- **Runtime Platform**: Windows Server 2022 Core
- **Execution Role**: ecsTaskExecutionRole (for pulling images and logs)
- **Task Role**: ecsTaskRole (for application permissions)

**Valid CPU/Memory Combinations for Windows Fargate**:
- CPU: 1024 (.5 vCPU) → Memory: 2048-8192 MB
- CPU: 2048 (1 vCPU) → Memory: 4096-16384 MB
- CPU: 4096 (2 vCPU) → Memory: 8192-30720 MB

### ECS Service Configuration

The service definition (`ecs/service-definition.json`) specifies:

- **Desired Count**: 2 (for high availability)
- **Network Configuration**: awsvpc with specified subnets and security groups
- **Load Balancer**: Optional ALB with target group
- **Health Check Grace Period**: 300 seconds (for Windows startup)
- **Deployment Configuration**: Rolling updates with circuit breaker
- **Tags**: Service-level tags propagated to tasks

### Deployment Steps

#### Step 1: Prepare Infrastructure

1. **Create or identify VPC and subnets**:
   ```bash
   aws ec2 describe-vpcs --region us-east-1
   aws ec2 describe-subnets --region us-east-1
   ```

2. **Create security group** (if not exists):
   ```bash
   aws ec2 create-security-group \
     --group-name tour-management-sg \
     --description "Security group for Tour Management ECS tasks" \
     --vpc-id vpc-xxxxxxxxx \
     --region us-east-1
   
   # Allow inbound HTTP traffic
   aws ec2 authorize-security-group-ingress \
     --group-id sg-xxxxxxxxx \
     --protocol tcp \
     --port 80 \
     --cidr 0.0.0.0/0 \
     --region us-east-1
   ```

3. **Create CloudWatch log group** (optional, auto-created by script):
   ```bash
   aws logs create-log-group \
     --log-group-name /ecs/tour-management \
     --region us-east-1
   ```

#### Step 2: Deploy Application

**Linux/macOS**:
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

**Windows**:
```cmd
.\scripts\deploy-image.bat
```

**Deployment Script Workflow**:

1. **AWS Configuration**:
   - Prompts for AWS region and ECS cluster name
   - Retrieves AWS Account ID
   - Creates ECS cluster if it doesn't exist

2. **Network Configuration**:
   - Prompts for VPC ID, subnet IDs, and security group ID
   - Validates network resources

3. **Container Image Configuration**:
   - Prompts for Docker image URI (from build-push script output)

4. **Database Configuration**:
   - Prompts for database server, name, user, and password
   - Updates task definition environment variables

5. **Load Balancer Configuration**:
   - Asks if load balancer is needed
   - If yes:
     - Creates Application Load Balancer
     - Creates Target Group with IP target type (required for Fargate)
     - Creates Listener on port 80
     - Captures Target Group ARN
   - If no:
     - Removes load balancer configuration from service definition

6. **Task Definition Registration**:
   - Updates task definition JSON with provided values
   - Registers task definition with ECS
   - Captures task definition ARN

7. **Service Creation/Update**:
   - Checks if service exists
   - Creates new service or updates existing service
   - Uses full task definition ARN for updates

8. **Service Stabilization**:
   - Waits for service to reach stable state
   - Monitors running task count

9. **Deployment Verification**:
   - Displays service status and running tasks
   - Shows load balancer DNS (if applicable)
   - Provides CloudWatch log group name

**Example Deployment Prompts**:
```
Enter AWS region (e.g., us-east-1): us-east-1
Enter ECS cluster name (e.g., tour-management-cluster): tour-prod-cluster
AWS Account ID: 123456789012

Enter VPC ID (e.g., vpc-0abc123def456): vpc-0a1b2c3d4e5f6
Enter Subnet IDs comma-separated: subnet-0abc123,subnet-0def456
Enter Security Group ID (e.g., sg-0abc123def): sg-0a1b2c3d

Enter Docker image URI: 123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management:v1.0

Enter Database Server: tour-db.abc123.us-east-1.rds.amazonaws.com
Enter Database Name (default: tourdb): tourdb
Enter Database User (default: admin): admin
Enter Database Password: **********

Do you need a load balancer for this service? (y/n): y
Creating Application Load Balancer...
ALB ARN: arn:aws:elasticloadbalancing:us-east-1:123456789012:loadbalancer/app/tour-management-alb/abc123
Target Group ARN: arn:aws:elasticloadbalancing:us-east-1:123456789012:targetgroup/tour-management-tg/def456
Load Balancer DNS: tour-management-alb-123456789.us-east-1.elb.amazonaws.com

Registering ECS task definition...
Task Definition ARN: arn:aws:ecs:us-east-1:123456789012:task-definition/tour-management-task:1

Creating new ECS service...
Waiting for service to stabilize...
Service is stable

Deployment Complete!
Cluster: tour-prod-cluster
Service: tour-management-service
Load Balancer DNS: http://tour-management-alb-123456789.us-east-1.elb.amazonaws.com
Health Check URL: http://tour-management-alb-123456789.us-east-1.elb.amazonaws.com/health.aspx
CloudWatch Logs: /ecs/tour-management
```

#### Step 3: Verify Deployment

1. **Check service status**:
   ```bash
   aws ecs describe-services \
     --cluster tour-prod-cluster \
     --services tour-management-service \
     --region us-east-1
   ```

2. **Check running tasks**:
   ```bash
   aws ecs list-tasks \
     --cluster tour-prod-cluster \
     --service-name tour-management-service \
     --region us-east-1
   ```

3. **Test health endpoint** (if using load balancer):
   ```bash
   curl http://tour-management-alb-123456789.us-east-1.elb.amazonaws.com/health.aspx
   ```

4. **View logs**:
   ```bash
   aws logs tail /ecs/tour-management --follow --region us-east-1
   ```

---

## Configuration Management

### Environment Variables

The application uses the following environment variables (configured in task definition):

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `ASPNET_ENV` | ASP.NET environment | Production | No |
| `DB_SERVER` | SQL Server hostname/IP | - | Yes |
| `DB_NAME` | Database name | tourdb | Yes |
| `DB_USER` | Database username | - | Yes |
| `DB_PASSWORD` | Database password | - | Yes |

### Web.config Transformation

The application uses Web.config with connection string placeholders:

```xml
<connectionStrings>
  <add name="dbconnection" 
       connectionString="Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD}" 
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

Environment variables are substituted at runtime by the container.

### Secrets Management

For production deployments, use AWS Secrets Manager or Parameter Store:

1. **Store database credentials in Secrets Manager**:
   ```bash
   aws secretsmanager create-secret \
     --name tour-management/db-credentials \
     --secret-string '{"username":"admin","password":"YourSecurePassword"}' \
     --region us-east-1
   ```

2. **Update task definition to use secrets**:
   ```json
   "secrets": [
     {
       "name": "DB_USER",
       "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:tour-management/db-credentials:username::"
     },
     {
       "name": "DB_PASSWORD",
       "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:tour-management/db-credentials:password::"
     }
   ]
   ```

3. **Grant ecsTaskExecutionRole access to secrets**:
   ```json
   {
     "Version": "2012-10-17",
     "Statement": [
       {
         "Effect": "Allow",
         "Action": [
           "secretsmanager:GetSecretValue"
         ],
         "Resource": "arn:aws:secretsmanager:us-east-1:123456789012:secret:tour-management/db-credentials*"
       }
     ]
   }
   ```

---

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptom**: Task status shows `STOPPED` immediately after launch

**Possible Causes**:
- Invalid CPU/memory combination
- Image pull errors
- IAM role permissions
- Network configuration issues

**Resolution**:
```bash
# Check task stopped reason
aws ecs describe-tasks \
  --cluster tour-prod-cluster \
  --tasks <task-id> \
  --region us-east-1 \
  --query 'tasks[0].stoppedReason'

# Check CloudWatch logs
aws logs tail /ecs/tour-management --follow --region us-east-1
```

#### 2. Health Check Failures

**Symptom**: Tasks continuously restart, health checks fail

**Possible Causes**:
- Database connectivity issues
- Incorrect health check endpoint
- Insufficient health check grace period
- Application startup time exceeds timeout

**Resolution**:
1. **Verify database connectivity**:
   - Check security group rules allow traffic to database
   - Verify database server hostname resolves correctly
   - Test database credentials

2. **Increase health check grace period**:
   ```bash
   aws ecs update-service \
     --cluster tour-prod-cluster \
     --service tour-management-service \
     --health-check-grace-period-seconds 600 \
     --region us-east-1
   ```

3. **Check health endpoint manually**:
   ```bash
   # Get task private IP
   TASK_IP=$(aws ecs describe-tasks \
     --cluster tour-prod-cluster \
     --tasks <task-id> \
     --region us-east-1 \
     --query 'tasks[0].attachments[0].details[?name==`privateIPv4Address`].value' \
     --output text)
   
   # Test health endpoint (from within VPC)
   curl http://$TASK_IP/health.aspx
   ```

#### 3. Image Pull Errors

**Symptom**: Task fails with "CannotPullContainerError"

**Possible Causes**:
- ECR repository doesn't exist
- Incorrect image URI
- IAM permissions missing

**Resolution**:
1. **Verify image exists**:
   ```bash
   aws ecr describe-images \
     --repository-name tour-management \
     --region us-east-1
   ```

2. **Check ecsTaskExecutionRole has ECR permissions**:
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
           "ecr:BatchGetImage"
         ],
         "Resource": "*"
       }
     ]
   }
   ```

#### 4. Windows Container Specific Issues

**Symptom**: Container fails to start or behaves unexpectedly

**Possible Causes**:
- Incorrect Windows Server version
- PowerShell script errors
- IIS configuration issues

**Resolution**:
1. **Check task logs for PowerShell errors**:
   ```bash
   aws logs tail /ecs/tour-management --follow --region us-east-1
   ```

2. **Verify Windows Server version compatibility**:
   - Task definition must specify `WINDOWS_SERVER_2022_CORE`
   - Base image must match: `windowsservercore-ltsc2022`

3. **Test container locally**:
   ```powershell
   docker run -it --rm tour-management:local powershell
   # Inside container:
   Get-Website
   Invoke-WebRequest http://localhost/health.aspx
   ```

#### 5. Service Not Registering with Target Group

**Symptom**: Load balancer shows no healthy targets

**Possible Causes**:
- Incorrect target type (must be `ip` for Fargate)
- Health check path incorrect
- Security group not allowing traffic from ALB

**Resolution**:
1. **Verify target group configuration**:
   ```bash
   aws elbv2 describe-target-groups \
     --names tour-management-tg \
     --region us-east-1 \
     --query 'TargetGroups[0].{TargetType:TargetType,HealthCheckPath:HealthCheckPath}'
   ```

2. **Check target health**:
   ```bash
   aws elbv2 describe-target-health \
     --target-group-arn <target-group-arn> \
     --region us-east-1
   ```

3. **Verify security group allows ALB traffic**:
   - ECS task security group must allow inbound from ALB security group on port 80

---

## Monitoring and Logging

### CloudWatch Logs

All container logs are automatically sent to CloudWatch Logs:

- **Log Group**: `/ecs/tour-management`
- **Log Stream**: `ecs/tour-management/<task-id>`

**View logs**:
```bash
# Tail logs in real-time
aws logs tail /ecs/tour-management --follow --region us-east-1

# Filter logs by time range
aws logs tail /ecs/tour-management \
  --since 1h \
  --follow \
  --region us-east-1

# Search for specific patterns
aws logs tail /ecs/tour-management \
  --filter-pattern "ERROR" \
  --follow \
  --region us-east-1
```

### CloudWatch Metrics

ECS automatically publishes metrics to CloudWatch:

**Service Metrics**:
- CPUUtilization
- MemoryUtilization
- TaskCount (Running, Pending, Desired)

**View metrics**:
```bash
# Get CPU utilization
aws cloudwatch get-metric-statistics \
  --namespace AWS/ECS \
  --metric-name CPUUtilization \
  --dimensions Name=ServiceName,Value=tour-management-service Name=ClusterName,Value=tour-prod-cluster \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average \
  --region us-east-1
```

### Application-Level Monitoring

For comprehensive monitoring, consider:

1. **Application Insights** (if migrating to .NET Core later)
2. **Custom CloudWatch Metrics** via SDK
3. **ALB Access Logs** for request tracking
4. **X-Ray** for distributed tracing

---

## Security Considerations

### Network Security

1. **Use Private Subnets**: Deploy ECS tasks in private subnets with NAT Gateway
2. **Restrict Security Groups**: Only allow necessary inbound traffic
3. **Enable VPC Flow Logs**: Monitor network traffic

### Application Security

1. **Use Secrets Manager**: Store database credentials securely
2. **Enable HTTPS**: Configure ALB with SSL/TLS certificate
3. **Implement WAF**: Add AWS WAF to ALB for protection
4. **Regular Updates**: Keep Windows base images updated

### IAM Best Practices

1. **Principle of Least Privilege**: Grant minimum required permissions
2. **Separate Execution and Task Roles**: 
   - Execution role for ECS infrastructure
   - Task role for application permissions
3. **Use Managed Policies**: Where appropriate, use AWS managed policies

### Container Security

1. **Scan Images**: Use ECR image scanning
   ```bash
   aws ecr start-image-scan \
     --repository-name tour-management \
     --image-id imageTag=latest \
     --region us-east-1
   ```

2. **Run as Non-Root**: Already configured in Dockerfile
3. **Read-Only Root Filesystem**: Consider for production

---

## Scaling and Performance

### Auto Scaling

Configure ECS Service Auto Scaling based on CloudWatch metrics:

```bash
# Register scalable target
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/tour-prod-cluster/tour-management-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1

# Create scaling policy (CPU-based)
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/tour-prod-cluster/tour-management-service \
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

### Performance Optimization

1. **Windows Container Optimization**:
   - Use Windows Server Core (not Full)
   - Minimize layer count in Dockerfile
   - Pre-compile ASP.NET views

2. **Database Connection Pooling**:
   - Configure connection pool size in connection string
   - Example: `Server=...;Max Pool Size=100;Min Pool Size=10`

3. **Application-Level Caching**:
   - Enable ASP.NET output caching
   - Use in-memory cache for frequently accessed data

4. **Resource Allocation**:
   - Start with CPU: 1024, Memory: 2048
   - Monitor and adjust based on actual usage
   - Windows containers require more resources than Linux

### Blue/Green Deployments

For zero-downtime deployments:

1. **Using ECS Deployment Configuration**:
   - Already configured with `maximumPercent: 200`
   - Allows new tasks to start before stopping old tasks

2. **Using CodeDeploy**:
   ```bash
   # Create CodeDeploy application and deployment group
   # Configure for ECS blue/green deployment
   # Integrate with ALB for traffic shifting
   ```

---

## Additional Resources

### AWS Documentation
- [Amazon ECS Developer Guide](https://docs.aws.amazon.com/ecs/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [Windows Containers on ECS](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/ECS_Windows.html)
- [ECS Task Definitions](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/task_definitions.html)

### Docker Documentation
- [Windows Container Documentation](https://docs.microsoft.com/en-us/virtualization/windowscontainers/)
- [Dockerfile Best Practices](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)

### .NET Framework Resources
- [ASP.NET Web Forms Documentation](https://docs.microsoft.com/en-us/aspnet/web-forms/)
- [Containerizing .NET Framework Applications](https://docs.microsoft.com/en-us/dotnet/framework/deployment/docker/)

---

## Support and Maintenance

### Regular Maintenance Tasks

1. **Update Base Images**: Monthly security updates
2. **Review CloudWatch Logs**: Weekly log analysis
3. **Monitor Costs**: AWS Cost Explorer for ECS/Fargate costs
4. **Backup Database**: Regular RDS snapshots
5. **Test Disaster Recovery**: Quarterly DR drills

### Rollback Procedures

If a deployment fails:

1. **Identify previous task definition**:
   ```bash
   aws ecs list-task-definitions \
     --family-prefix tour-management-task \
     --sort DESC \
     --region us-east-1
   ```

2. **Update service to previous version**:
   ```bash
   aws ecs update-service \
     --cluster tour-prod-cluster \
     --service tour-management-service \
     --task-definition tour-management-task:<previous-revision> \
     --force-new-deployment \
     --region us-east-1
   ```

3. **Monitor rollback**:
   ```bash
   aws ecs describe-services \
     --cluster tour-prod-cluster \
     --services tour-management-service \
     --region us-east-1
   ```

---

## Contact and Support

For issues or questions:
- AWS Support: https://console.aws.amazon.com/support/
- Docker Support: https://www.docker.com/support/
- Internal Team: [Your team contact information]

---

**Document Version**: 1.0
**Last Updated**: 2026-01-29
**Maintained By**: DevOps Team
