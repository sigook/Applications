FROM node:16 AS build-stage
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG ENV
RUN echo "🔍 DEBUG: ENV variable = $ENV" && \
    echo "🔍 DEBUG: Checking .env files:" && \
    ls -la .env* || echo "No .env files found" && \
    echo "🔍 DEBUG: Available npm scripts:" && \
    npm run && \
    echo "🔍 DEBUG: Starting build with: npm run $ENV" && \
    npm run $ENV 2>&1 || (echo "❌ Build failed with exit code $?" && exit 1) && \
    echo "✅ Build completed successfully" && \
    echo "🔍 DEBUG: Listing /app directory:" && \
    ls -la && \
    if [ ! -d "wwwroot" ]; then \
      echo "❌ ERROR: wwwroot directory not found!" && \
      exit 1; \
    fi && \
    echo "✅ wwwroot directory verified" && \
    ls -la wwwroot/

FROM nginx:stable-alpine AS production-stage
EXPOSE 80

COPY --from=build-stage /app/wwwroot /usr/share/nginx/html
COPY --from=build-stage /app/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80 || exit

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
