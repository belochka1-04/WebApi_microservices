# См. статью по ссылке [https://aka.ms/customizecontainer](https://aka.ms/customizecontainer), чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6050
EXPOSE 6051

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["2_KameraData/KameraData.csproj", "2_KameraData/"]
COPY ["SharedMicroserviceLibrary/SharedMicroserviceLibrary.csproj", "SharedMicroserviceLibrary/"]
COPY ["WebApi_MaskService/WebApi_MaskService.csproj", "WebApi_MaskService/"]

RUN dotnet restore "./WebApi_MaskService/WebApi_MaskService.csproj"
COPY . .
WORKDIR "/src/WebApi_MaskService"
RUN dotnet build "./WebApi_MaskService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi_MaskService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi_MaskService.dll"]
