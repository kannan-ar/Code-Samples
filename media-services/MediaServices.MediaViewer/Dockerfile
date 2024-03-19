FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:9000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MediaServices.MediaViewer/MediaServices.MediaViewer.csproj", "MediaServices.MediaViewer/"]
RUN dotnet restore "./MediaServices.MediaViewer/./MediaServices.MediaViewer.csproj"
COPY . .
WORKDIR "/src/MediaServices.MediaViewer"
RUN dotnet build "./MediaServices.MediaViewer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MediaServices.MediaViewer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaServices.MediaViewer.dll"]