using Aspire.Hosting;
using ECommerceApp.AppHost;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);


string orderServiceIntern = "http://orderservice:8081";
string productServiceIntern = "http://productservice:8081";
string deliveryServiceIntern = "http://deliveryservice:8081";
string paymentServiceIntern = "http://paymentservice:8081";


string cartDb = "Server=sqldb;Database=ECommerceApp_CartDb;User Id=sa;Password=ECom2025!Pass;TrustServerCertificate=True;";
string deliveryDb = "Server=sqldb;Database=ECommerceApp_DeliveryDb;User Id=sa;Password=ECom2025!Pass;TrustServerCertificate=True;";
string notificationDb = "Server=sqldb;Database=ECommerceApp_NotificationDb;User Id=sa;Password=ECom2025!Pass;TrustServerCertificate=True;";
string orderDb = "Server=sqldb;Database=ECommerceApp_OrderDb;User Id=sa;Password=ECom2025!Pass;TrustServerCertificate=True;";
string productDb = "Server=sqldb;Database=ECommerceApp_ProductDb;User Id=sa;Password=ECom2025!Pass;TrustServerCertificate=True;";
string userDb = "Server=sqldb;Database=ECommerceApp_UserDb;User Id=sa;Password=ECom2025!Pass;TrustServerCertificate=True;";


// SQL Server
var sql = builder.AddSqlServer("sqldb")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "ECom2025!Pass")
    .WithVolume("sql-data", "/var/opt/mssql")
    .WithEndpoint("sql", e =>
    {
        e.Port = 1433;
        e.TargetPort = 0; 
    })
    .WithLifetime(ContainerLifetime.Persistent);

// RabbitMQ
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
    .WithManagementPlugin()
    .WithEndpoint("amqp", e =>
    {
        e.Port = 5672;
        e.TargetPort = 5672;
    })
    .WithEndpoint("ui", e =>
    {
        e.Port = 15672;
        e.TargetPort = 15672;
    });


// Microservices
var cartServices = builder.AddProject<CartService_Api>("cartservice")
    .WithJwtEnv()
    .WithRabbitEnv()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithEndpoint("grpc", e => e.Port = 8081)
    .WithEnvironment("ConnectionStrings__DefaultConnection", cartDb)
    .WithEnvironment("OrderService__GrpcAddress", orderServiceIntern)
    .WithEnvironment("ProductService__GrpcAddress", productServiceIntern)
    .WithEnvironment("EnableSwagger", "true")
    .WithReference(sql)
    .WithReference(rabbitmq);

var deliveryService = builder.AddProject<DeliveryService_Api>("deliveryservice")
    .WithJwtEnv()
    .WithRabbitEnv()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithEndpoint("grpc", e => e.Port = 8081)
    .WithEnvironment("ConnectionStrings__DefaultConnection", deliveryDb)
    .WithEnvironment("OrderService__GrpcAddress", orderServiceIntern)
    .WithEnvironment("EnableSwagger", "true")
    .WithReference(sql)
    .WithReference(rabbitmq);

var notificationService = builder.AddProject<NotificationService_Api>("notificationservice")
    .WithJwtEnv()
    .WithRabbitEnv()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithEndpoint("grpc", e => e.Port = 8081)
    .WithEnvironment("ConnectionStrings__DefaultConnection", notificationDb)
    .WithEnvironment("EnableSwagger", "true")
    .WithReference(sql)
    .WithReference(rabbitmq);


var orderService = builder.AddProject<OrderService_Api>("orderservice")
    .WithJwtEnv()
    .WithRabbitEnv()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithEndpoint("grpc", e => e.Port = 8081)
    .WithEnvironment("ConnectionStrings__DefaultConnection", orderDb)
    .WithEnvironment("DeliveryService__GrpcAddress", deliveryServiceIntern)
    .WithEnvironment("PaymentService__GrpcAddress", paymentServiceIntern)
    .WithEnvironment("EnableSwagger", "true")
    .WithReference(sql)
    .WithReference(rabbitmq);


var paymentService = builder.AddProject<PaymentService_Api>("paymentservice")
    .WithJwtEnv()
    .WithRabbitEnv()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithEndpoint("grpc", e => e.Port = 8081)
    .WithEnvironment("Stripe__SecretKey", "sk_test_yourSecret")
    .WithEnvironment("Stripe__WebHookSecret", "whsec_yourSercret")
    .WithEnvironment("Stripe__SuccessUrl", "http://paymentservice:8080/api/Payment/success")
    .WithEnvironment("Stripe__CancelUrl", "http://paymentservice:8080/api/Payment/cancel")
    .WithEnvironment("EnableSwagger", "true")
    .WithReference(rabbitmq);


var productService = builder.AddProject<ProductService_Api>("productservice")
    .WithJwtEnv()
    .WithRabbitEnv()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithEndpoint("grpc", e => e.Port = 8081)
    .WithEnvironment("ConnectionStrings__DefaultConnection", productDb)
    .WithEnvironment("EnableSwagger", "true")
    .WithReference(sql)
    .WithReference(rabbitmq);


var userService = builder.AddProject<UserService_Api>("userservice")
    .WithJwtEnv()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithEndpoint("grpc", e => e.Port = 8081)
    .WithEnvironment("Jwt__ExpiresInMinutes", "600")
    .WithEnvironment("ConnectionStrings__DefaultConnection", userDb)
    .WithEnvironment("EnableSwagger", "true")
    .WithReference(sql);


var gatewayService = builder.AddProject<GatewayService>("gatewayservice")
    .WithEndpoint("http", e => e.Port = 8080)
    .WithReference(cartServices)
    .WithReference(orderService)
    .WithReference(productService)
    .WithReference(deliveryService)
    .WithReference(notificationService)
    .WithReference(paymentService)
    .WithReference(userService);




builder.Build().Run();
