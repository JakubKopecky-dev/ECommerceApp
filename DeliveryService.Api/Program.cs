using DeliveryService.Persistence;
using DeliveryService.Application;
using DeliveryService.Api.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Register Services (Dependency Injection)

// Persistence (DbContext, Repositories)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

//Authentization
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

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
