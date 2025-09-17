using System.Text.Json.Serialization;
using DeliveryService.Api.DependencyInjection;
using DeliveryService.Api.Grpc.GrpcServices;
using DeliveryService.Api.Middleware;
using DeliveryService.Application;
using DeliveryService.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region Register Services (Dependency Injection)

// Persistence (DbContext, Repositories)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
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
builder.Services.AddSwaggerWithJwt();

#endregion


var app = builder.Build();

#region Middleware pipeline

// Swagger
if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/DeliveryService/swagger.json", "DeliveryService - v1");
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

#endregion


app.Run();



public partial class Program { };

