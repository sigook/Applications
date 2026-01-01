FROM node:16 AS build-stage
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm ci

# Copy source code
COPY . .

# Set build argument
ARG ENV=production
RUN echo "Building for environment: ${ENV}"

# Run Vue CLI build
# Force production build mode and output to wwwroot
RUN set -e && \
    if [ "${ENV}" = "staging" ]; then \
      echo "Running: npm run staging" && \
      npm run staging && \
      echo "✅ Staging build completed"; \
    else \
      echo "Running: npm run production" && \
      npm run production && \
      echo "✅ Production build completed"; \
    fi && \
    echo "Checking for wwwroot..." && \
    ls -la

# Verify wwwroot was created
RUN ls -la wwwroot/ || (echo "ERROR: Build did not create wwwroot directory!" && exit 1)

FROM nginx:stable-alpine AS production-stage
EXPOSE 80

COPY --from=build-stage /app/wwwroot /usr/share/nginx/html
COPY --from=build-stage /app/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80 || exit

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
