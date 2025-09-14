using PaymentService.Api.DependencyInjection;
using Stripe;
using PaymentService.Api.Middleware;
using PaymentService.Api.Grpc.GrpcService;
using PaymentService.Api.Interfaces;
using PaymentServiceService = PaymentService.Api.Services.PaymentService;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSwaggerWithJwt();

#endregion


var app = builder.Build();

#region Middleware pipeline


// Swagger
if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/PaymentService/swagger.json", "PaymentService - v1");
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
