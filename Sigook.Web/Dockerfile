FROM node:22 AS build-stage
WORKDIR /app

# Enable pnpm via corepack (ships with Node 22)
RUN corepack enable

# Copy manifest + lockfile + pnpm config first to leverage Docker layer cache
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml .npmrc ./
RUN pnpm install --frozen-lockfile

# Copy source code
COPY . .

# Set build argument (NO default - must be passed explicitly)
ARG ENV

# Validate ENV argument
RUN if [ -z "${ENV}" ]; then \
      echo "ERROR: ENV build argument is required"; \
      exit 1; \
    fi

# Run Vite build
RUN if [ "${ENV}" = "staging" ]; then \
      pnpm run staging; \
    else \
      pnpm run production; \
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
