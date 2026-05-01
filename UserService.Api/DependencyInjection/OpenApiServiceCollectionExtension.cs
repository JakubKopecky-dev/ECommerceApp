using Microsoft.OpenApi;


namespace UserService.Api.DependencyInjection
{
    public static class OpenApiServiceCollectionExtension
    {
        public static IServiceCollection AddOpenApiWithJwt(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "User Service API",
                        Version = "v1",
                        Description = "Web API for User management in E-commerce app.",
                        Contact = new() { Name = "Jakub Kopecký", Url = new("https://github.com/JakubKopecky-dev") }
                    };
                    document.Components ??= new();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter JWT token (without 'Bearer' prefix, Swagger adds it automatically)"
                    };
                    return Task.CompletedTask;
                });

                options.AddOperationTransformer((operation, context, cancellationToken) =>
                {
                    operation.Security ??= [];

                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
                    });

                    return Task.CompletedTask;
                });
            });
            return services;
        }
    }
}