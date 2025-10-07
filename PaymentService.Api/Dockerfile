#Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files first
COPY PaymentService.Api/*.csproj PaymentService.Api/
RUN dotnet restore PaymentService.Api/PaymentService.Api.csproj

# copy the rest of the source code
COPY . .
WORKDIR /src/PaymentService.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.Api.dll"]