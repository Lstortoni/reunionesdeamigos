FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Api/src/ReunionesDeAmigos.Api/ReunionesDeAmigos.Api.csproj", "Api/src/ReunionesDeAmigos.Api/"]
COPY ["Api/src/ReunionesDeAmigos.Application/ReunionesDeAmigos.Application.csproj", "Api/src/ReunionesDeAmigos.Application/"]
COPY ["Api/src/ReunionesDeAmigos.Domain/ReunionesDeAmigos.Domain.csproj", "Api/src/ReunionesDeAmigos.Domain/"]
COPY ["Api/src/ReunionesDeAmigos.Infrastructure/ReunionesDeAmigos.Infrastructure.csproj", "Api/src/ReunionesDeAmigos.Infrastructure/"]
RUN dotnet restore "Api/src/ReunionesDeAmigos.Api/ReunionesDeAmigos.Api.csproj"

COPY . .
RUN dotnet publish "Api/src/ReunionesDeAmigos.Api/ReunionesDeAmigos.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ReunionesDeAmigos.Api.dll"]
