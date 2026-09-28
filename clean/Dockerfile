# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/ShopLego.Api/ShopLego.Api.csproj && \
    dotnet publish src/ShopLego.Api/ShopLego.Api.csproj -c Release --no-restore -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080
COPY --from=build --chown=app:app /app .
USER $APP_UID
ENTRYPOINT ["dotnet", "ShopLego.Api.dll"]
