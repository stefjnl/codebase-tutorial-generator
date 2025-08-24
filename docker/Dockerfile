# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files first to leverage Docker layer caching
COPY ["src/DotNetTutorialGenerator.Api/DotNetTutorialGenerator.Api.csproj", "src/DotNetTutorialGenerator.Api/"]
COPY ["src/DotNetTutorialGenerator.Console/DotNetTutorialGenerator.Console.csproj", "src/DotNetTutorialGenerator.Console/"]
COPY ["src/DotNetTutorialGenerator.Blazor/DotNetTutorialGenerator.Blazor.csproj", "src/DotNetTutorialGenerator.Blazor/"]
RUN dotnet restore "src/DotNetTutorialGenerator.Api/DotNetTutorialGenerator.Api.csproj"
RUN dotnet restore "src/DotNetTutorialGenerator.Console/DotNetTutorialGenerator.Console.csproj"
RUN dotnet restore "src/DotNetTutorialGenerator.Blazor/DotNetTutorialGenerator.Blazor.csproj"

# Copy remaining files
COPY . .

# Build the projects
WORKDIR "/src/src/DotNetTutorialGenerator.Api"
RUN dotnet build "DotNetTutorialGenerator.Api.csproj" -c Release -o /app/build/api
WORKDIR "/src/src/DotNetTutorialGenerator.Console"
RUN dotnet build "DotNetTutorialGenerator.Console.csproj" -c Release -o /app/build/console
WORKDIR "/src/src/DotNetTutorialGenerator.Blazor"
RUN dotnet build "DotNetTutorialGenerator.Blazor.csproj" -c Release -o /app/build/blazor

# Stage 2: Publish
FROM build AS publish
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
USER appuser
EXPOSE 80
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Api.dll"]

# Stage 4: Final image (Console)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final-console
WORKDIR /app
COPY --from=publish /app/publish/console .
USER appuser
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Console.dll"]

# Stage 5: Final image (Blazor)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final-blazor
WORKDIR /app
COPY --from=publish /app/publish/blazor .
USER appuser
EXPOSE 8080
ENTRYPOINT ["dotnet", "DotNetTutorialGenerator.Blazor.dll"]
