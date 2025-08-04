using System.Text.Json.Serialization;
using CartService.Api.DependencyInjection;
using CartService.Application;
using CartService.Application.Interfaces.Services;
using CartService.Persistence;
using CartServiceService = CartService.Application.Services.CartService;

var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependecy Injection)

// Persitence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Authentication
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// HTTP Context accessor
builder.Services.AddHttpContextAccessor();

// HTTP client + Registre CartService
var orderServiceUrl = builder.Configuration["OrderService:BaseUrl"];
builder.Services.AddHttpClient<ICartService, CartServiceService>(client =>
{
    client.BaseAddress = new Uri(orderServiceUrl!);
});

// Controllers
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
        options.SwaggerEndpoint("CartService/swagger.json", "CartService - v1");
    });
}


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
