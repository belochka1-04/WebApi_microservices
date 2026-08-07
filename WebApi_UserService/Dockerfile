# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# paths are relative to solution root (compose context = .)
COPY WebApi_UserService/WebApi_UserService.csproj WebApi_UserService/
COPY 2_KameraData/KameraData.csproj 2_KameraData/
COPY SharedMicroserviceLibrary/SharedMicroserviceLibrary.csproj SharedMicroserviceLibrary/

RUN for i in 1 2 3 4 5; do \
    dotnet restore WebApi_UserService/WebApi_UserService.csproj \
      --source https://www.nuget.org/api/v2/ \
      --disable-parallel \
    && break; \
    if [ "$i" = "5" ]; then exit 1; fi; \
    echo "dotnet restore failed, retry $i/5"; \
    sleep 15; \
  done

COPY . .
WORKDIR /src/WebApi_UserService
RUN dotnet build WebApi_UserService.csproj -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish WebApi_UserService.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi_UserService.dll"]
