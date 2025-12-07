using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProductService.Api.DependencyInjection
{
    public static class AuthServiceCollectionExtension
    {
        public static IServiceCollection AddAuthenticationServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            // Load ES256 public key from config
            var publicKeyPem = configuration["Jwt:PublicKey"] ?? throw new InvalidOperationException("Missing Jwt:PublicKey");

            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(publicKeyPem);

            var securityKey = new ECDsaSecurityKey(ecdsa);


            // JWT autentizace
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = securityKey,
                    RoleClaimType = ClaimTypes.Role
                };

                // Logging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogError(context.Exception, "[AUTH ERROR] {Message}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogInformation("[AUTH SUCCESS] Token is valid");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogWarning("[AUTH CHALLENGE] {Error} - {Description}", context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });



            return services;
        }
    }
}
