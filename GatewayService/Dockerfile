# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY GatewayService/*.csproj GatewayService/
RUN dotnet restore GatewayService/GatewayService.csproj

# copy the rest of the source code
COPY . .
RUN dotnet publish GatewayService/GatewayService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GatewayService.dll"]
