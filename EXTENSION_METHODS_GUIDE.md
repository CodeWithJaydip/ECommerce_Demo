# Extension Methods Strategy

## Overview

Extension methods are used throughout the application for configuration, mappings, and utilities instead of AutoMapper and configuration classes.

## Extension Method Locations

### 1. ECommerce.Api/Extensions/
**Purpose**: API layer configuration extensions

**Files**:
- `ServiceCollectionExtensions.cs`: Configure dependency injection services
  - `AddApplicationServices()`: Register application services
  - `AddInfrastructureServices()`: Register infrastructure services
  - `AddIdentityServices()`: Register identity/JWT services
  - `AddDbContext()`: Configure database context

- `ApplicationBuilderExtensions.cs`: Configure middleware pipeline
  - `UseApplicationMiddleware()`: Configure custom middleware
  - `UseExceptionHandling()`: Configure exception handling
  - `UseSwaggerConfiguration()`: Configure Swagger/OpenAPI

- `WebApplicationExtensions.cs`: Configure web application settings
  - `ConfigureCors()`: Configure CORS for React frontend
  - `ConfigureSwagger()`: Configure Swagger UI
  - `ConfigureEndpoints()`: Configure API endpoints

### 2. ECommerce.Application/Features/{Feature}/Mappings/
**Purpose**: Feature-specific mapping extensions (Domain ↔ DTO)

**Files** (Example: Auth feature):
- `UserMappingExtensions.cs`
  - `ToDto(this User user)`: Map User entity to UserDto
  - `ToEntity(this CreateUserDto dto)`: Map CreateUserDto to User entity
  - `ToDtoList(this IEnumerable<User> users)`: Map collection to DTO list
  - `UpdateEntity(this User user, UpdateUserDto dto)`: Update entity from DTO

**Pattern**: Each feature has its own mapping extension class

### 3. ECommerce.Application.Common/Extensions/
**Purpose**: Common/Shared extension methods

**Files**:
- `MappingExtensions.cs`: Base mapping utilities
- `StringExtensions.cs`: String utilities
- `DateTimeExtensions.cs`: Date/time utilities
- `EnumExtensions.cs`: Enum utilities
- `ValidationExtensions.cs`: Validation helpers

### 4. ECommerce.Infrastructure/Extensions/
**Purpose**: Infrastructure-specific extensions

**Files**:
- `ServiceCollectionExtensions.cs`: Infrastructure service registration
- `QueryExtensions.cs`: EF Core query extensions
  - `AsNoTrackingQuery()`: Apply AsNoTracking globally
  - `IncludeNavigation()`: Include navigation properties
- `RepositoryExtensions.cs`: Repository helper methods

### 5. ECommerce.Infrastructure.Data/Extensions/
**Purpose**: Data access configuration extensions

**Files**:
- `DbContextExtensions.cs`: DbContext configuration
  - `ApplyConfigurations()`: Apply entity configurations
  - `SeedData()`: Seed initial data
- `EntityConfigurationExtensions.cs`: Entity configuration helpers

### 6. ECommerce.Infrastructure.Identity/Extensions/
**Purpose**: Identity service configuration extensions

**Files**:
- `ServiceCollectionExtensions.cs`: Identity service registration
  - `AddJwtAuthentication()`: Configure JWT authentication
  - `AddPasswordHasher()`: Register password hashing service
  - `AddTokenService()`: Register token service
- `JwtConfigurationExtensions.cs`: JWT-specific configuration

## Extension Method Patterns

### Configuration Extension Pattern

```csharp
// Example pattern (not actual code)
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register services
        services.AddScoped<IUserService, UserService>();
        // ... more registrations
        return services;
    }
}
```

### Mapping Extension Pattern

```csharp
// Example pattern (not actual code)
public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            // ... map other properties
        };
    }

    public static User ToEntity(this CreateUserDto dto)
    {
        return new User
        {
            Email = dto.Email,
            Name = dto.Name,
            // ... map other properties
        };
    }

    public static List<UserDto> ToDtoList(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToDto()).ToList();
    }
}
```

### Query Extension Pattern

```csharp
// Example pattern (not actual code)
public static class QueryExtensions
{
    public static IQueryable<T> AsNoTracking<T>(this IQueryable<T> query)
        where T : class
    {
        return query.AsNoTracking();
    }
}
```

## Benefits of Extension Methods

1. **Performance**: No reflection overhead (unlike AutoMapper)
2. **Explicit**: Clear mapping logic, easy to debug
3. **Type Safety**: Compile-time checking
4. **Testability**: Easy to unit test
5. **Maintainability**: Simple to understand and modify
6. **Configuration**: Clean separation of configuration code
7. **Reusability**: Share configuration and mapping logic
8. **IntelliSense**: Better IDE support

## Usage Guidelines

1. **Naming Convention**: 
   - Configuration extensions: `Add*Services()`, `Use*Middleware()`
   - Mapping extensions: `ToDto()`, `ToEntity()`, `ToDtoList()`
   - Utility extensions: Descriptive names (e.g., `ToFormattedString()`)

2. **Organization**: 
   - Group related extensions in the same file
   - One extension class per feature/concern
   - Use namespaces to organize

3. **Testing**:
   - Test mapping extensions with unit tests
   - Test configuration extensions with integration tests

4. **Documentation**:
   - Add XML comments for public extension methods
   - Document any special behavior or requirements

## No AutoMapper

- Manual mapping via extension methods
- Explicit control over mapping logic
- Better performance
- Easier debugging
- Type-safe at compile time

## No CQRS

- Traditional service-based architecture
- Services handle both commands and queries
- No need for MediatR or command/query handlers
- Simpler architecture for the use case
