# Azure Friday - Developer Setup
# Run this script once after cloning to install all prerequisites.
# Prerequisites: .NET 10 SDK, Node.js 20+

param(
    [switch]$SkipRestore
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$core = Join-Path $root "azure-friday.core"

Write-Host "=== Azure Friday Developer Setup ===" -ForegroundColor Cyan

# Check prerequisites
Write-Host "`nChecking prerequisites..." -ForegroundColor Yellow

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Host "ERROR: .NET SDK not found. Install from https://dot.net" -ForegroundColor Red
    exit 1
}
$dotnetVersion = dotnet --version
Write-Host "  .NET SDK: $dotnetVersion" -ForegroundColor Green

$node = Get-Command node -ErrorAction SilentlyContinue
if (-not $node) {
    Write-Host "ERROR: Node.js not found. Install from https://nodejs.org" -ForegroundColor Red
    exit 1
}
$nodeVersion = node --version
Write-Host "  Node.js:  $nodeVersion" -ForegroundColor Green

# Install npm dependencies (Tailwind CSS)
Write-Host "`nInstalling npm dependencies..." -ForegroundColor Yellow
Push-Location $core
try {
    npm install --ignore-scripts
    if ($LASTEXITCODE -ne 0) { throw "npm install failed" }
    Write-Host "  Tailwind CSS installed" -ForegroundColor Green
} finally {
    Pop-Location
}

# Build Tailwind CSS
Write-Host "`nBuilding Tailwind CSS..." -ForegroundColor Yellow
Push-Location $core
try {
    npx tailwindcss -i wwwroot/css/input.css -o wwwroot/css/tailwind.css --minify
    if ($LASTEXITCODE -ne 0) { throw "Tailwind build failed" }
    $size = (Get-Item wwwroot/css/tailwind.css).Length / 1KB
    Write-Host "  Built wwwroot/css/tailwind.css ($([math]::Round($size, 1)) KB)" -ForegroundColor Green
} finally {
    Pop-Location
}

# Restore .NET packages
if (-not $SkipRestore) {
    Write-Host "`nRestoring .NET packages..." -ForegroundColor Yellow
    dotnet restore "$root/azure-friday.sln"
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }
    Write-Host "  .NET packages restored" -ForegroundColor Green
}

# Build
Write-Host "`nBuilding solution..." -ForegroundColor Yellow
dotnet build "$root/azure-friday.sln" --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

# Run tests
Write-Host "`nRunning tests..." -ForegroundColor Yellow
dotnet test "$root/azure-friday.tests" --no-build
if ($LASTEXITCODE -ne 0) { throw "Tests failed" }

Write-Host "`n=== Setup Complete ===" -ForegroundColor Cyan
Write-Host @"

Ready to develop! Common commands:

  dotnet run --project azure-friday.core     # Run the site locally
  npm run watch:css                          # Watch mode for Tailwind (run in azure-friday.core/)
  dotnet test azure-friday.tests             # Run all tests

For active CSS development, run in two terminals:
  Terminal 1:  cd azure-friday.core && npm run watch:css
  Terminal 2:  dotnet run --project azure-friday.core

"@ -ForegroundColor White
