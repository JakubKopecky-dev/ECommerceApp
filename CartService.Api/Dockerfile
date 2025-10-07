# Build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY CartService.Api/*.csproj CartService.Api/
COPY CartService.Application/*.csproj CartService.Application/
COPY CartService.Persistence/*.csproj CartService.Persistence/
COPY CartService.Domain/*.csproj CartService.Domain/
RUN dotnet restore CartService.Api/CartService.Api.csproj

# copy the rest of the source code
COPY . .
WORKDIR /src/CartService.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CartService.Api.dll"]
