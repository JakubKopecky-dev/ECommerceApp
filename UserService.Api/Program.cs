using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json.Serialization;
using UserService.Api.Auth;
using UserService.Api.DependencyInjection;
using UserService.Infrastructure;
using UserService.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP/1.1 for Swagger + Controllers
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // HTTP/2 for gRPC
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

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
builder.Services.AddSwaggerWithJwt(builder.Environment);


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
        options.SwaggerEndpoint("./UserService/swagger.json", "UserService - v1");
    });
}

// Authentitaction and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller map
app.MapControllers();

// Add roles
if (!env.IsEnvironment("Test"))
    await RoleSeeder.SeedRolesAsync(app.Services);


#endregion


app.Run();

public partial class Program { };
