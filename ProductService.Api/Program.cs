using ProductService.Persistence;
using ProductService.Application;
using ProductService.Api.DependencyInjection;






var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependency Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services, AutoMapper)
builder.Services.AddApplicationServices();

// Autentization
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// Controllers
builder.Services.AddControllers();

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
        options.SwaggerEndpoint("ProductService/swagger.json", "ProductService - v1");
    });
}

// HTTPS Redirect
app.UseHttpsRedirection();


// Authentitaction and Authorization
app.UseAuthentication();
app.UseAuthorization();

// controller map
app.MapControllers();


#endregion

app.Run();

public partial class Program { };
