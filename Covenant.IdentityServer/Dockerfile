FROM mcr.microsoft.com/dotnet/sdk:6.0.400 AS build
WORKDIR /app
COPY . .
COPY ["Covenant.IdentityServer/nuget.config", ""]

ARG PAT
RUN dotnet restore Covenant.IdentityServer/Covenant.IdentityServer.csproj --configfile "./Covenant.IdentityServer/nuget.config"
RUN dotnet build Covenant.IdentityServer/Covenant.IdentityServer.csproj -c Release --no-restore
RUN dotnet publish Covenant.IdentityServer/Covenant.IdentityServer.csproj -c Release -o out --no-restore

#App image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80
WORKDIR /app
COPY --from=build /app/out ./
HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80/health || exit
ENTRYPOINT ["dotnet","Covenant.IdentityServer.dll"]