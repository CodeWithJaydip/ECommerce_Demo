# ECommerce Demo - Project Guide

## Project Overview

Multi-vendor e-commerce platform with a .NET 8 Web API backend and React 18 (Vite) frontend. Supports user authentication, role-based authorization (Super Admin, Seller, Buyer), product catalog management, category management, and file uploads.

## Tech Stack

| Layer | Technologies |
|-------|-------------|
| **Backend** | .NET 8, ASP.NET Core Web API, Entity Framework Core 8, SQL Server (LocalDB) |
| **Auth** | JWT (HS256), BCrypt.Net-Next for password hashing |
| **Logging** | Serilog (Console + File sinks) |
| **Frontend** | React 18, Vite, React Router 6, Redux Toolkit |
| **UI** | Tailwind CSS 3, shadcn/ui (Radix UI primitives), Lucide React icons |
| **Forms** | React Hook Form + Zod validation |
| **API Docs** | Swagger/Swashbuckle |

## Architecture

Clean Architecture with inward dependency flow:

```
Domain (no deps) ← Application ← Infrastructure ← API
```

- **Domain** — Entities, enums, constants. Zero external dependencies.
- **Application** — DTOs, service/repository interfaces, common models. Depends on Domain only.
- **Infrastructure** — EF Core DbContext, repository/service implementations, migrations. Depends on Domain + Application.
- **API** — Controllers, middleware, DI configuration, static files. Depends on all layers.

## Solution Structure

```
ECommerce_Demo.sln
src/
├── ECommerce.Domain/
│   ├── Constants/          # Validation messages (ProductConstants, CategoryConstants)
│   ├── Entities/           # BaseEntity, User, Role, UserRole, Category, Product
│   └── Enums/              # UserRole enum (Admin, Customer, Vendor)
├── ECommerce.Application/
│   ├── Features/
│   │   ├── Auth/           # DTOs (LoginRequest, RegisterRequest, AuthResponse) + Interfaces
│   │   ├── Category/       # DTOs (Create/UpdateCategoryRequest, CategoryResponse) + Interfaces
│   │   ├── Product/        # DTOs (Create/UpdateProductRequest, ProductResponse) + Interfaces
│   │   └── User/           # DTOs (UpdateUserRequest, UserResponse, UserFilter) + Interfaces
│   └── Common/Models/      # ApiResponse<T>, PagedResult<T>, PagedMetadata
├── ECommerce.Infrastructure/
│   ├── Data/               # ApplicationDbContext, EF configurations, migrations, UnitOfWork
│   ├── Repositories/       # UserRepository, RoleRepository, CategoryRepository, ProductRepository
│   └── Services/           # AuthService, JwtTokenService, PasswordHasher, CategoryService, ProductService, UserService, FileUploadService
└── ECommerce.Api/
    ├── Controllers/        # AuthController, UserController, CategoryController, ProductController
    ├── Middleware/          # GlobalExceptionHandlerMiddleware, ModelValidationFilter
    ├── wwwroot/uploads/    # categories/, products/ (uploaded images)
    ├── Program.cs          # DI, JWT, CORS, Swagger, middleware pipeline
    └── appsettings.json    # Connection string, JWT config, Serilog, CORS origins

client/                     # React frontend (Vite)
├── src/
│   ├── components/         # admin/, auth/, catalog/, common/, dashboard/, landing/, products/, ui/
│   ├── hooks/              # redux.js (useAppDispatch, useAppSelector)
│   ├── lib/                # utils.js (cn() for Tailwind class merging)
│   ├── services/api/       # apiClient.js, authApi.js, categoryApi.js, productApi.js, userApi.js
│   ├── store/slices/       # authSlice.js (Redux Toolkit)
│   ├── App.jsx             # Route definitions
│   └── main.jsx            # Entry point
```

## Backend Coding Standards

### Dependency Injection
- Use **primary constructor** pattern for DI:
  ```csharp
  public class CategoryService(ICategoryRepository repo, IUnitOfWork uow) : ICategoryService
  ```
- All services/repos registered as **Scoped** in `Program.cs`

### Repository Pattern
- Repositories handle data access only — **no `SaveChangesAsync()`** in repositories
- All reads use **`AsNoTracking()`** for performance
- Tracked entities fetched via separate `GetByIdForUpdateAsync()` methods
- **UnitOfWork** manages `SaveChangesAsync()`, transactions

### DTOs
- Organized under `Features/{Feature}/DTOs/Requests/` and `Features/{Feature}/DTOs/Responses/`
- Request DTOs: `Create{Entity}Request`, `Update{Entity}Request`
- Response DTOs: `{Entity}Response`
- **Manual mapping** via private static `MapToResponse()` methods in services

### API Responses
- All endpoints return `ApiResponse<T>` wrapper:
  ```json
  { "success": true, "message": "...", "data": {...}, "errors": [] }
  ```
- Paginated endpoints return `ApiResponse<PagedResult<T>>` with `PagedMetadata`

### General Conventions
- **File-scoped namespaces** (`namespace X;`)
- **CancellationToken** on all async method signatures
- **Soft delete** via `IsActive` flag (never hard delete)
- **BaseEntity** provides `Id`, `CreatedAt`, `UpdatedAt`, `IsActive`
- Validation constants in `Domain/Constants/` (e.g., `ProductConstants.ProductNameExists`)
- Exception-based error flow caught by `GlobalExceptionHandlerMiddleware`
- `[Authorize(Roles = "Super Admin")]` for admin endpoints
- `[AllowAnonymous]` for public endpoints
- `[FromForm]` parameters with `IFormFile` for file uploads
- Route convention: `[Route("api/[controller]")]`

## Frontend Coding Standards

### State Management
- **Redux Toolkit** for global auth state (`authSlice.js`)
- **Local component state** (`useState`) for UI/data state
- Custom hooks: `useAppDispatch()`, `useAppSelector()`

### API Calls
- Base client: `apiClient.js` with `get()`, `post()`, `put()`, `del()` helpers
- Feature-specific files: `productApi.js`, `categoryApi.js`, `userApi.js`, `authApi.js`
- `FormData` for file uploads via raw `fetch()`
- API proxy configured in `vite.config.js` (`/api` → `http://localhost:5176`)

### UI Patterns
- **shadcn/ui** components in `components/ui/` (Button, Card, Input, Select, Table, AlertDialog, etc.)
- **Tailwind CSS** with responsive prefixes (`sm:`, `md:`, `lg:`, `xl:`)
- Class merging with `cn()` utility from `lib/utils.js`
- **Lucide React** for icons
- Responsive grid: `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4`

### Forms & Validation
- **React Hook Form** + **Zod** schema validation
- `@hookform/resolvers` for Zod integration

### Component Organization
- Pages in feature folders: `components/admin/`, `components/products/`, `components/catalog/`
- Shared components: `components/common/` (Header, ProtectedRoute)
- UI primitives: `components/ui/`

## Key Commands

```bash
# Backend
dotnet build                                           # Build solution
dotnet run --project src/ECommerce.Api                 # Run API (port 5176)
dotnet ef migrations add <Name> -p src/ECommerce.Infrastructure -s src/ECommerce.Api  # Add migration
dotnet ef database update -p src/ECommerce.Infrastructure -s src/ECommerce.Api        # Apply migrations

# Frontend
cd client && npm install                               # Install dependencies
cd client && npm run dev                               # Dev server (port 3000)
cd client && npm run build                             # Production build
```

## Database

- **Provider**: SQL Server LocalDB
- **Connection**: `Server=(localdb)\MSSQLLocalDB;Database=ECommerce_Demo;Integrated Security=true`
- **Tables**: Users, Roles, UserRoles, Categories, Products
- **Seed Data**: 3 roles (Super Admin, Seller, Buyer)

### Migrations (chronological)
1. `InitialCreate` — Users, Roles, UserRoles tables
2. `AddCategory` — Categories table
3. `AddRoleSeedData` — Seed Admin/Seller/Buyer roles
4. `AddImagePathToCategory` — ImagePath column on Categories
5. `AddProductTable` — Products table with Category/Seller FK

## API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/register` | None | Register new user |
| POST | `/api/auth/login` | None | Login, get JWT token |
| GET | `/api/category` | Yes | List all categories |
| GET | `/api/category/{id}` | Yes | Get category by ID |
| POST | `/api/category` | Super Admin | Create category |
| PUT | `/api/category/{id}` | Super Admin | Update category |
| DELETE | `/api/category/{id}` | Super Admin | Soft delete category |
| GET | `/api/product` | None | List all products |
| GET | `/api/product/{id}` | None | Get product by ID |
| GET | `/api/product/catalog` | None | Search/filter/sort/paginate products |
| POST | `/api/product` | Super Admin, Seller | Create product |
| PUT | `/api/product/{id}` | Super Admin, Seller | Update product |
| DELETE | `/api/product/{id}` | Super Admin, Seller | Delete product |
| GET | `/api/user` | Super Admin | List users (paginated) |
| GET | `/api/user/{id}` | Super Admin | Get user by ID |
| PUT | `/api/user/{id}` | Super Admin | Update user |
| PUT | `/api/user/{id}/roles` | Super Admin | Assign roles |
| DELETE | `/api/user/{id}` | Super Admin | Deactivate user |

## Current Progress

### Completed
- Clean Architecture project structure
- User authentication (JWT) with login/register
- Password hashing (BCrypt) with account lockout
- Role-based authorization (Super Admin, Seller, Buyer)
- Category CRUD with image upload
- Product CRUD with image upload and seller ownership
- User management (admin) with pagination, filtering, sorting
- Global exception handling middleware
- Structured logging (Serilog)
- Swagger API documentation
- CORS configuration for React frontend
- React frontend with landing page, auth, dashboard, product/category management
- Catalog page with search, filters, sorting, and pagination

### Not Yet Implemented
- Shopping cart / basket
- Order management / checkout
- Payment integration
- User profile page
- Reviews / ratings
- Email notifications
- Test projects (unit + integration)
- CI/CD pipeline
- Docker containerization
