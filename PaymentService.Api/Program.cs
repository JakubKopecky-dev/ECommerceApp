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
builder.Services.AddOpenApiWithJwt();



var app = builder.Build();


// Swagger
if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";

        if (app.Environment.IsDevelopment())
            options.SwaggerEndpoint("/openapi/v1.json", "PaymentService - v1");
        else
            options.SwaggerEndpoint("/payment/openapi/v1.json", "PaymentService - v1");
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



app.Run();
