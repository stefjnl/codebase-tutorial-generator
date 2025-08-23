# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files first to leverage Docker layer caching
COPY ["src/DotNetTutorialGenerator.Api/DotNetTutorialGenerator.Api.csproj", "src/DotNetTutorialGenerator.Api/"]
COPY ["src/DotNetTutorialGenerator.Console/DotNetTutorialGenerator.Console.csproj", "src/DotNetTutorialGenerator.Console/"]
RUN dotnet restore "src/DotNetTutorialGenerator.Api/DotNetTutorialGenerator.Api.csproj"
RUN dotnet restore "src/DotNetTutorialGenerator.Console/DotNetTutorialGenerator.Console.csproj"

# Copy remaining files
COPY . .

# Build the projects
WORKDIR "/src/src/DotNetTutorialGenerator.Api"
RUN dotnet build "DotNetTutorialGenerator.Api.csproj" -c Release -o /app/build/api
WORKDIR "/src/src/DotNetTutorialGenerator.Console"
RUN dotnet build "DotNetTutorialGenerator.Console.csproj" -c Release -o /app/build/console

# Stage 2: Publish
FROM build AS publish
WORKDIR "/src/src/DotNetTutorialGenerator.Api"
RUN dotnet publish "DotNetTutorialGenerator.Api.csproj" -c Release -o /app/publish/api
WORKDIR "/src/src/DotNetTutorialGenerator.Console"
RUN dotnet publish "DotNetTutorialGenerator.Console.csproj" -c Release -o /app/publish/console

# Stage 3: Final image (API)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final-api
WORKDIR /app
COPY --from=publish /app/publish/api .
USER appuser
EXPOSE 80
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Api.dll"]

# Stage 4: Final image (Console)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final-console
WORKDIR /app
COPY --from=publish /app/publish/console .
USER appuser
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Console.dll"]
