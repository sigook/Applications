FROM node:16 AS build-stage
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm ci

# Copy source code
COPY . .

# Set build argument (NO default - must be passed explicitly)
ARG ENV

# Validate ENV argument
RUN if [ -z "${ENV}" ]; then \
      echo "ERROR: ENV build argument is required"; \
      exit 1; \
    fi

# Run Vue CLI build
RUN if [ "${ENV}" = "staging" ]; then \
      npm run staging; \
    else \
      npm run production; \
    fi

# Verify build output exists
RUN test -d wwwroot || (echo "ERROR: Build did not create wwwroot directory" && exit 1)

FROM nginx:stable-alpine AS production-stage
EXPOSE 80

COPY --from=build-stage /app/wwwroot /usr/share/nginx/html
COPY --from=build-stage /app/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80 || exit

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
