#Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY ProductService.Api/*.csproj ProductService.Api/
COPY ProductService.Application/*.csproj ProductService.Application/
COPY ProductService.Persistence/*.csproj ProductService.Persistence/
COPY ProductService.Domain/*.csproj ProductService.Domain/
RUN dotnet restore ProductService.Api/ProductService.Api.csproj

# copy the rest of the source code
COPY . .
WORKDIR /src/ProductService.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductService.Api.dll"]