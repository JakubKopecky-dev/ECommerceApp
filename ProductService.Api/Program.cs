using ProductService.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region Register services (Dependency Injection)

// Persistence (DbContext, Repository)
builder.Services.AddPersistenceServices(builder.Configuration);

#endregion





var app = builder.Build();

#region Middleware pipeline


#endregion

app.Run();

public partial class Program { };
