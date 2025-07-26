#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# ASP.NET Core runtime (includes Microsoft.AspNetCore.App)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

# SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and restore the project
COPY ["domain-service/DomainService.csproj", "domain-service/"]
RUN dotnet restore "./domain-service/DomainService.csproj"

# Copy the entire source and build
COPY . .
WORKDIR "/src/domain-service"
RUN dotnet build "./DomainService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DomainService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image with ASP.NET Core runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose Orleans Dashboard port if needed
EXPOSE 8081

ENTRYPOINT ["dotnet", "DomainService.dll"]
