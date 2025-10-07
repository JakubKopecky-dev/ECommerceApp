#Build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY OrderService.Api/*.csproj OrderService.Api/
COPY OrderService.Application/*.csproj OrderService.Application/
COPY OrderService.Persistence/*.csproj OrderService.Persistence/
COPY OrderService.Domain/*.csproj OrderService.Domain/
RUN dotnet restore OrderService.Api/OrderService.Api.csproj

# copy the rest of the source code
COPY . .
WORKDIR /src/OrderService.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]