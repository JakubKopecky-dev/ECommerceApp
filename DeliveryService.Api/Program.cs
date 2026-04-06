using DeliveryService.Api.DependencyInjection;
using DeliveryService.Api.Grpc.GrpcServices;
using DeliveryService.Api.Middleware;
using DeliveryService.Application;
using DeliveryService.Persistence;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP/1.1 for Swagger + Controllers
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // HTTP/2 for gRPC
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});


// Persistence (DbContext, Repositories)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services)
builder.Services.AddApplicationServices();

//Authentication
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// gRPC clients
builder.Services.AddGrpcClients(builder.Configuration);

// MassTransit + RebbitMQ
builder.Services.AddMassTransitService(builder.Configuration);

// gRPC server
builder.Services.AddGrpc();

// Controllers && JSON Setting
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddOpenApiWithJwt();


var app = builder.Build();


var env = app.Services.GetRequiredService<IWebHostEnvironment>();

// Apply migration
if (!env.IsEnvironment("Test"))
    app.ApplyMigrations();

if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";

        if (app.Environment.IsDevelopment())
            options.SwaggerEndpoint("/openapi/v1.json", "DeliveryService - v1");
        else
            options.SwaggerEndpoint("/delivery/openapi/v1.json", "DeliveryService - v1");
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

// gRPC map
app.MapGrpcService<DeliveryGrpcService>();



app.Run();



