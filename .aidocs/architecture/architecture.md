# Picus Picks Backend Architecture

## Overview
Picus Picks is an NFL game prediction platform where users can test their football knowledge by picking winners for upcoming NFL games. The system consists of three main components:

1. A React Frontend (client application)
2. This .NET Core Backend API
3. External NFL Data API Integration

The application flow works as follows:

1. The backend regularly fetches NFL game data from an external API and stores it locally
2. Users log in through Auth0 authentication
3. Users can join leagues and compete with other players
4. Users make predictions ("picks") for upcoming games before the pick deadline
5. Once games are completed, picks are scored and points are awarded
6. Leaderboards track user performance within their leagues

## Project Structure

```
src/
└── Picus.Api/
    ├── Configuration/     # Application configuration classes
    ├── Controllers/       # API endpoints and route handlers
    ├── Data/             # Database context and data access
    ├── Mappers/          # Object mapping configurations
    ├── Middleware/       # Custom HTTP pipeline components
    ├── Migrations/       # EF Core database migrations
    ├── Models/           # Domain and DTO models
    ├── Services/         # Business logic implementation
    └── Program.cs        # Application entry point and setup
```

## Architectural Layers

### 1. Presentation Layer (Controllers/)
- Handles HTTP requests and responses
- Implements RESTful API endpoints
- Manages input validation and model binding
- Returns appropriate HTTP status codes and responses

### 2. Business Layer (Services/)
- Implements core business logic
- Handles data processing and transformations
- Manages business rules and validations
- Coordinates between different components

### 3. Data Access Layer (Data/)
- Manages database operations using Entity Framework Core
- Implements repository pattern for data access
- Handles database migrations and schema updates
- Provides data persistence operations

### 4. Domain Layer (Models/)
- Contains domain entities and value objects
- Defines data transfer objects (DTOs)
- Implements domain-specific validation rules
- Represents the core business concepts

## Domain Model

### Core Entities

#### Games
- Represents an NFL game with home and away teams
- Tracks game time, pick deadline, scores, and completion status
- Supports both regular season and playoff games
- Links to the external API via ExternalGameId

#### Users
- Authenticated via Auth0
- Can belong to a league
- Have customizable display names and timezones
- Can have different roles (Player, Admin)
- Track their picks and performance

#### Picks
- Represents a user's prediction for a specific game
- Records submission time and selected team
- Tracks correctness and points awarded
- Includes optional notes for the pick

#### Leagues
- Groups users together for competition
- Enables league-specific leaderboards and statistics
- Allows for different competition formats

#### Teams
- Represents NFL teams
- Stores team details and statistics
- Used in game and pick relationships

## Key Components

### Configuration Management
- Uses standard ASP.NET Core configuration system
- Environment-specific settings in appsettings.json files
- Strongly-typed configuration classes
- Secure secrets management

### Middleware Pipeline
- Custom middleware components for cross-cutting concerns
- Request/response processing
- Error handling and logging
- Authentication and authorization

### Data Mapping
- Object-to-object mapping configurations
- DTO to domain model transformations
- Clean separation between API and domain models

## Technical Stack

### Framework & Runtime
- ASP.NET Core 6.0+
- Entity Framework Core for ORM
- C# 10+ language features

### Database
- Uses Entity Framework Core migrations
- Supports relational database (specific type determined by configuration)

### Authentication & Security
- JWT token-based authentication
- HTTPS enforcement
- CORS policy implementation
- Secure password handling

### API Documentation
- Swagger/OpenAPI integration
- XML documentation
- API versioning support

## Design Patterns & Principles

### Implemented Patterns
- Repository Pattern for data access
- Dependency Injection for loose coupling
- Factory Pattern where appropriate
- Builder Pattern for complex object construction

### SOLID Principles
- Single Responsibility Principle: Each class has one primary responsibility
- Open/Closed Principle: Open for extension, closed for modification
- Liskov Substitution Principle: Proper inheritance hierarchies
- Interface Segregation: Focused interfaces
- Dependency Inversion: High-level modules depend on abstractions

## Cross-Cutting Concerns

### Logging
- Structured logging implementation
- Error and diagnostic information capture
- Performance monitoring capabilities

### Error Handling
- Global exception handling middleware
- Consistent error response format
- Detailed development errors
- Sanitized production errors

### Validation
- Model validation using Data Annotations
- Business rule validation in service layer
- Input sanitization and security checks

## Testing Strategy

### Unit Tests
- Business logic testing
- Service layer testing
- Controller testing
- Mocking of dependencies

### Integration Tests
- API endpoint testing
- Database integration testing
- Authentication flow testing

## Performance Considerations

### Optimization Techniques
- Async/await for I/O operations
- Efficient LINQ queries
- Proper indexing strategy
- Caching where appropriate

### Scalability
- Stateless design
- Resource optimization
- Connection pooling
- Query optimization

## Security Measures

### Implementation
- Input validation
- SQL injection prevention
- XSS protection
- CSRF protection
- Rate limiting
- Secure headers

### Data Protection
- Encryption at rest
- Secure communication
- Password hashing
- Sensitive data handling

## Deployment Considerations

### Requirements
- .NET 6.0+ runtime
- Database server
- HTTPS certificate
- Appropriate permissions

### Configuration
- Environment-specific settings
- Connection strings
- API keys and secrets
- Logging levels

## Maintenance & Monitoring

### Health Checks
- Database connectivity
- External service dependencies
- System resources

### Monitoring
- Performance metrics
- Error rates
- API usage statistics
- Resource utilization

## Future Considerations

### Potential Improvements
- Caching implementation
- Background job processing
- Event sourcing
- Message queuing
- Microservices architecture (if needed)
