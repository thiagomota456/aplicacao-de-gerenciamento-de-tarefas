#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "Root: $ROOT_DIR"

command -v docker >/dev/null 2>&1 || { echo "Docker não encontrado. Instale/execute o Docker."; exit 1; }

# Check for docker-compose or docker compose
if command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
elif docker compose version &> /dev/null; then
    DOCKER_COMPOSE="docker compose"
else
    echo "Docker Compose não encontrado. Instale-o para continuar."
    exit 1
fi

echo "Building and deploying with Docker Compose..."

# Remove old standalone containers if they exist to avoid conflicts
docker rm -f taskmanager_api taskmanager_ui taskmanager_db >/dev/null 2>&1 || true

# Function to handle cleanup on exit
cleanup() {
    echo ""
    echo "Parando e removendo containers..."
    $DOCKER_COMPOSE down
    exit
}

# Trap Ctrl+C
trap cleanup SIGINT

echo "Iniciando aplicação..."
echo "API: http://localhost:8080"
echo "UI: http://localhost:3000"
echo "Pressione Ctrl+C para encerrar e limpar o ambiente."

$DOCKER_COMPOSE up --build --remove-orphans