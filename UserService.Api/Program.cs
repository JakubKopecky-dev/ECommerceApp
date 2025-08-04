using System.Text.Json.Serialization;
using UserService.Api.Auth;
using UserService.Api.DependencyInjection;
using UserService.Infrastructure;
using UserService.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependency Injection)

// Persistence (DbContext)
builder.Services.AddPersistenceServices(builder.Configuration);

// Infrastrucutre (Automapper, Services)
builder.Services.AddInfrastructureServices();

// Identity & Autentization (JWT, UserManager, TokenGenerator)
builder.Services.AddAuthenticationAndIdentityServiceCollection(builder.Configuration);

// Controllers & JSON setting
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
        options.SwaggerEndpoint("UserService/swagger.json", "UserService - v1");
    });
}

// HTTPS Redirect
app.UseHttpsRedirection();


// Authentitaction and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller map
app.MapControllers();

// Add roles
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
if (!env.IsEnvironment("Test"))
    await RoleSeeder.SeedRolesAsync(app.Services);


#endregion


app.Run();

public partial class Program { };
