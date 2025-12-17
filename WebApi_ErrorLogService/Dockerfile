# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6050
EXPOSE 6051


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["2_KameraData/KameraData.csproj", "2_KameraData/"]
COPY ["SharedMicroserviceLibrary/SharedMicroserviceLibrary.csproj", "SharedMicroserviceLibrary/"]
COPY ["WebApi_ErrorLogService/WebApi_ErrorLogService.csproj", "WebApi_ErrorLogService/"]
RUN dotnet restore "./WebApi_ErrorLogService/WebApi_ErrorLogService.csproj"
COPY . .
WORKDIR "/src/WebApi_ErrorLogService"
RUN dotnet build "./WebApi_ErrorLogService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi_ErrorLogService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi_ErrorLogService.dll"]