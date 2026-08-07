FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# пути Ч относительно контекста (корн€ microservices), а не папки Dockerfile
COPY WebApi_PartSourceService/WebApi_PartSourceService.csproj WebApi_PartSourceService/
COPY 2_KameraData/KameraData.csproj 2_KameraData/
COPY SharedMicroserviceLibrary/SharedMicroserviceLibrary.csproj SharedMicroserviceLibrary/

RUN for i in 1 2 3 4 5; do \
    dotnet restore WebApi_PartSourceService/WebApi_PartSourceService.csproj \
      --source https://www.nuget.org/api/v2/ \
      --disable-parallel \
    && break; \
    if [ "$i" = "5" ]; then exit 1; fi; \
    echo "dotnet restore failed, retry $i/5"; \
    sleep 15; \
  done

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
