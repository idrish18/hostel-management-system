# 🐳 Docker & Docker Compose Setup Guide

## Overview

This guide explains how to run the Smart Hostel Management System using Docker and Docker Compose with PostgreSQL database and Redis cache.

---

## 📋 Prerequisites

- **Docker Desktop** (Windows/Mac) or **Docker Engine** (Linux)
  - Download: https://www.docker.com/products/docker-desktop
- **Docker Compose** (usually comes with Docker Desktop)
  - Verify: `docker-compose --version`
- **Git** (optional, for cloning repository)

### System Requirements

- **CPU**: 2+ cores
- **RAM**: 4+ GB (8+ GB recommended)
- **Disk Space**: 5+ GB for images and volumes

---

## 🚀 Quick Start (5 Minutes)

### 1. Clone/Navigate to Project

```bash
cd hostel-management-system
```

### 2. Create Environment File

```bash
# Copy example env file
cp .env.example .env

# Edit .env with your values (optional for development)
# For development, defaults in .env.example are fine
```

### 3. Start All Services

```bash
# Start all containers (PostgreSQL, Redis, .NET App)
docker-compose up -d

# View logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f app
docker-compose logs -f postgres
docker-compose logs -f redis
```

### 4. Access Services

- **API Documentation**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **PgAdmin (Database UI)**: http://localhost:5050
  - Email: `admin@smarthostel.com`
  - Password: `AdminPassword123!`

### 5. Stop Services

```bash
docker-compose down

# Also remove volumes (WARNING: deletes data)
docker-compose down -v
```

---

## 🐳 Services Overview

### PostgreSQL Database
```
Container Name: smarthostel-postgres
Image: postgres:16-alpine
Port: 5432 (localhost:5432)
Database: SmartHostelDB
User: postgres
Password: SecurePassword123! (default)
```

**Access PostgreSQL:**
```bash
# From host machine
psql -h localhost -U postgres -d SmartHostelDB

# From container
docker-compose exec postgres psql -U postgres -d SmartHostelDB
```

### Redis Cache
```
Container Name: smarthostel-redis
Image: redis:7-alpine
Port: 6379 (localhost:6379)
Password: RedisSecure123! (default)
```

**Access Redis:**
```bash
# From host machine
redis-cli -p 6379 -a RedisSecure123!

# From container
docker-compose exec redis redis-cli -a RedisSecure123!
```

### .NET Application
```
Container Name: smarthostel-app
Image: Built from Dockerfile
Ports: 5000 (HTTP), 5001 (HTTPS)
```

**View Application Logs:**
```bash
docker-compose logs -f app
```

### PgAdmin (Database Management)
```
Container Name: smarthostel-pgadmin
Image: dpage/pgadmin4:latest
Port: 5050 (http://localhost:5050)
Email: admin@smarthostel.com
Password: AdminPassword123! (default)
```

---

## ⚙️ Environment Variables (.env)

### Database Configuration
```env
DB_NAME=SmartHostelDB           # PostgreSQL database name
DB_USER=postgres                # PostgreSQL user
DB_PASSWORD=SecurePassword123!  # PostgreSQL password
DB_PORT=5432                    # PostgreSQL port
```

### Redis Configuration
```env
REDIS_PASSWORD=RedisSecure123!  # Redis password
REDIS_PORT=6379                 # Redis port
```

### Application Configuration
```env
ASPNETCORE_ENVIRONMENT=Production  # Environment (Development/Staging/Production)
APP_PORT_HTTP=5000              # HTTP port
APP_PORT_HTTPS=5001             # HTTPS port
LOG_LEVEL=Information           # Logging level
```

### JWT Configuration
```env
JWT_SECRET_KEY=your-secret-key  # Min 32 characters (CHANGE IN PRODUCTION!)
JWT_ISSUER=SmartHostelAPI
JWT_AUDIENCE=SmartHostelClient
JWT_EXPIRES_HOURS=24
```

---

## 📁 Docker Files Explanation

### Dockerfile
Multi-stage build file:
- **Build Stage**: Compiles .NET application
- **Publish Stage**: Publishes Release build
- **Runtime Stage**: Runs optimized production image

**Features:**
- Alpine-based for small image size
- Health check included
- Minimal dependencies
- Production-optimized

### docker-compose.yml
Orchestration file defining all services:
- **postgres**: PostgreSQL database
- **redis**: Redis cache
- **app**: .NET application
- **pgadmin**: Database management UI

**Features:**
- Service dependencies
- Health checks
- Volume persistence
- Network isolation
- Environment variables
- Logging configuration

### .env.example
Example environment variables file:
- Copy to `.env` for local setup
- Change values as needed
- Never commit `.env` to version control

### .dockerignore
Specifies files to exclude from Docker build:
- Reduces image size
- Improves build speed
- Prevents unnecessary copying

### init-db.sql
Database initialization script:
- Runs on first PostgreSQL container start
- Creates extensions
- Sets up users and permissions
- Idempotent (safe to run multiple times)

---

## 🛠️ Common Commands

### Build

```bash
# Build Docker image
docker-compose build

# Build specific service
docker-compose build app

# Build without cache
docker-compose build --no-cache
```

### Run

```bash
# Start all services
docker-compose up

# Start in background
docker-compose up -d

# Start specific service
docker-compose up -d postgres
```

### View Logs

```bash
# View all logs
docker-compose logs

# Follow logs (real-time)
docker-compose logs -f

# View specific service logs
docker-compose logs -f app

# View last 100 lines
docker-compose logs --tail=100
```

### Execute Commands

```bash
# Run command in container
docker-compose exec app dotnet --version

# Open bash shell in container
docker-compose exec app bash

# Access database
docker-compose exec postgres psql -U postgres
```

### Stop & Remove

```bash
# Stop all services
docker-compose stop

# Stop and remove containers
docker-compose down

# Remove containers, volumes, and networks
docker-compose down -v

# Remove unused images
docker image prune
```

### Health Check

```bash
# Check service status
docker-compose ps

# Check health
docker-compose exec app curl http://localhost:5000/health
```

---

## 🔍 Troubleshooting

### Issue: "Cannot connect to Docker daemon"
**Solution:**
```bash
# Ensure Docker Desktop is running (Windows/Mac)
# Or start Docker service (Linux)
sudo systemctl start docker

# Verify Docker installation
docker --version
```

### Issue: Port already in use
**Solution:**
```bash
# Option 1: Change port in .env
# Edit APP_PORT_HTTP=5000 to APP_PORT_HTTP=5001

# Option 2: Kill process using port
# Windows (PowerShell)
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

### Issue: Database connection failed
**Solution:**
```bash
# Check if postgres container is running
docker-compose ps

# View postgres logs
docker-compose logs postgres

# Verify connection string matches container name
# Should be: Server=postgres;Port=5432;...
# NOT: Server=localhost;...
```

### Issue: Redis connection failed
**Solution:**
```bash
# Check if redis container is running
docker-compose ps

# Test Redis connection
docker-compose exec redis redis-cli -a RedisSecure123! ping

# Should return: PONG
```

### Issue: App crashes on startup
**Solution:**
```bash
# View app logs
docker-compose logs app

# Check migrations need to run
# App should run migrations automatically

# Restart app
docker-compose restart app

# Rebuild app
docker-compose up -d --build app
```

### Issue: Volume permission errors
**Solution:**
```bash
# Linux: Fix volume permissions
sudo chown -R 999:999 postgres_data

# Or recreate volumes
docker-compose down -v
docker-compose up -d
```

---

## 📊 Monitoring & Performance

### View Resource Usage

```bash
# Real-time monitoring
docker stats

# Monitor specific container
docker stats smarthostel-app
```

### Check Container Health

```bash
# Check all containers
docker-compose ps

# Check specific container
docker-compose exec app curl http://localhost:5000/health
```

### Logs Rotation

Logs are automatically rotated (max 10MB per file, max 5 files)

---

## 🔒 Security Best Practices

### For Production

1. **Change Default Passwords**
   ```bash
   # Edit .env file
   DB_PASSWORD=YourStrongPassword123!
   REDIS_PASSWORD=YourRedisPassword123!
   JWT_SECRET_KEY=YourLongSecretKey32+ chars
   ```

2. **Use Secret Management**
   ```bash
   # Use Docker secrets or external secret manager
   # Don't commit .env to version control
   ```

3. **Enable HTTPS**
   ```bash
   # Mount SSL certificates
   # Configure ASPNETCORE_URLS
   ```

4. **Network Security**
   ```bash
   # Use custom network
   # Restrict port exposure
   # Use firewall rules
   ```

5. **Backup Database**
   ```bash
   # Regular backups
   docker-compose exec postgres pg_dump -U postgres SmartHostelDB > backup.sql
   ```

---

## 📦 Backup & Restore

### Backup Database

```bash
# Create SQL dump
docker-compose exec postgres pg_dump -U postgres SmartHostelDB > backup.sql

# Backup volumes
docker run --rm -v postgres_data:/data -v $(pwd):/backup \
  alpine tar czf /backup/postgres_backup.tar.gz /data
```

### Restore Database

```bash
# Restore from SQL dump
cat backup.sql | docker-compose exec -T postgres psql -U postgres

# Restore from volume backup
docker run --rm -v postgres_data:/data -v $(pwd):/backup \
  alpine tar xzf /backup/postgres_backup.tar.gz -C /
```

---

## 🔄 Updating the Application

### Update Code and Redeploy

```bash
# Pull latest code
git pull origin main

# Rebuild application
docker-compose build app

# Restart services
docker-compose up -d
```

### Update Dependencies

```bash
# Rebuild with no cache
docker-compose build --no-cache app

# Restart
docker-compose up -d
```

---

## 📝 Docker Compose Profiles (Advanced)

### Development Profile

```bash
# Run with development profile
docker-compose --profile dev up -d

# Includes: app, postgres, redis, pgadmin, debugger
```

### Production Profile

```bash
# Run production only (no PgAdmin)
docker-compose --profile prod up -d
```

---

## 🚀 Deployment Scenarios

### Local Development

```bash
docker-compose up -d
# All services running locally
```

### Staging Environment

```bash
# Build and push image
docker build -t myrepo/smarthostel:staging .
docker push myrepo/smarthostel:staging

# Use in compose
docker-compose up -d
```

### Production Deployment

```bash
# Use Docker Swarm or Kubernetes
# Push to registry
docker build -t registry.example.com/smarthostel:latest .
docker push registry.example.com/smarthostel:latest

# Deploy via orchestration
docker service create --name smarthostel \
  -p 5000:5000 \
  registry.example.com/smarthostel:latest
```

---

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
- [Redis Docker Image](https://hub.docker.com/_/redis)
- [ASP.NET Core Docker Images](https://hub.docker.com/_/microsoft-dotnet-aspnet)

---

## ✅ Verification Checklist

```
Before going to production:

□ Change all default passwords
□ Generate strong JWT secret key
□ Configure HTTPS/SSL certificates
□ Set up backup strategy
□ Configure logging and monitoring
□ Test database backups
□ Test failover scenarios
□ Set resource limits
□ Configure health checks
□ Set up CI/CD pipeline
□ Document deployment procedure
```

---

## 📞 Support

For issues or questions:
1. Check troubleshooting section above
2. View container logs: `docker-compose logs -f`
3. Check Docker documentation
4. Review application logs

---

**Version**: 1.0  
**Last Updated**: April 2026  
**Status**: Production Ready
