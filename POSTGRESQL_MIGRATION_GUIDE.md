# 📦 Migration from SQL Server to PostgreSQL

## Overview

This guide explains how to migrate the Smart Hostel Management System from SQL Server (LocalDB) to PostgreSQL for Docker deployment.

---

## ✅ Changes Made

### 1. **Project File (.csproj)**
- Added PostgreSQL EF Core Provider: `Npgsql.EntityFrameworkCore.PostgreSQL`
- Kept SQL Server provider for backward compatibility (optional)

### 2. **Program.cs**
Changed database configuration:
```csharp
// Old (SQL Server)
options.UseSqlServer(connectionString)

// New (PostgreSQL)
options.UseNpgsql(connectionString, x => x.MigrationsAssembly("SmartHostelManagementSystem"))
```

### 3. **Connection String (appsettings.json)**
```json
// Old (SQL Server)
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartHostelDB;Trusted_Connection=true;TrustServerCertificate=true;"

// New (PostgreSQL)
"DefaultConnection": "Server=localhost;Port=5432;Database=SmartHostelDB;User Id=postgres;Password=SecurePassword123!;Encrypt=false;TrustServerCertificate=false;"
```

---

## 🔄 Migration Steps

### Step 1: Update NuGet Packages

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

### Step 2: Recreate Migrations (if needed)

```bash
# Remove old migrations
cd newhms
dotnet ef migrations remove

# Create new migrations for PostgreSQL
dotnet ef migrations add InitialCreatePostgreSQL

# If using Code First with existing models
dotnet ef migrations add InitialCreate
```

### Step 3: Update Database

```bash
# Apply migrations
dotnet ef database update

# Or let Docker do it automatically
```

### Step 4: Run with Docker

```bash
docker-compose up -d
```

---

## 🗄️ Database Differences

### SQL Server vs PostgreSQL

| Feature | SQL Server | PostgreSQL |
|---------|-----------|-----------|
| **Connection String** | Uses `Server=`, `Trusted_Connection` | Uses `Server=`, `User Id=`, `Password=` |
| **Type System** | Specific types | Standard SQL types |
| **Case Sensitivity** | Case-insensitive by default | Case-sensitive (identifiers lowercase) |
| **GUID** | UNIQUEIDENTIFIER | uuid |
| **Boolean** | bit (0/1) | boolean |
| **Auto-increment** | IDENTITY | SERIAL / GENERATED ALWAYS |
| **DateTime** | DATETIME2 | timestamp without time zone |

### EF Core Provider Changes

```csharp
// SQL Server
options.UseSqlServer(connectionString);

// PostgreSQL
options.UseNpgsql(connectionString);

// Both with custom configuration
options.UseNpgsql(connectionString, x => 
{
    x.MigrationsAssembly("AssemblyName");
    x.UseAdminDatabase("postgres");  // Reconnect to admin DB after creation
});
```

---

## 🔧 Entity Configuration Updates

### Example: GUID Primary Keys

```csharp
// Both support GUID, but configuration differs slightly
modelBuilder.Entity<MyEntity>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // SQL Server
    entity.Property(e => e.Id)
        .HasDefaultValueSql("NEWID()");
    
    // PostgreSQL
    entity.Property(e => e.Id)
        .HasDefaultValueSql("gen_random_uuid()");
});
```

### Example: String Properties

```csharp
// Both are similar for string properties
entity.Property(e => e.Name)
    .IsRequired()
    .HasMaxLength(100);
```

### Example: Decimal Properties

```csharp
// Precision configuration is similar
entity.Property(e => e.Amount)
    .HasPrecision(18, 2);  // Works in both
```

---

## 📊 Migration Patterns

### Pattern 1: Keep Both Providers

```csharp
// Program.cs - conditionally select provider
var dbProvider = Environment.GetEnvironmentVariable("DB_PROVIDER") ?? "PostgreSQL";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (dbProvider == "SqlServer")
        options.UseSqlServer(connectionString);
    else
        options.UseNpgsql(connectionString);
});
```

**Environment variables:**
```bash
# .env
DB_PROVIDER=PostgreSQL  # or SqlServer
```

### Pattern 2: Conditional Configuration

```csharp
// ApplicationDbContext.cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        
        #if DEBUG
            optionsBuilder.UseNpgsql(connectionString);
        #else
            optionsBuilder.UseNpgsql(connectionString);
        #endif
    }
}
```

---

## 🔍 Testing the Migration

### 1. Local Development (PostgreSQL)

```bash
# Ensure PostgreSQL is running
docker-compose up -d postgres redis

# Wait for PostgreSQL to be ready
docker-compose exec postgres pg_isready

# Run migrations
dotnet ef database update

# Run application
dotnet run
```

### 2. Docker Deployment

```bash
# Start all services
docker-compose up -d

# Check logs
docker-compose logs -f app

# Verify database
docker-compose exec postgres psql -U postgres -d SmartHostelDB -c "\dt"
```

### 3. Verify Data

```bash
# Using PgAdmin
# Navigate to http://localhost:5050
# Login and browse databases

# Using psql
docker-compose exec postgres psql -U postgres -d SmartHostelDB

# Inside psql shell
\dt                           # List tables
SELECT * FROM "AspNetUsers"; # Query users
\q                           # Exit
```

---

## ⚠️ Common Issues & Solutions

### Issue: "Connection refused"
```
Error: Unable to connect to host postgres
```
**Solution:**
```bash
# Ensure containers are running
docker-compose ps

# Check logs
docker-compose logs postgres

# Wait for PostgreSQL to start
docker-compose exec postgres pg_isready

# Retry application
docker-compose restart app
```

### Issue: "Column name does not exist"
```
Error: column "columnName" of relation "table" does not exist
```
**Solution:**
```bash
# PostgreSQL is case-sensitive for unquoted identifiers
# Either:
# 1. Quote identifiers in EF Core
entity.Property(e => e.MyColumn).HasColumnName("\"MyColumn\"");

# 2. Use lowercase names
entity.Property(e => e.MyColumn).HasColumnName("mycolumn");

# 3. Use snake_case
entity.Property(e => e.MyColumn).HasColumnName("my_column");
```

### Issue: "Migrations not found"
```
Error: No executable found matching command "dotnet-ef"
```
**Solution:**
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Or update
dotnet tool update --global dotnet-ef

# Verify
dotnet ef --version
```

### Issue: "Type mismatch"
```
Error: Unable to cast object of type 'System.Int32' to type 'System.Int64'
```
**Solution:**
```csharp
# Ensure correct types in entities
public int Id { get; set; }      // int
public long Count { get; set; }  // long
public bool IsActive { get; set; } // bool (not bit)
```

---

## 🚀 Performance Tuning for PostgreSQL

### 1. Index Configuration

```csharp
modelBuilder.Entity<Student>(entity =>
{
    // Single column index
    entity.HasIndex(s => s.RollNumber);
    
    // Composite index
    entity.HasIndex(s => new { s.HostelId, s.Email });
    
    // Unique index
    entity.HasIndex(s => s.Email).IsUnique();
});
```

### 2. Query Optimization

```csharp
// Use projection for large datasets
var students = await context.Students
    .Where(s => s.HostelId == hostelId)
    .Select(s => new { s.Id, s.Name })  // Only select needed columns
    .ToListAsync();

// Use skip/take for pagination
var page = await context.Students
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 3. Connection Pool Configuration

```csharp
// In Program.cs
options.UseNpgsql(connectionString, x =>
{
    x.EnableRetryOnFailure();
    x.CommandTimeout(30);  // 30 second timeout
});
```

---

## 📚 PostgreSQL Best Practices

### 1. Naming Conventions

```sql
-- Use lowercase with underscores (snake_case)
CREATE TABLE student_records (
    student_id SERIAL PRIMARY KEY,
    roll_number VARCHAR(50) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Or use quoted identifiers for mixed case
CREATE TABLE "StudentRecords" (
    "StudentId" SERIAL PRIMARY KEY
);
```

### 2. Backup Strategy

```bash
# Regular backups
docker-compose exec postgres pg_dump -U postgres SmartHostelDB > backup.sql

# Automated backup
0 0 * * * docker-compose -f /path/to/docker-compose.yml exec -T postgres pg_dump -U postgres SmartHostelDB > /backups/backup_$(date +\%Y\%m\%d).sql
```

### 3. Monitoring

```bash
# Check connections
docker-compose exec postgres psql -U postgres -c "SELECT count(*) FROM pg_stat_activity;"

# Check table sizes
docker-compose exec postgres psql -U postgres -c "SELECT schemaname, tablename, pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) FROM pg_tables ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;"
```

---

## 🔄 Rollback to SQL Server

If you need to revert to SQL Server:

### 1. Update Program.cs

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

### 2. Update Connection String

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartHostelDB;Trusted_Connection=true;TrustServerCertificate=true;"
```

### 3. Update NuGet Reference

```bash
dotnet remove package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### 4. Recreate Migrations

```bash
dotnet ef migrations remove
dotnet ef migrations add InitialCreateSqlServer
dotnet ef database update
```

---

## 📋 Verification Checklist

```
□ PostgreSQL provider installed
□ Program.cs updated to use UseNpgsql()
□ Connection string updated
□ appsettings.json updated
□ Project builds without errors
□ Migrations created successfully
□ Docker containers start without errors
□ Database tables created
□ Application loads homepage
□ Swagger documentation works
□ Health check endpoint responds
□ Database backup created
```

---

## 📞 Additional Resources

- [Npgsql EF Core Provider Docs](https://www.npgsql.org/efcore/)
- [PostgreSQL Official Docs](https://www.postgresql.org/docs/)
- [EF Core Database Providers](https://docs.microsoft.com/en-us/ef/core/providers/)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)

---

**Version**: 1.0  
**Status**: Complete  
**Last Updated**: April 2026
