FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/src/SmartHome.Api/SmartHome.Api.csproj backend/src/SmartHome.Api/
COPY backend/src/SmartHome.Domain/SmartHome.Domain.csproj backend/src/SmartHome.Domain/
COPY backend/src/SmartHome.Infrastructure/SmartHome.Infrastructure.csproj backend/src/SmartHome.Infrastructure/

RUN dotnet restore backend/src/SmartHome.Api/SmartHome.Api.csproj

COPY backend/src/SmartHome.Api/ backend/src/SmartHome.Api/
COPY backend/src/SmartHome.Domain/ backend/src/SmartHome.Domain/
COPY backend/src/SmartHome.Infrastructure/ backend/src/SmartHome.Infrastructure/

RUN dotnet publish backend/src/SmartHome.Api/SmartHome.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN addgroup --system app && adduser --system --ingroup app app && \
    apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

USER app

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartHome.Api.dll"]
