using Microsoft.AspNetCore.Server.Kestrel.Core;
using PaymentService.Api.DependencyInjection;
using PaymentService.Api.Grpc.GrpcService;
using PaymentService.Api.Interfaces;
using PaymentService.Api.Middleware;
using Stripe;
using PaymentServiceService = PaymentService.Api.Services.PaymentService;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

#region Register Services (Dependency Injection)

//Authentication
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

// stripe ApiKey configuration
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Payment service registration
builder.Services.AddScoped<IPaymentService, PaymentServiceService>();

// MassTransit + RebbitMQ
builder.Services.AddMassTransitService(builder.Configuration);

//gRPC server
builder.Services.AddGrpc();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerWithJwt(builder.Environment);

#endregion


var app = builder.Build();

#region Middleware pipeline


// Swagger
if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("./PaymentService/swagger.json", "PaymentService - v1");
    });
}

// Global error handling
app.UseGlobalExceptionHandling();

// Client cancellation logging
app.UseClientCancellationLogging();

// Authentitaction and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller map
app.MapControllers();

// gRPC map service
app.MapGrpcService<PaymentGrpcService>();

#endregion


app.Run();
