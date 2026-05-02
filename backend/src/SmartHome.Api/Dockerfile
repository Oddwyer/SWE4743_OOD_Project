FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/src/SmartHome.Api/SmartHome.Api.csproj backend/src/SmartHome.Api/
COPY backend/src/SmartHome.Domain/SmartHome.Domain.csproj backend/src/SmartHome.Domain/
COPY backend/src/SmartHome.Infrastructure/SmartHome.Infrastructure.csproj backend/src/SmartHome.Infrastructure/

RUN dotnet restore backend/src/SmartHome.Api/SmartHome.Api.csproj

COPY . .

RUN dotnet publish backend/src/SmartHome.Api/SmartHome.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartHome.Api.dll"]