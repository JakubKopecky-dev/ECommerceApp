using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Interfaces.JwtToken;
using UserService.Infrastructure.Auth;
using UserService.Infrastructure.Identity;
using UserService.Persistence;

namespace UserService.Api.DependencyInjection
{
    public static class AuthServiceCollectionExtension
    {
        public static IServiceCollection AddAuthenticationAndIdentityServiceCollection(this IServiceCollection services, IConfiguration configuration)
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


            // Identity without cookies; also registering UserManager and RoleManager
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();


            // JWT settings + token generator
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.AddScoped<IJwtTokenGenerator, JwtGenerator>();

            return services;

        }
    }
}
