#!/usr/bin/env bash
#
# build-frontend.sh
#
# Builda o frontend (Vue 3 + Vite) e coloca o resultado em ./nginx/dist,
# que e o diretorio copiado pela imagem do Nginx no docker compose.
#
# Uso (a partir da raiz do projeto):
#   ./scripts/build-frontend.sh                 # build + (re)cria a imagem nginx
#   ./scripts/build-frontend.sh --no-deploy     # apenas builda, nao mexe no docker
#
# Requisitos no host: Node.js 20+ e npm.
#
set -euo pipefail

# Raiz do projeto = pasta pai do diretorio onde este script esta.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"

FRONTEND_DIR="${ROOT_DIR}/frontend"
DIST_SRC="${FRONTEND_DIR}/dist"
NGINX_DIST="${ROOT_DIR}/nginx/dist"

DEPLOY=true
if [[ "${1:-}" == "--no-deploy" ]]; then
    DEPLOY=false
fi

echo "==> Raiz do projeto: ${ROOT_DIR}"

if ! command -v npm >/dev/null 2>&1; then
    echo "ERRO: npm nao encontrado. Instale Node.js 20+ no host." >&2
    exit 1
fi

echo "==> Instalando dependencias do frontend"
cd "${FRONTEND_DIR}"
if [[ -f package-lock.json ]]; then
    # Tenta o install reprodutivel (npm ci); se o lock estiver dessincronizado
    # com o package.json, cai para npm install em vez de abortar o build.
    npm ci || npm install
else
    npm install
fi

echo "==> Buildando o frontend (vite build)"
npm run build

if [[ ! -d "${DIST_SRC}" ]]; then
    echo "ERRO: build nao gerou ${DIST_SRC}" >&2
    exit 1
fi

echo "==> Copiando build para ${NGINX_DIST}"
rm -rf "${NGINX_DIST}"
mkdir -p "${NGINX_DIST}"
cp -r "${DIST_SRC}/." "${NGINX_DIST}/"

if [[ "${DEPLOY}" == "true" ]]; then
    echo "==> Reconstruindo e subindo o container do Nginx"
    cd "${ROOT_DIR}"
    docker compose up -d --build nginx
    echo "==> Pronto. Frontend disponivel via container nginx."
else
    echo "==> Build copiado. Pulei o deploy (--no-deploy)."
    echo "    Para aplicar: docker compose up -d --build nginx"
fi
