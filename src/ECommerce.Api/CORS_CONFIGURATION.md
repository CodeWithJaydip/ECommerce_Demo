# CORS Configuration

This document explains the CORS (Cross-Origin Resource Sharing) configuration for the ECommerce API.

## Overview

CORS is configured to allow the React frontend application to make requests to the API from different origins (ports/domains).

## Configuration

### Policy: "ReactClient"

The main CORS policy allows:
- **Origins**: Configured in `appsettings.json` and `appsettings.Development.json`
  - Development: `http://localhost:3000`, `http://localhost:5173`, `https://localhost:3000`
  - Production: `http://localhost:3000`, `https://localhost:3000`
- **Methods**: All HTTP methods (GET, POST, PUT, DELETE, etc.)
- **Headers**: All headers (including Authorization for JWT tokens)
- **Credentials**: Enabled (required for JWT token authentication)

### Configuration Files

#### appsettings.json (Production)
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://localhost:3000"
    ]
  }
}
```

#### appsettings.Development.json (Development)
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "https://localhost:3000"
    ]
  }
}
```

## Adding New Origins

To allow additional origins (e.g., production frontend URL):

1. Add the origin to the `Cors:AllowedOrigins` array in the appropriate `appsettings.json` file
2. Restart the API application

Example:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://localhost:3000",
      "https://your-production-domain.com"
    ]
  }
}
```

## Middleware Order

The CORS middleware is placed early in the pipeline (before authentication) to handle preflight OPTIONS requests:

```
1. Swagger (Development only)
2. CORS ← Handles preflight requests
3. HTTPS Redirection
4. Exception Handler
5. Authentication
6. Authorization
7. Controllers
```

## Important Notes

1. **AllowCredentials()**: When using `AllowCredentials()`, you **cannot** use `AllowAnyOrigin()`. You must specify exact origins using `WithOrigins()`.

2. **Preflight Requests**: Browsers send OPTIONS requests (preflight) for certain cross-origin requests. The CORS middleware handles these automatically.

3. **JWT Tokens**: The `AllowCredentials()` setting is required to send JWT tokens in the Authorization header from the React frontend.

4. **Security**: In production, always specify exact origins. Never use `AllowAnyOrigin()` with `AllowCredentials()`.

## Testing CORS

To verify CORS is working:

1. Start the API server
2. Start the React frontend on `http://localhost:3000`
3. Open browser DevTools → Network tab
4. Make a request from the React app
5. Check the response headers for:
   - `Access-Control-Allow-Origin: http://localhost:3000`
   - `Access-Control-Allow-Credentials: true`

## Troubleshooting

### CORS Errors in Browser

If you see CORS errors:
1. Verify the frontend URL matches one in `AllowedOrigins`
2. Check that `AllowCredentials()` is set (required for Authorization header)
3. Ensure CORS middleware is before Authentication middleware
4. Check browser console for specific error messages

### Common Errors

- **"No 'Access-Control-Allow-Origin' header"**: Origin not in allowed list
- **"Credentials flag is true, but 'Access-Control-Allow-Credentials' is not 'true'"**: Missing `AllowCredentials()` call
- **"Preflight request doesn't pass"**: CORS middleware not handling OPTIONS requests properly
