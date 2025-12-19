# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать
# как настроить контейнер отладки и как Visual Studio использует этот Dockerfile
# для создания образов для более быстрой отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

# Этап сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Пути относительно корня решения (контекст docker compose = .)
COPY 2_KameraData/KameraData.csproj 2_KameraData/
COPY SharedMicroserviceLibrary/SharedMicroserviceLibrary.csproj SharedMicroserviceLibrary/
COPY WebApi_MaskService/WebApi_MaskService.csproj WebApi_MaskService/

RUN dotnet restore WebApi_MaskService/WebApi_MaskService.csproj

COPY . .
WORKDIR /src/WebApi_MaskService
RUN dotnet build WebApi_MaskService.csproj -c $BUILD_CONFIGURATION -o /app/build

# Публикация проекта службы
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish WebApi_MaskService.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ для рабочего режима
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi_MaskService.dll"]
