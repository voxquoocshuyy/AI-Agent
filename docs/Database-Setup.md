# Database Setup

## Overview
The project uses PostgreSQL as the database with Entity Framework Core for data access. The database is configured with health checks and proper connection management.

## Configuration

### Connection String
The database connection string is configured in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=aiagent;Username=postgres;Password=postgres"
  }
}
```

### Entity Framework Core
- Using Npgsql.EntityFrameworkCore.PostgreSQL (version 8.0.2)
- Entity configurations are in the `Configurations` folder
- Migrations are managed through EF Core tools

### Health Checks
Two health checks are implemented for the database:
1. Custom `DatabaseHealthCheck` that verifies:
   - Database connectivity
   - Query execution capability
2. Built-in Npgsql health check that verifies:
   - Connection pool health
   - Database availability

## Database Schema

### Document Entity
- `Id`: Guid (Primary Key)
- `Name`: string (Required, max length 255)
- `Content`: string (Required)
- `FileType`: string (Required, max length 50)
- `CreatedAt`: DateTime (Required)
- `LastModifiedAt`: DateTime (Nullable)
- `IsProcessed`: bool (Required)
- `ProcessingError`: string (Nullable, max length 1000)
- `SearchVector`: tsvector (Shadow property for full-text search)

## Setup Instructions

1. Install PostgreSQL (if not already installed)
2. Create the database:
   ```sql
   CREATE DATABASE aiagent;
   ```
3. Run the build script to create and apply migrations:
   ```powershell
   ./build.ps1
   ```

## Health Check Endpoints
- `/health/ready`: Checks if the database is ready to accept connections
- `/health/live`: Basic liveness check

## Future Considerations
- Add database backup strategy
- Implement database versioning
- Add database performance monitoring
- Consider implementing database sharding for large-scale deployments 