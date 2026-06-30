# Imagem do Nginx que serve o frontend (build estático do Vite) e faz
# proxy reverso de /api -> serviço "api" (.NET) na rede do Docker Compose.
#
# O build do frontend NÃO é feito aqui. Ele é gerado no host pelo script
# scripts/build-frontend.sh, que copia o resultado para ./nginx/dist.
# Assim o build da imagem do Nginx é rápido e não exige Node dentro dela.
FROM nginx:1.27-alpine

# Remove a config default do Nginx e injeta a nossa.
RUN rm -f /etc/nginx/conf.d/default.conf
COPY default.conf /etc/nginx/conf.d/default.conf

# Conteudo estatico do frontend (gerado previamente em ./nginx/dist).
COPY dist/ /usr/share/nginx/html/

EXPOSE 80

HEALTHCHECK --interval=10s --timeout=5s --start-period=10s --retries=5 \
    CMD wget -q -O /dev/null http://localhost/ || exit 1
