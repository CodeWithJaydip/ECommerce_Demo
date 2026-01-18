@echo off
echo Starting ECommerce Development Environment...
echo.

echo Starting .NET API...
start "ECommerce API" powershell -NoExit -Command "cd /d %~dp0src\ECommerce.Api && dotnet run"

timeout /t 3 /nobreak >nul

echo Starting React Frontend...
start "React Frontend" powershell -NoExit -Command "cd /d %~dp0client && npm run dev"

echo.
echo Both servers are starting in separate windows...
echo Backend API: http://localhost:5176
echo React Frontend: http://localhost:3000
echo.
echo Close the windows or press Ctrl+C to stop the servers.
echo.
pause
