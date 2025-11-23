param()
Set-StrictMode -Version Latest

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Write-Host "Root: $root"

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker não encontrado. Instale/execute o Docker Desktop."
    exit 1
}

# Build TaskManagerApi (context: repo root)
Write-Host "Building TaskManagerApi..."
docker build -f "$root\TaskManagerApi\Dockerfile" -t taskmanagerapi:latest $root

# Build taskmanager-ui
Write-Host "Building taskmanager-ui..."
docker build -f "$root\taskmanager-ui\Dockerfile" -t taskmanagerui:latest "$root\taskmanager-ui"

# Remove existing containers (ignora erros)
docker rm -f taskmanager_api taskmanager_ui 2>$null

# Run API
Write-Host "Running TaskManagerApi on :8080..."
docker run -d --name taskmanager_api -p 8080:80 taskmanagerapi:latest

# Run UI
Write-Host "Running taskmanager-ui on :3000..."
docker run -d --name taskmanager_ui -p 3000:3000 taskmanagerui:latest

Write-Host "Build e deploy concluídos."