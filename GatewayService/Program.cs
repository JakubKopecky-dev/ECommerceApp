using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Add YARP proxy support
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


var app = builder.Build();

// Map YARP as the request handler
app.MapReverseProxy();


app.Run();
