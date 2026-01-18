# Script to start both backend API and React frontend

Write-Host "Starting ECommerce Development Environment..." -ForegroundColor Green
Write-Host ""

# Start Backend API
Write-Host "Starting .NET API..." -ForegroundColor Cyan
$apiProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\src\ECommerce.Api'; dotnet run" -PassThru

# Wait a moment for API to start
Start-Sleep -Seconds 3

# Start React Frontend
Write-Host "Starting React Frontend..." -ForegroundColor Cyan
$reactProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\client'; npm run dev" -PassThru

Write-Host ""
Write-Host "Both servers are starting in separate windows..." -ForegroundColor Green
Write-Host "Backend API: http://localhost:5176" -ForegroundColor Yellow
Write-Host "React Frontend: http://localhost:3000" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press Ctrl+C to stop all processes" -ForegroundColor Gray

# Wait for user to stop
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
} finally {
    Write-Host "Stopping processes..." -ForegroundColor Red
    Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue
    Stop-Process -Id $reactProcess.Id -Force -ErrorAction SilentlyContinue
    Write-Host "All processes stopped." -ForegroundColor Green
}
