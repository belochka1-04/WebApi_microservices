FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# пути — относительно контекста (корня microservices), а не папки Dockerfile
COPY WebApi_PartSourceService/WebApi_PartSourceService.csproj WebApi_PartSourceService/
COPY 2_KameraData/KameraData.csproj 2_KameraData/
COPY SharedMicroserviceLibrary/SharedMicroserviceLibrary.csproj SharedMicroserviceLibrary/

RUN dotnet restore WebApi_PartSourceService/WebApi_PartSourceService.csproj

COPY . .
WORKDIR /src/WebApi_PartSourceService
RUN dotnet build WebApi_PartSourceService.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish WebApi_PartSourceService.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi_PartSourceService.dll"]
