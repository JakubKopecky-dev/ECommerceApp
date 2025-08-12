using System.Text.Json.Serialization;
using DeliveryService.Api.DependencyInjection;
using DeliveryService.Api.Middleware;
using DeliveryService.Application;
using DeliveryService.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region Register Services (Dependency Injection)

// Persistence (DbContext, Repositories)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

//Authentization
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// HTTP Context accessor
builder.Services.AddHttpContextAccessor();

// HTTP client for OrderService
builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OrderService:BaseUrl"]!);
});

// MassTransit + RebbitMQ
builder.Services.AddMassTransitService();

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
        options.SwaggerEndpoint("DeliveryService/swagger.json", "DeliveryService - v1");
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

#endregion


app.Run();

public partial class Program { };
