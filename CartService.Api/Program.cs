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

// HTTP Context accessor
builder.Services.AddHttpContextAccessor();

// HTTP client for OrderService
builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OrderService:BaseUrl"]!);
});

// HTTP client for ProductService
builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProductService:BaseUrl"]!);
});


// Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerWithJwt();


#endregion

var app = builder.Build();


#region Middleware pipeline

// Swagger in a DEV solution
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("CartService/swagger.json", "CartService - v1");
    });
}

// Global error handling
app.UseGlobalExceptionHandling();

// Client cancellation logging
app.UseClientCancellationLogging();

// HTTPS Redirect
app.UseHttpsRedirection();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller map
app.MapControllers();


#endregion

app.Run();

public partial class Program { };
