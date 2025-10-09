using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProductService.Api.DependencyInjection;
using ProductService.Api.Grpc.GrpcServices;
using ProductService.Api.Middleware;
using ProductService.Application;
using ProductService.Persistence;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

#region Register services (Dependency Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Autentization
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// MassTransit + RebbitMQ
builder.Services.AddMassTransitService(builder.Configuration);

// gRPC server
builder.Services.AddGrpc();

// Controllers && JSON Setting
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerWithJwt(builder.Environment);

#endregion

var app = builder.Build();


#region Middleware pipeline

var env = app.Services.GetRequiredService<IWebHostEnvironment>();

// Apply migration
if (!env.IsEnvironment("Test"))
    app.ApplyMigrations();

// Swagger
if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("./ProductService/swagger.json", "ProductService - v1");
    });
}

// Global error handling
app.UseGlobalExceptionHandling();

// Client cancellation logging
app.UseClientCancellationLogging();

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
