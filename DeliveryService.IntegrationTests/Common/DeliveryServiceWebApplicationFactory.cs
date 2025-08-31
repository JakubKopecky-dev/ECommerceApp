using System.Linq;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using DeliveryService.Application.Interfaces.External;
using DeliveryService.Persistence;

namespace DeliveryService.IntegrationTests.Common
{
    public class DeliveryServiceWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public DeliveryServiceWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // remove old DbContext registrations
                var descriptors = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<DeliveryDbContext>) ||
                        d.ServiceType == typeof(DeliveryDbContext) ||
                        d.ServiceType.FullName!.Contains("IDbContextOptions"))
                    .ToList();

                foreach (var d in descriptors)
                    services.Remove(d);

                // register SQLite in-memory
                services.AddDbContext<DeliveryDbContext>(opt => opt.UseSqlite(_connection));

                // ensure DB created
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
                    db.Database.EnsureCreated();
                }

                // mock IOrderReadClient
                Mock<IOrderReadClient> orderClientMock = new();
                services.AddSingleton(orderClientMock);
                services.AddSingleton(sp => orderClientMock.Object);

                // MassTransit test harness
                services.AddMassTransitTestHarness();

                // test authentication
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = "Test";
                    o.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
        }
    }
}
