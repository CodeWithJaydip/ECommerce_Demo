# Quick Start Guide

## Running the Project

You need to run both the backend API and React frontend separately:

**Terminal 1 - Backend API:**
```powershell
cd src\ECommerce.Api
dotnet run
```

**Terminal 2 - React Frontend:**
```powershell
cd client
npm run dev
```

## Alternative: Use Startup Scripts (Optional)

If you prefer to use the startup scripts:

**Option 1: Batch Script**
```powershell
.\start-dev.bat
```

**Option 2: PowerShell Script**
```powershell
.\start-dev.ps1
```

These scripts will start both servers in separate windows.

## Access Points

Once running:
- **React Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5176
- **Swagger UI**: http://localhost:5176/swagger

## Notes

- Both servers must be started separately
- Run them in separate terminal windows
- Close the terminal windows or press Ctrl+C to stop the servers
