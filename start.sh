#!/bin/bash
set -e

# Inicia o Nginx em background
nginx &

# Inicia a API .NET em foreground (para manter o contêiner rodando)
cd /app/backend
dotnet TaskManagerApi.dll