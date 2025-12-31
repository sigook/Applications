FROM node:16 AS build-stage
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci

# Copy all source files
COPY . .

# Debug: Show what was copied
RUN echo "========================================" && \
    echo "FILES COPIED TO CONTAINER" && \
    echo "========================================" && \
    find . -maxdepth 1 -name ".env*" && \
    echo "----------------------------------------" && \
    ls -la | grep -E "^-" | wc -l && echo "files copied" && \
    echo "----------------------------------------"

ARG ENV

# Debug: Show environment and files
RUN echo "========================================" && \
    echo "DEBUG INFO" && \
    echo "========================================" && \
    echo "ENV variable: $ENV" && \
    echo "----------------------------------------" && \
    echo "Checking for .env files:" && \
    ls -la .env* 2>/dev/null || echo "❌ No .env files found!" && \
    echo "----------------------------------------" && \
    echo "Checking if .env.staging exists:" && \
    test -f .env.staging && echo "✅ .env.staging exists" || echo "❌ .env.staging missing" && \
    echo "Checking if .env.production exists:" && \
    test -f .env.production && echo "✅ .env.production exists" || echo "❌ .env.production missing"

# Run the build
RUN npm run ${ENV}

# Verify build output
RUN echo "========================================" && \
    echo "BUILD VERIFICATION" && \
    echo "========================================" && \
    ls -la && \
    if [ -d "wwwroot" ]; then \
      echo "✅ wwwroot directory found" && \
      ls -la wwwroot/; \
    else \
      echo "❌ ERROR: wwwroot directory not found!" && \
      exit 1; \
    fi

FROM nginx:stable-alpine AS production-stage
EXPOSE 80

COPY --from=build-stage /app/wwwroot /usr/share/nginx/html
COPY --from=build-stage /app/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80 || exit

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
