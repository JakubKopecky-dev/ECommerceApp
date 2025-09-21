using System.Text.Json.Serialization;
using CartService.Api.DependencyInjection;
using CartService.Api.Middleware;
using CartService.Application;
using CartService.Application.Interfaces.Services;
using CartService.Persistence;


var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependecy Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Authentication
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// gRPC clients
builder.Services.AddGrpcClients(builder.Configuration);

// Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerWithJwt();


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
        options.SwaggerEndpoint("/swagger/CartService/swagger.json", "CartService - v1");
    });
}

// Global error handling
app.UseGlobalExceptionHandling();

// Client cancellation logging
app.UseClientCancellationLogging();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller map
app.MapControllers();


#endregion

app.Run();


public partial class Program { };

