FROM node:16 as build-stage
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG ENV
RUN npm run $ENV

FROM nginx:stable-alpine as production-stage
EXPOSE 80

COPY --from=build-stage /app/wwwroot /usr/share/nginx/html
COPY --from=build-stage /app/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80 || exit

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
