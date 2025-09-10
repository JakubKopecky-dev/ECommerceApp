using System;
using System.Linq;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationService.Api.Hubs;
using NotificationService.Persistence;

namespace NotificationService.IntegrationTests.Common
{
    public class NotificationServiceWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public NotificationServiceWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registrations
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<NotificationDbContext>)
                             || d.ServiceType == typeof(NotificationDbContext)
                             || d.ServiceType.FullName!.Contains("IDbContextOptions"))
                    .ToList();

                foreach (var d in descriptors)
                {
                    services.Remove(d);
                }

                // Register SQLite in-memory DbContext
                services.AddDbContext<NotificationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // Ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                db.Database.EnsureCreated();


                // Add MassTransit Test Harness
                services.AddMassTransitTestHarness();

                // Mock IHubContext<NotificationHub>
                var hubMock = new Mock<IHubContext<NotificationHub>>();
                var clientProxyMock = new Mock<IClientProxy>();


                hubMock.Setup(h => h.Clients.User(It.IsAny<string>()))
                .Returns(clientProxyMock.Object);

                services.AddSingleton(hubMock);
                services.AddSingleton(hubMock.Object);  
                services.AddSingleton(clientProxyMock);

                // Add test authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });



             
        }





    }
}
