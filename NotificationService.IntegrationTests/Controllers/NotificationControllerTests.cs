using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Enum;
using NotificationService.IntegrationTests.Common;
using NotificationService.Persistence;

namespace NotificationService.IntegrationTests.Controllers
{
    public class NotificationControllerTests(NotificationServiceWebApplicationFactory factory) :  IClassFixture<NotificationServiceWebApplicationFactory>
    {
        private readonly NotificationServiceWebApplicationFactory _factory = factory;



        private HttpClient CreateUserClient(Guid userId)
        {
            TestAuthHandler.FixedUserId = userId;
            TestAuthHandler.FixedRoles = new() { "User" };

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            return client;
        }



        private HttpClient CreateAdminClient(Guid userId)
        {
            TestAuthHandler.FixedUserId = userId;
            TestAuthHandler.FixedRoles = new() { "User", "Admin" };

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            return client;
        }



        [Fact]
        public async Task GetAllUserNotifications_ReturnsEmpty_WhenNoneExist()
        {
            var userId = Guid.NewGuid();
            var client = CreateUserClient(userId);

            var response = await client.GetAsync("api/Notification");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var notifications = await response.Content.ReadFromJsonAsync<IReadOnlyList<NotificationDto>>();
            notifications.Should().NotBeNull();
            notifications.Should().BeEmpty();
        }



        [Fact]
        public async Task GetNotification_ReturnsNotFound_WhenDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var client = CreateUserClient(userId);

            var response = await client.GetAsync($"api/Notification/detail/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        public async Task CreateNotification_ReturnsCreated_WhenAdmin()
        {
            var client = CreateAdminClient(Guid.NewGuid());

            CreateNofiticationDto request = new()
            {
                UserId = Guid.NewGuid(),
                Title = "Order created",
                Message = "Order is created",
                CreatedAt = DateTime.UtcNow,
                Type = NotificationType.OrderCreated
            };

            var response = await client.PostAsJsonAsync("api/Notification", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<NotificationDto>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            created.Should().NotBeNull();
            created!.Title.Should().Be("Order created");
            created.Message.Should().Be("Order is created");
            created.Type.Should().Be(NotificationType.OrderCreated);

            // Verify persisted
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var entity = await db.Notifications.FindAsync(created.Id);
            entity.Should().NotBeNull();
            entity!.Title.Should().Be("Order created");
        }




    }
}
