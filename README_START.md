# Quick Start Guide

## Automatic Start (Recommended)

Simply run:
```powershell
cd src\ECommerce.Api
dotnet run
```

This will automatically:
1. Start the .NET API backend
2. Launch the React frontend in a separate window

Both servers will start automatically!

## Manual Start (Alternative)

If you prefer to start them manually or the automatic start doesn't work:

**Option 1: Use the startup script**
```powershell
.\start-dev.bat
```
or
```powershell
.\start-dev.ps1
```

**Option 2: Run separately**
- Terminal 1: `cd src\ECommerce.Api; dotnet run`
- Terminal 2: `cd client; npm run dev`

## Access Points

Once running:
- **React Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5176
- **Swagger UI**: http://localhost:5176/swagger

## Notes

- The automatic React server start only works in **Debug** mode
- Both servers run in separate windows
- Close the windows to stop the servers
