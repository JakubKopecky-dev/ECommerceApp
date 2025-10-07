#Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY DeliveryService.Api/*.csproj DeliveryService.Api/
COPY DeliveryService.Application/*.csproj DeliveryService.Application/
COPY DeliveryService.Persistence/*.csproj DeliveryService.Persistence/
COPY DeliveryService.Domain/*.csproj DeliveryService.Domain/
RUN dotnet restore DeliveryService.Api/DeliveryService.Api.csproj

# copy the rest of the source code
COPY . .
WORKDIR /src/DeliveryService.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DeliveryService.Api.dll"]