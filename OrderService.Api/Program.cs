
using System.Text.Json.Serialization;
using OrderService.Api.DependencyInjection;
using OrderService.Application;
using OrderService.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependency Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Autentization
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

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
        options.SwaggerEndpoint("ProductService/swagger.json", "ProductService - v1");
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

