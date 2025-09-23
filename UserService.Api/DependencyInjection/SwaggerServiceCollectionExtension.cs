using Microsoft.OpenApi.Models;
using System.Reflection;

namespace UserService.Api.DependencyInjection
{
    public static class SwaggerServiceCollectionExtension
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("UserService", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "User Service API",
                    Description = "Web API for User management in E-commerce app.",
                    Contact = new OpenApiContact
                    {
                        Name = "Jakub Kopecký",
                        Url = new("https://github.com/JakubKopecky-dev")
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token in the format: Bearer <token>",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

            });

            return services;
        }
    }
}
