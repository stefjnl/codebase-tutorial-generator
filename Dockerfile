# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files for dependency resolution
COPY ["src/DotNetTutorialGenerator.Core/DotNetTutorialGenerator.Core.csproj", "src/DotNetTutorialGenerator.Core/"]
COPY ["src/DotNetTutorialGenerator.Infrastructure/DotNetTutorialGenerator.Infrastructure.csproj", "src/DotNetTutorialGenerator.Infrastructure/"]
COPY ["src/DotNetTutorialGenerator.Api/DotNetTutorialGenerator.Api.csproj", "src/DotNetTutorialGenerator.Api/"]
COPY ["src/DotNetTutorialGenerator.Console/DotNetTutorialGenerator.Console.csproj", "src/DotNetTutorialGenerator.Console/"]
COPY ["src/DotNetTutorialGenerator.Blazor/DotNetTutorialGenerator.Blazor.csproj", "src/DotNetTutorialGenerator.Blazor/"]
COPY ["Directory.Build.props", "./"]
COPY ["global.json", "./"]

# Restore dependencies
RUN dotnet restore "src/DotNetTutorialGenerator.Api/DotNetTutorialGenerator.Api.csproj"
RUN dotnet restore "src/DotNetTutorialGenerator.Console/DotNetTutorialGenerator.Console.csproj"
RUN dotnet restore "src/DotNetTutorialGenerator.Blazor/DotNetTutorialGenerator.Blazor.csproj"

# Copy remaining files
COPY . .

# Stage 2: Publish (skip separate build stage)
FROM build AS publish

# Publish directly without separate build step
WORKDIR "/src/src/DotNetTutorialGenerator.Api"
RUN dotnet publish "DotNetTutorialGenerator.Api.csproj" -c Release -o /app/publish/api

WORKDIR "/src/src/DotNetTutorialGenerator.Console"
RUN dotnet publish "DotNetTutorialGenerator.Console.csproj" -c Release -o /app/publish/console

WORKDIR "/src/src/DotNetTutorialGenerator.Blazor"
RUN dotnet publish "DotNetTutorialGenerator.Blazor.csproj" -c Release -o /app/publish/blazor

# Stage 3: Final image (API)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final-api
WORKDIR /app
COPY --from=publish /app/publish/api .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Api.dll"]

# Stage 4: Final image (Console)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final-console
WORKDIR /app
COPY --from=publish /app/publish/console .
# Note: Console apps exit after completion - this is expected behavior
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Console.dll"]

# Stage 5: Final image (Blazor)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final-blazor
WORKDIR /app
COPY --from=publish /app/publish/blazor .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Blazor.dll"]