# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy file solution và project để restore dependencies (tối ưu Docker caching)
COPY ShopLegoApi.sln ./
COPY ShopLegoApi/ShopLegoApi.csproj ShopLegoApi/
RUN dotnet restore ShopLegoApi/ShopLegoApi.csproj

# Copy toàn bộ mã nguồn và tiến hành publish
COPY ShopLegoApi/ ShopLegoApi/
WORKDIR /src/ShopLegoApi
RUN dotnet publish ShopLegoApi.csproj \
    --configuration ${BUILD_CONFIGURATION} \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS production
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://0.0.0.0:8080 \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0 \
    TZ=Asia/Bangkok

EXPOSE 8080
STOPSIGNAL SIGTERM

COPY --from=build --chown=app:app /app/publish/ ./

USER $APP_UID
CMD ["dotnet", "ShopLegoApi.dll"]
