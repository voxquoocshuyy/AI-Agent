# Docker Setup

## Overview
The project uses Docker for containerization and Docker Compose for orchestration. The setup includes:
- API service (.NET 9)
- PostgreSQL database
- Elasticsearch for logging
- Kibana for log visualization

## Services

### API Service
- Built from `src/AI.Agent.API/Dockerfile`
- Multi-stage build for optimized image size
- Exposes ports 80 (HTTP) and 443 (HTTPS)
- Environment variables configured for development

### PostgreSQL
- Uses PostgreSQL 16 Alpine for smaller image size
- Persistent volume for data storage
- Exposed on port 5432
- Default credentials:
  - Database: aiagent
  - Username: postgres
  - Password: postgres

### Elasticsearch
- Version 8.12.1
- Single-node setup for development
- Security disabled for local development
- Memory limit set to 512MB
- Exposed on port 9200

### Kibana
- Version 8.12.1
- Connected to Elasticsearch
- Exposed on port 5601

## Configuration

### Environment Variables
```yaml
# API Service
ASPNETCORE_ENVIRONMENT: Development
ConnectionStrings__DefaultConnection: Host=postgres;Database=aiagent;Username=postgres;Password=postgres
Elasticsearch__Url: http://elasticsearch:9200
Kibana__Url: http://kibana:5601

# PostgreSQL
POSTGRES_DB: aiagent
POSTGRES_USER: postgres
POSTGRES_PASSWORD: postgres

# Elasticsearch
discovery.type: single-node
xpack.security.enabled: false
ES_JAVA_OPTS: -Xms512m -Xmx512m

# Kibana
ELASTICSEARCH_HOSTS: http://elasticsearch:9200
```

### Volumes
- `postgres_data`: PostgreSQL data persistence
- `elasticsearch_data`: Elasticsearch data persistence

## Usage

### Starting the Services
```bash
docker-compose up -d
```

### Stopping the Services
```bash
docker-compose down
```

### Viewing Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
```

### Accessing Services
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- Kibana: http://localhost:5601
- PostgreSQL: localhost:5432

## Development Workflow

### Building Images
```bash
docker-compose build
```

### Rebuilding a Specific Service
```bash
docker-compose build api
```

### Running Database Migrations
```bash
docker-compose exec api dotnet ef database update
```

## Security Considerations
- Default credentials are for development only
- Enable security features in production
- Use secrets management for sensitive data
- Configure proper network security
- Enable HTTPS in production

## Future Improvements
- Add health check endpoints
- Implement proper secrets management
- Add production configuration
- Set up CI/CD pipeline
- Add monitoring and alerting
- Configure backup strategies 