# See https://aka.ms/customizecontainer to learn how to customize your debug container.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Api/WolverinePoc.Api.csproj", "src/Api/"]
RUN dotnet restore "src/Api/WolverinePoc.Api.csproj" --configfile NuGet.Config
COPY . .
WORKDIR "/src/src/Api"
RUN dotnet build "WolverinePoc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WolverinePoc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WolverinePoc.Api.dll"]
