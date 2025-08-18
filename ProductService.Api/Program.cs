using System.Text.Json.Serialization;
using ProductService.Api.DependencyInjection;
using ProductService.Api.Grpc.GrpcServices;
using ProductService.Api.Middleware;
using ProductService.Application;
using ProductService.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependency Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Autentization
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// MassTransit + RebbitMQ
builder.Services.AddMassTransitService();

// gRPC server
builder.Services.AddGrpc();

// Controllers && JSON Setting
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerWithJwt();

#endregion

var app = builder.Build();


#region Middleware pipeline

// Swagger in DEV solution
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("ProductService/swagger.json", "ProductService - v1");
    });
}

// Global error handling
app.UseGlobalExceptionHandling();

// Client cancellation logging
app.UseClientCancellationLogging();

// HTTPS Redirect
app.UseHttpsRedirection();


// Authentitaction and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller map
app.MapControllers();

// gRPC map services
app.MapGrpcService<ProductGrpcService>();


#endregion

app.Run();

public partial class Program { };
