# Multi-Vendor E-Commerce Backend - Clean Architecture Design

## Solution Structure

### Projects Overview

```
ECommerce_Demo/
├── src/
│   ├── Presentation/
│   │   └── ECommerce.Api/                    (ASP.NET Core 8 Web API)
│   ├── Application/
│   │   ├── ECommerce.Application/            (Use Cases, DTOs, Interfaces)
│   │   └── ECommerce.Application.Common/     (Shared Application Logic)
│   ├── Domain/
│   │   └── ECommerce.Domain/                 (Entities, Value Objects, Domain Events)
│   └── Infrastructure/
│       ├── ECommerce.Infrastructure/         (EF Core, Repositories, External Services)
│       ├── ECommerce.Infrastructure.Data/    (DbContext, Migrations)
│       └── ECommerce.Infrastructure.Identity/ (JWT, Authentication Services)
├── tests/
│   ├── ECommerce.Application.Tests/
│   ├── ECommerce.Domain.Tests/
│   └── ECommerce.Infrastructure.Tests/
└── frontend/                                  (Future React Project)
    └── ecommerce-react/
```

---

## Project Responsibilities

### 1. ECommerce.Domain (Class Library)
**Purpose**: Core business entities and domain logic

**Contains**:
- Entities (Aggregate Roots, Entities)
- Value Objects
- Domain Events
- Domain Interfaces/Abstractions
- Enums (OrderStatus, PaymentStatus, UserRole, etc.)
- Constants (DomainConstants.cs)
- Domain Exceptions

**Dependencies**: None (pure .NET Standard or .NET 8)
- No external packages except for domain-specific value types

**Responsibilities**:
- Define business rules
- Contain entities without persistence concerns
- Define domain contracts (interfaces)
- Pure business logic

---

### 2. ECommerce.Application (Class Library)
**Purpose**: Application business logic and use cases

**Contains**:
- Use Cases / Application Services
- DTOs (Request/Response models)
- Mapping Extension Methods (Domain ↔ DTO)
- Validators (FluentValidation)
- Application Interfaces
- Application Exceptions
- Application Constants

**Dependencies**:
- ECommerce.Domain
- ECommerce.Application.Common
- FluentValidation

**Responsibilities**:
- Orchestrate domain operations
- Transform data (Domain ↔ DTO)
- Validate inputs
- Handle business workflows
- Define application contracts

**Structure**:
```
ECommerce.Application/
├── Features/
│   ├── Auth/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Mappings/
│   ├── Products/
│   ├── Vendors/
│   ├── Orders/
│   └── ...
├── Common/
│   ├── Mappings/
│   └── Exceptions/
└── Interfaces/
```

---

### 3. ECommerce.Application.Common (Class Library)
**Purpose**: Shared application utilities and cross-cutting concerns

**Contains**:
- Common DTOs (PagedResult, ApiResponse, etc.)
- Base classes
- Application constants
- Shared validators
- Extension methods (common utilities)
- Helpers
- Mapping extensions (base mappings)

**Dependencies**:
- ECommerce.Domain

**Responsibilities**:
- Provide reusable application components
- Common patterns and utilities

---

### 4. ECommerce.Infrastructure.Data (Class Library)
**Purpose**: Database context and data access configuration

**Contains**:
- DbContext
- Entity Configurations (IEntityTypeConfiguration)
- Database Migrations
- Seed Data (optional)

**Dependencies**:
- ECommerce.Domain
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer (or PostgreSQL)
- Microsoft.EntityFrameworkCore.Tools

**Responsibilities**:
- Configure EF Core
- Define entity mappings
- Manage migrations
- Database schema definition

---

### 5. ECommerce.Infrastructure (Class Library)
**Purpose**: Infrastructure implementations

**Contains**:
- Repository Implementations
- External Service Clients (Email, Payment Gateway, etc.)
- File Storage Services
- Caching Implementations (Redis, MemoryCache)
- Background Job Services
- Configuration Providers

**Dependencies**:
- ECommerce.Domain
- ECommerce.Application (for interfaces)
- ECommerce.Infrastructure.Data
- Microsoft.EntityFrameworkCore
- Microsoft.Extensions.Caching.*
- Other external packages as needed

**Responsibilities**:
- Implement repository pattern
- Integrate external services
- Handle infrastructure concerns
- Implement caching strategies

---

### 6. ECommerce.Infrastructure.Identity (Class Library)
**Purpose**: JWT authentication and authorization

**Contains**:
- JWT Token Service
- Password Hashing Service
- Authentication Middleware
- Token Validation
- User Claims Management
- Identity Constants

**Dependencies**:
- ECommerce.Domain
- ECommerce.Application (for interfaces)
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt
- BCrypt.Net (or similar)

**Responsibilities**:
- Generate and validate JWT tokens
- Hash/verify passwords
- Manage authentication logic
- Handle token refresh (if implemented)

---

### 7. ECommerce.Api (ASP.NET Core 8 Web API)
**Purpose**: API endpoints and HTTP layer

**Contains**:
- Controllers
- Middleware (Exception Handling, Logging)
- Program.cs configuration
- appsettings.json
- API Constants
- API Filters
- Configuration Extension Methods (ServiceCollection, ApplicationBuilder)

**Dependencies**:
- ECommerce.Application
- ECommerce.Application.Common
- ECommerce.Infrastructure
- ECommerce.Infrastructure.Data
- ECommerce.Infrastructure.Identity
- Microsoft.AspNetCore.*
- Swashbuckle.AspNetCore (Swagger)

**Responsibilities**:
- Expose REST API endpoints
- Handle HTTP requests/responses
- Configure dependency injection (via extension methods)
- Configure middleware pipeline (via extension methods)
- Handle CORS for React frontend
- API versioning (if needed)

**Structure**:
```
ECommerce.Api/
├── Controllers/
│   ├── AuthController.cs
│   ├── ProductsController.cs
│   ├── VendorsController.cs
│   └── ...
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   └── LoggingMiddleware.cs
├── Filters/
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   ├── ApplicationBuilderExtensions.cs
│   └── WebApplicationExtensions.cs
└── Program.cs
```

---

## Dependency Flow (Project References)

### Allowed References:

1. **ECommerce.Domain**
   - References: NONE (Pure domain layer)

2. **ECommerce.Application.Common**
   - References: ECommerce.Domain

3. **ECommerce.Application**
   - References: 
     - ECommerce.Domain
     - ECommerce.Application.Common

4. **ECommerce.Infrastructure.Data**
   - References: 
     - ECommerce.Domain

5. **ECommerce.Infrastructure.Identity**
   - References:
     - ECommerce.Domain
     - ECommerce.Application (for interfaces)

6. **ECommerce.Infrastructure**
   - References:
     - ECommerce.Domain
     - ECommerce.Application (for interfaces)
     - ECommerce.Infrastructure.Data

7. **ECommerce.Api**
   - References:
     - ECommerce.Application
     - ECommerce.Application.Common
     - ECommerce.Infrastructure
     - ECommerce.Infrastructure.Data
     - ECommerce.Infrastructure.Identity

### Dependency Rule:
- **Dependencies flow inward**: Outer layers can reference inner layers, but NOT vice versa
- Domain → Application → Infrastructure → Presentation

---

## EF Core Performance Guidelines

### Configuration in ECommerce.Infrastructure.Data:

1. **AsNoTracking for Queries**
   - Use `AsNoTracking()` for all read operations
   - Configure globally or per query

2. **Single SaveChangesAsync per Request**
   - Use Unit of Work pattern
   - Save once at the end of request processing
   - Implement in Infrastructure layer

3. **No Lazy Loading**
   - Disable lazy loading: `UseLazyLoadingProxies(false)`
   - Use explicit loading with `Include()` or projection
   - Configure navigation properties explicitly

4. **Additional Performance Considerations**:
   - Use compiled queries for frequently executed queries
   - Implement query splitting for complex includes
   - Use pagination for large datasets
   - Indexing strategy in entity configurations

---

## Folder Structure for React Integration

```
ECommerce_Demo/
├── src/                          (Backend projects)
├── tests/                        (Backend tests)
├── frontend/                     (Frontend projects)
│   └── ecommerce-react/          (React application)
│       ├── src/
│       ├── public/
│       ├── package.json
│       └── ...
├── docs/                         (Documentation)
├── scripts/                      (Build/deploy scripts)
└── .gitignore
```

---

## Constants and Enums Strategy

### Enums (ECommerce.Domain):
- `UserRole`: Admin, Vendor, Customer
- `OrderStatus`: Pending, Processing, Shipped, Delivered, Cancelled
- `PaymentStatus`: Pending, Paid, Failed, Refunded
- `ProductStatus`: Active, Inactive, OutOfStock
- `VendorStatus`: Active, Suspended, Pending

### Constants Files:
- **ECommerce.Domain/Constants/DomainConstants.cs**: Domain constants
- **ECommerce.Application/Constants/ApplicationConstants.cs**: Application constants
- **ECommerce.Api/Constants/ApiConstants.cs**: API constants (routes, policies)
- **ECommerce.Infrastructure.Identity/Constants/IdentityConstants.cs**: JWT constants

---

## Key Architectural Patterns

1. **Repository Pattern**: Abstract data access in Infrastructure
2. **Unit of Work**: Single SaveChangesAsync per request
3. **Extension Methods**: Used for configuration, mappings, and utilities
4. **Dependency Injection**: Configure in API layer via extension methods
5. **DTO Pattern**: Separate domain entities from API contracts
6. **Manual Mapping**: Use extension methods for Domain ↔ DTO transformations

---

## Security Considerations

1. **JWT Configuration**:
   - Store secrets in appsettings (or User Secrets/Key Vault)
   - Token expiration policies
   - Refresh token strategy (if implemented)

2. **CORS Configuration**:
   - Configure for React frontend origin
   - Set appropriate headers

3. **API Security**:
   - Rate limiting
   - Input validation
   - SQL injection prevention (EF Core parameterized queries)

---

## Testing Structure

### Test Projects:
1. **ECommerce.Domain.Tests**: Unit tests for domain logic
2. **ECommerce.Application.Tests**: Unit tests for use cases
3. **ECommerce.Infrastructure.Tests**: Integration tests for repositories/services

### Test Dependencies:
- xUnit or NUnit
- Moq or NSubstitute
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory (for testing)

---

## Next Steps (When Implementing)

1. Create solution file (.sln)
2. Create each Class Library project
3. Add project references as per dependency flow
4. Install NuGet packages per project
5. Configure DbContext in Infrastructure.Data
6. Set up JWT authentication in Infrastructure.Identity
7. Configure dependency injection in API project
8. Implement middleware pipeline
9. Create first feature (Auth) as proof of concept

---

## Important Notes

- Each layer is a **separate Class Library project**
- Follow **SOLID principles** in every layer
- Use **constants and enums** instead of magic strings/numbers
- **No hardcoded values** - use configuration
- **Performance-first** EF Core approach
- **Separation of concerns** between layers
- **Testability** - each layer can be tested independently
- **Extension Methods** - Used for configuration, mappings, and utilities
- **No AutoMapper** - Manual mapping via extension methods
- **No CQRS** - Traditional service-based approach