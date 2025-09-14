#Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY NotificationService.Api/*.csproj NotificationService.Api/
COPY NotificationService.Application/*.csproj NotificationService.Application/
COPY NotificationService.Persistence/*.csproj NotificationService.Persistence/
COPY NotificationService.Domain/*.csproj NotificationService.Domain/
RUN dotnet restore NotificationService.Api/NotificationService.Api.csproj

# copy the rest of the source code
COPY . .
WORKDIR /src/NotificationService.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]