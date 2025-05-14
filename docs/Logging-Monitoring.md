# Logging and Monitoring

## Overview
The application uses Serilog for structured logging with multiple sinks:
- Console output for development
- Elasticsearch for centralized log storage
- File-based logging for backup
- Performance monitoring middleware
- Health checks for system components

## Configuration

### Serilog Setup
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "Elasticsearch": {
    "Url": "http://localhost:9200"
  },
  "Kibana": {
    "Url": "http://localhost:5601"
  }
}
```

### Log Enrichment
The following enrichers are configured:
- Machine Name
- Environment Name
- Thread ID
- Process ID
- Log Context

### Sinks
1. Console
   - Structured JSON output
   - Human-readable format for development

2. Elasticsearch
   - Index format: aiagent-{yyyy-MM}
   - Auto-register template
   - 2 shards, 1 replica
   - JSON formatting

3. File
   - Daily rolling logs
   - Path: logs/aiagent-.log
   - Structured format with timestamps

## Performance Monitoring

### Middleware
The `PerformanceMonitoringMiddleware` tracks:
- Request duration
- HTTP method and path
- Status codes
- Slow request detection (>1s)

### Metrics
- Request duration
- Status code distribution
- Error rates
- Slow request patterns

## Health Checks

### Endpoints
- `/health` - Overall system health
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

### Components Monitored
1. Elasticsearch
   - Cluster health
   - Connection status
   - Response time

2. Database
   - Connection status
   - Query performance
   - Transaction health

## Kibana Dashboards

### Available Dashboards
1. Application Overview
   - Request rates
   - Error rates
   - Response times
   - Status code distribution

2. Performance Analysis
   - Slow request patterns
   - Resource utilization
   - Bottleneck identification

3. Error Tracking
   - Error distribution
   - Stack traces
   - Error patterns

## Usage

### Logging
```csharp
// Basic logging
_logger.LogInformation("Processing request {RequestId}", requestId);

// Structured logging
_logger.LogInformation(
    "User {UserId} performed action {Action} on {Resource}",
    userId,
    action,
    resource);

// Error logging
try
{
    // Operation
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to process request {RequestId}", requestId);
}
```

### Health Checks
```bash
# Check overall health
curl http://localhost:5000/health

# Check specific component
curl http://localhost:5000/health/elasticsearch
```

## Best Practices
1. Use structured logging
2. Include correlation IDs
3. Log appropriate context
4. Monitor performance metrics
5. Set up alerts for critical issues
6. Regular log rotation
7. Secure log storage
8. Regular health check monitoring

## Future Improvements
1. Add APM integration
2. Implement log aggregation
3. Add custom metrics
4. Enhance alerting system
5. Add trace correlation
6. Implement log sampling
7. Add log retention policies
8. Enhance dashboard visualizations 