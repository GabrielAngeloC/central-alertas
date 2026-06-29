FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/CentralAlertas.Api/CentralAlertas.Api.csproj", "src/CentralAlertas.Api/"]
COPY ["src/CentralAlertas.Application/CentralAlertas.Application.csproj", "src/CentralAlertas.Application/"]
COPY ["src/CentralAlertas.Domain/CentralAlertas.Domain.csproj", "src/CentralAlertas.Domain/"]
COPY ["src/CentralAlertas.Infrastructure/CentralAlertas.Infrastructure.csproj", "src/CentralAlertas.Infrastructure/"]

RUN dotnet restore "src/CentralAlertas.Api/CentralAlertas.Api.csproj"

COPY . .

RUN dotnet publish "src/CentralAlertas.Api/CentralAlertas.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CentralAlertas.Api.dll"]