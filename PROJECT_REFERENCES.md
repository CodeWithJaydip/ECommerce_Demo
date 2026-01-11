# Project References & Dependency Graph

## Project Reference Matrix

| Project | References |
|---------|-----------|
| **ECommerce.Domain** | None (Base Layer) |
| **ECommerce.Application.Common** | ECommerce.Domain |
| **ECommerce.Application** | ECommerce.Domain<br>ECommerce.Application.Common |
| **ECommerce.Infrastructure.Data** | ECommerce.Domain |
| **ECommerce.Infrastructure.Identity** | ECommerce.Domain<br>ECommerce.Application |
| **ECommerce.Infrastructure** | ECommerce.Domain<br>ECommerce.Application<br>ECommerce.Infrastructure.Data |
| **ECommerce.Api** | ECommerce.Application<br>ECommerce.Application.Common<br>ECommerce.Infrastructure<br>ECommerce.Infrastructure.Data<br>ECommerce.Infrastructure.Identity |

---

## Visual Dependency Flow

```
┌─────────────────────────────────────────┐
│      ECommerce.Api (Presentation)       │  ← API Layer
└────────────┬────────────────────────────┘
             │ References:
             ├──→ ECommerce.Application
             ├──→ ECommerce.Application.Common
             ├──→ ECommerce.Infrastructure
             ├──→ ECommerce.Infrastructure.Data
             └──→ ECommerce.Infrastructure.Identity
             │
             ▼
┌─────────────────────────────────────────┐
│     ECommerce.Infrastructure            │  ← Infrastructure Layer
│     ECommerce.Infrastructure.Data       │
│     ECommerce.Infrastructure.Identity   │
└────────────┬────────────────────────────┘
             │ References:
             ├──→ ECommerce.Application (interfaces)
             └──→ ECommerce.Domain
             │
             ▼
┌─────────────────────────────────────────┐
│     ECommerce.Application               │  ← Application Layer
│     ECommerce.Application.Common        │
└────────────┬────────────────────────────┘
             │ References:
             └──→ ECommerce.Domain
             │
             ▼
┌─────────────────────────────────────────┐
│     ECommerce.Domain                    │  ← Domain Layer (Core)
└─────────────────────────────────────────┘
             │
             └──→ No Dependencies (Pure Domain)
```

---

## Standard NuGet Package References

### ECommerce.Domain
```
- (Minimal dependencies - pure domain)
- System.ComponentModel.Annotations (for Data Annotations if needed)
```

### ECommerce.Application
```
- FluentValidation
- FluentValidation.DependencyInjectionExtensions
```

### ECommerce.Application.Common
```
- (Minimal - only Domain reference)
```

### ECommerce.Infrastructure.Data
```
- Microsoft.EntityFrameworkCore (v8.0.0)
- Microsoft.EntityFrameworkCore.SqlServer (v8.0.0)
  OR
- Npgsql.EntityFrameworkCore.PostgreSQL (v8.0.0)
- Microsoft.EntityFrameworkCore.Tools (v8.0.0)
- Microsoft.EntityFrameworkCore.Design (v8.0.0)
```

### ECommerce.Infrastructure
```
- Microsoft.EntityFrameworkCore
- Microsoft.Extensions.Caching.Abstractions
- Microsoft.Extensions.Caching.Memory
  OR
- StackExchange.Redis (for Redis caching)
- Microsoft.Extensions.Configuration.Abstractions
- Microsoft.Extensions.Logging.Abstractions
```

### ECommerce.Infrastructure.Identity
```
- Microsoft.AspNetCore.Authentication.JwtBearer (v8.0.0)
- System.IdentityModel.Tokens.Jwt
- BCrypt.Net-Next (for password hashing)
- Microsoft.Extensions.Configuration.Abstractions
- Microsoft.Extensions.Options
```

### ECommerce.Api
```
- Microsoft.AspNetCore.OpenApi (v8.0.0)
- Swashbuckle.AspNetCore
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.Design
- Serilog.AspNetCore (for logging, optional)
- Microsoft.AspNetCore.Cors
```

---

## Command to Add Project References (PowerShell/CLI)

### ECommerce.Application.Common
```bash
dotnet add reference ../ECommerce.Domain/ECommerce.Domain.csproj
```

### ECommerce.Application
```bash
dotnet add reference ../ECommerce.Domain/ECommerce.Domain.csproj
dotnet add reference ../ECommerce.Application.Common/ECommerce.Application.Common.csproj
```

### ECommerce.Infrastructure.Data
```bash
dotnet add reference ../ECommerce.Domain/ECommerce.Domain.csproj
```

### ECommerce.Infrastructure.Identity
```bash
dotnet add reference ../ECommerce.Domain/ECommerce.Domain.csproj
dotnet add reference ../ECommerce.Application/ECommerce.Application.csproj
```

### ECommerce.Infrastructure
```bash
dotnet add reference ../ECommerce.Domain/ECommerce.Domain.csproj
dotnet add reference ../ECommerce.Application/ECommerce.Application.csproj
dotnet add reference ../ECommerce.Infrastructure.Data/ECommerce.Infrastructure.Data.csproj
```

### ECommerce.Api
```bash
dotnet add reference ../ECommerce.Application/ECommerce.Application.csproj
dotnet add reference ../ECommerce.Application.Common/ECommerce.Application.Common.csproj
dotnet add reference ../ECommerce.Infrastructure/ECommerce.Infrastructure.csproj
dotnet add reference ../ECommerce.Infrastructure.Data/ECommerce.Infrastructure.Data.csproj
dotnet add reference ../ECommerce.Infrastructure.Identity/ECommerce.Infrastructure.Identity.csproj
```

---

## Solution File Structure

When creating the solution, add projects in this order:

```
Solution: ECommerce_Demo.sln

├── src/
│   ├── Presentation/
│   │   └── ECommerce.Api
│   ├── Application/
│   │   ├── ECommerce.Application
│   │   └── ECommerce.Application.Common
│   ├── Domain/
│   │   └── ECommerce.Domain
│   └── Infrastructure/
│       ├── ECommerce.Infrastructure
│       ├── ECommerce.Infrastructure.Data
│       └── ECommerce.Infrastructure.Identity
└── tests/
    ├── ECommerce.Domain.Tests
    ├── ECommerce.Application.Tests
    └── ECommerce.Infrastructure.Tests
```

---

## Key Principles

1. **Dependency Rule**: Dependencies point inward
   - Domain has no dependencies
   - Application depends only on Domain
   - Infrastructure depends on Domain and Application
   - API depends on all layers

2. **Interface Placement**:
   - Domain interfaces → ECommerce.Domain
   - Application interfaces → ECommerce.Application
   - Implementations → Infrastructure layers

3. **Separation**:
   - Data access (DbContext, Configurations) → Infrastructure.Data
   - Business logic implementations → Application
   - Infrastructure services → Infrastructure
   - Identity/Auth → Infrastructure.Identity

4. **No Circular Dependencies**:
   - Always validate that references follow the dependency flow
   - Use dependency injection to invert dependencies where needed
