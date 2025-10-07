#Build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY UserService.Api/*.csproj UserService.Api/
COPY UserService.Application/*.csproj UserService.Application/
COPY UserService.Persistence/*.csproj UserService.Persistence/
COPY UserService.Infrastructure/*.csproj UserService.Infrastructure/
COPY UserService.Domain/*.csproj UserService.Domain/
RUN dotnet restore UserService.Api/UserService.Api.csproj

# copy the rest of the source code
COPY . .
WORKDIR /src/UserService.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UserService.Api.dll"]