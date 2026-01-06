FROM mcr.microsoft.com/dotnet/sdk:6.0.400 AS build
WORKDIR /app
COPY . .
RUN dotnet restore Covenant.Api/Covenant.Api.csproj
RUN dotnet publish Covenant.Api/Covenant.Api.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/out ./

HEALTHCHECK --interval=30s --timeout=10s --retries=3 --start-period=40s \
    CMD curl --fail --silent --show-error http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "Covenant.Api.dll"]