# API Documentation

## Overview
The AI Agent API is a RESTful service built with ASP.NET Core 9, providing endpoints for various AI-related operations. The API uses OpenAPI/Swagger for documentation and supports versioning.

## API Versioning
The API uses URL-based versioning with the following format:
- Current version: v1
- Version format: `v{major}.{minor}`
- Example: `/api/v1/weatherforecast`

## Authentication
The API uses JWT (JSON Web Token) authentication. To access protected endpoints:

1. Include the JWT token in the Authorization header:
```
Authorization: Bearer {your-token}
```

2. The token should be obtained from the authentication endpoint (to be implemented).

## Available Endpoints

### Weather Forecast
```http
GET /api/v1/weatherforecast
```

Returns a 5-day weather forecast.

#### Authentication
- Required: Yes
- Policy: RequireUserRole

#### Response
```json
[
  {
    "date": "2024-03-20",
    "temperatureC": 25,
    "temperatureF": 77,
    "summary": "Sunny"
  }
]
```

### Health Check
```http
GET /health
```

Returns the health status of the API and its dependencies.

#### Authentication
- Required: No

#### Response
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "elasticsearch",
      "status": "Healthy",
      "description": "Elasticsearch is responding",
      "duration": "00:00:00.1234567"
    },
    {
      "name": "database",
      "status": "Healthy",
      "description": "Database is responding",
      "duration": "00:00:00.0123456"
    }
  ]
}
```

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "The request was invalid",
  "traceId": "00-1234567890abcdef1234567890abcdef-1234567890abcdef-00"
}
```

### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication failed",
  "traceId": "00-1234567890abcdef1234567890abcdef-1234567890abcdef-00"
}
```

### 403 Forbidden
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You don't have permission to access this resource",
  "traceId": "00-1234567890abcdef1234567890abcdef-1234567890abcdef-00"
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "The requested resource was not found",
  "traceId": "00-1234567890abcdef1234567890abcdef-1234567890abcdef-00"
}
```

### 429 Too Many Requests
```json
{
  "type": "https://tools.ietf.org/html/rfc6585#section-4",
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Rate limit exceeded. Please try again later.",
  "traceId": "00-1234567890abcdef1234567890abcdef-1234567890abcdef-00"
}
```

### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An error occurred while processing your request",
  "traceId": "00-1234567890abcdef1234567890abcdef-1234567890abcdef-00"
}
```

## Rate Limiting
The API implements rate limiting to prevent abuse:
- Maximum requests: 100 per window
- Window size: 1 second
- Refill rate: 10 requests per second

## Security Headers
The API includes the following security headers:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: strict-origin-when-cross-origin`
- `Content-Security-Policy`
- `Permissions-Policy`

## CORS Configuration
The API supports CORS with the following configuration:
- Allowed origins: Configurable through `AllowedOrigins` in appsettings.json
- Allowed methods: GET, POST, PUT, DELETE, OPTIONS
- Allowed headers: All
- Credentials: Allowed

## Swagger UI
The API documentation is available through Swagger UI:
- URL: `/swagger`
- Authentication: JWT Bearer token
- Features:
  - Interactive API documentation
  - Request/response examples
  - Authentication support
  - API versioning support

## Future Improvements
1. Add more endpoints for AI operations
2. Implement OAuth2 support
3. Add request/response validation
4. Implement API key authentication
5. Add more detailed error responses
6. Implement request logging
7. Add API usage analytics
8. Implement request caching

## Development
To run the API locally:
1. Clone the repository
2. Install dependencies
3. Configure appsettings.json
4. Run the application
5. Access Swagger UI at `/swagger`

## Testing
The API includes unit tests and integration tests:
- Unit tests: `tests/AI.Agent.UnitTests`
- Integration tests: `tests/AI.Agent.IntegrationTests`
- Test coverage: To be implemented 