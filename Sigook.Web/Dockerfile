FROM node:16 AS build-stage
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm ci

# Copy source code
COPY . .

# Set build argument (default: production)
ARG ENV=production

# Display build environment info
RUN echo "========================================" && \
    echo "Sigook.Web Docker Build" && \
    echo "========================================" && \
    echo "ENV argument received: ${ENV}" && \
    echo "Node version: $(node --version)" && \
    echo "NPM version: $(npm --version)" && \
    echo "========================================"

# Run Vue CLI build
RUN set -e && \
    echo "Building for environment: ${ENV}" && \
    if [ "${ENV}" = "staging" ]; then \
      echo "✅ Running: npm run staging" && \
      echo "✅ Will load: .env.staging" && \
      npm run staging; \
    else \
      echo "✅ Running: npm run production" && \
      echo "✅ Will load: .env.production" && \
      npm run production; \
    fi && \
    echo "Build completed successfully"

# Verify build output exists
RUN test -d wwwroot || (echo "ERROR: Build did not create wwwroot directory" && exit 1)

FROM nginx:stable-alpine AS production-stage
EXPOSE 80

COPY --from=build-stage /app/wwwroot /usr/share/nginx/html
COPY --from=build-stage /app/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80 || exit

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
