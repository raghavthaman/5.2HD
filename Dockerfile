# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["backend/AboriginalArtGallery.API/AboriginalArtGallery.API.csproj", "backend/AboriginalArtGallery.API/"]
RUN dotnet restore "backend/AboriginalArtGallery.API/AboriginalArtGallery.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/backend/AboriginalArtGallery.API"
RUN dotnet build "AboriginalArtGallery.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AboriginalArtGallery.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set default ASP.NET Core URLs to bind to the port Render expects (8080 or dynamic PORT environment variable)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AboriginalArtGallery.API.dll"]
