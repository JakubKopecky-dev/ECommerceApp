using System.Text.Json.Serialization;
using NotificationService.Api.DependencyInjection;
using NotificationService.Api.Hubs;
using NotificationService.Api.Middleware;
using NotificationService.Application;
using NotificationService.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependency Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Authentication
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// MassTransit + RebbitMQ
builder.Services.AddMassTransitService();

// Controllers & JSON setting
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Register signalR
builder.Services.AddSignalR();

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
        options.SwaggerEndpoint("NotificationService/swagger.json", "NotificationService - v1");
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

// Notification hubu map
app.MapHub<NotificationHub>("/hubs/notifications");

#endregion


app.Run();


public partial class Program { };






/*
 






1315e35c-0d2c-42c0-6908-08ddd6a13a94

*/