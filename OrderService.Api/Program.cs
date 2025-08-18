
using System.Text.Json.Serialization;
using OrderService.Api.DependencyInjection;
using OrderService.Api.Grpc.GrpcServices;
using OrderService.Api.Middleware;
using OrderService.Application;
using OrderService.Application.Interfaces.Services;
using OrderService.Persistence;


var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependency Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Authentication
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// gRPC clients
builder.Services.AddGrpcClients(builder.Configuration);

// MassTransit + RebbitMQ
builder.Services.AddMassTransitService();

// gRPC server
builder.Services.AddGrpc();

// Controllers & JSON setting
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerWithJwt();

#endregion

var app = builder.Build();


#region Middleware pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("OrderService/swagger.json", "OrderService - v1");
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
app.MapGrpcService<OrderGrpcService>();

#endregion

app.Run();

public partial class Program { };

