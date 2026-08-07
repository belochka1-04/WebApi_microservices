# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать
# как настроить контейнер отладки и как Visual Studio использует этот Dockerfile
# для создания образов для более быстрой отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Пути относительно корня решения (контекст docker compose = .)
COPY WebApi_ApplicationService/WebApi_applications.csproj WebApi_ApplicationService/
COPY 2_KameraData/KameraData.csproj 2_KameraData/
COPY SharedMicroserviceLibrary/SharedMicroserviceLibrary.csproj SharedMicroserviceLibrary/

RUN for i in 1 2 3 4 5; do \
    dotnet restore WebApi_ApplicationService/WebApi_applications.csproj \
      --source https://www.nuget.org/api/v2/ \
      --disable-parallel \
    && break; \
    if [ "$i" = "5" ]; then exit 1; fi; \
    echo "dotnet restore failed, retry $i/5"; \
    sleep 15; \
  done

COPY . .
WORKDIR /src/WebApi_ApplicationService
RUN dotnet build WebApi_applications.csproj -c $BUILD_CONFIGURATION -o /app/build

# Публикация проекта службы
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish WebApi_applications.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi_applications.dll"]
