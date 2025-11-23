#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "Root: $ROOT_DIR"

# Se Docker Desktop não estiver em execução, tenta abrir
if ! command -v docker >/dev/null 2>&1; then
  echo "Docker não encontrado no PATH. Verifique a instalação."
  exit 1
fi

if ! docker info >/dev/null 2>&1; then
  echo "Docker não está rodando. Abrindo Docker Desktop (aguardando inicialização)..."
  open -a Docker || true
  until docker info >/dev/null 2>&1; do
    sleep 1
  done
fi

# Reaproveita o mesmo fluxo do script linux
"$ROOT_DIR/build_and_deploy.sh"