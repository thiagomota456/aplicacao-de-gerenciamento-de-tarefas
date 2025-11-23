#!/bin/bash
echo "Limpando ambiente..."
docker rm -f taskmanager_api taskmanager_ui taskmanager_db 2>/dev/null
if command -v docker-compose &> /dev/null; then
    docker-compose down
elif docker compose version &> /dev/null; then
    docker compose down
fi
echo "Limpeza concluída."
