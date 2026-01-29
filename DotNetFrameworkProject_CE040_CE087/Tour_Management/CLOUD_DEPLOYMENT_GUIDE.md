# Cloud Deployment Guide - Tour Management Application

## Overview
This guide provides instructions for deploying the Tour Management application to AWS cloud environment.

## Cloud Readiness Fixes Applied

### 1. Configuration Management
- ✅ Replaced hardcoded database connection string with environment variables
- ✅ Replaced hardcoded Windows file paths with configurable paths
- ✅ Replaced hardcoded admin credentials with configuration-based authentication
- ✅ Added support for Linux-compatible paths (/tmp)

### 2. Security Improvements
- ✅ Fixed SQL injection vulnerability in user login (parameterized queries)
- ✅ Removed hardcoded credentials from code
- ✅ Implemented proper resource disposal patterns

### 3. Database Management
- ✅ Added proper using statements for all SqlConnection instances
- ✅ Implemented proper disposal patterns for database resources
- ✅ Prepared for cloud database migration (AWS RDS)

### 4. File System Management
- ✅ Made file upload paths configurable via environment variables
- ✅ Added directory creation logic for upload paths
- ✅ Prepared for cloud storage migration (AWS S3)

## Environment Variables Required

Create a `.env` file based on `.env.example` with the following variables:

```bash
# Database Connection
DB_CONNECTION_STRING="Data Source=your-rds.amazonaws.com;Initial Catalog=TourManagement;User ID=admin;Password=secure123"

# File Storage
UPLOAD_PATH="/var/app/uploads"
CHART_TEMP_DIR="/tmp/charts"

# Admin Credentials
ADMIN_EMAIL="admin@yourdomain.com"
ADMIN_PASSWORD="SecurePassword123!"
```

## AWS Deployment Steps

### 1. Database Migration (AWS RDS)

1. Create an AWS RDS SQL Server instance:
   ```bash
   aws rds create-db-instance \
     --db-instance-identifier tour-management-db \
     --db-instance-class db.t3.medium \
     --engine sqlserver-ex \
     --master-username admin \
     --master-user-password YourSecurePassword \
     --allocated-storage 20
   ```

2. Migrate local database to RDS:
   - Export local database: `tourdb.mdf`
   - Use SQL Server Management Studio or Azure Data Studio
   - Import to RDS instance

3. Update connection string in environment variables

### 2. File Storage Setup (AWS S3)

1. Create S3 bucket for file uploads:
   ```bash
   aws s3 mb s3://tour-management-uploads
   ```

2. Configure bucket policy for application access

3. Update application code to use S3 SDK for file operations (future enhancement)

### 3. Application Deployment (AWS Elastic Beanstalk)

1. Install AWS EB CLI:
   ```bash
   pip install awsebcli
   ```

2. Initialize Elastic Beanstalk:
   ```bash
   eb init tour-management --platform "IIS 10.0 running on 64bit Windows Server" --region us-east-1
   ```

3. Create environment:
   ```bash
   eb create tour-management-prod
   ```

4. Set environment variables:
   ```bash
   eb setenv DB_CONNECTION_STRING="..." ADMIN_EMAIL="..." ADMIN_PASSWORD="..."
   ```

5. Deploy application:
   ```bash
   eb deploy
   ```

### 4. Alternative: Docker + ECS Deployment

1. Create Dockerfile (for ASP.NET Framework on Windows Container)
2. Build and push to ECR
3. Deploy to ECS Fargate

## Security Recommendations

1. **Use AWS Secrets Manager** for sensitive data:
   ```bash
   aws secretsmanager create-secret \
     --name tour-management/admin-creds \
     --secret-string '{"email":"admin@domain.com","password":"secure123"}'
   ```

2. **Enable AWS WAF** for web application protection

3. **Use AWS Certificate Manager** for SSL/TLS certificates

4. **Configure Security Groups**:
   - Allow inbound HTTPS (443) from ALB
   - Allow outbound to RDS on port 1433
   - Restrict RDS access to application security group only

## Monitoring and Logging

1. **CloudWatch Logs**: Configure IIS logs to stream to CloudWatch
2. **CloudWatch Metrics**: Monitor application performance
3. **AWS X-Ray**: Add distributed tracing (optional)

## Cost Optimization

- Use RDS reserved instances for production
- Enable S3 lifecycle policies for old uploads
- Use Auto Scaling for EC2/ECS instances
- Consider AWS Lambda for background jobs

## Known Limitations

1. **ASP.NET Web Forms**: This legacy framework is not ideal for cloud. Consider migrating to ASP.NET Core for better cloud compatibility.
2. **LocalDB**: Completely replaced with RDS SQL Server
3. **File Storage**: Currently using local file system. Recommend migrating to S3 for scalability.

## Next Steps for Full Cloud-Native Migration

1. Migrate from ASP.NET Web Forms to ASP.NET Core
2. Implement S3 integration for file uploads
3. Add caching layer (ElastiCache Redis)
4. Implement JWT-based authentication
5. Add API Gateway for RESTful APIs
6. Implement CI/CD pipeline with AWS CodePipeline

## Support

For issues or questions, contact the cloud migration team.
