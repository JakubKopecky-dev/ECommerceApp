using DeliveryService.Application;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.Interfaces.External;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Domain.Common;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using DeliveryService.Domain.ValueObjects;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Contracts.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using DeliveryServiceService = DeliveryService.Application.Services.DeliveryService;

namespace DeliveryService.UnitTests.Services
{
    public class DeliveryServiceTests
    {

        private static Courier MockCourier() => Courier.Create("DHL", null, null);


        private static Address MockAddress() => new("Nusle 50", "Prague 4", "140 00", "Czech Republic");
        private static Delivery MockDelivery(Guid orderId, Guid courierId) =>
            Delivery.Create(orderId, courierId,new Email("aa@b.com"), "Petr", "Novak", "123456789", MockAddress(), null);


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetDeliveryByOrderIdAsync_ReturnsDeliveryExtendedDto_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();

            Delivery delivery = MockDelivery(orderId, courier.Id);


            typeof(Delivery)
            .GetProperty(nameof(Delivery.Courier))!
            .SetValue(delivery, courier);

            DeliveryExtendedDto expectedDto = delivery.DeliveryToDeliveryExtendedDto();

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryExtendedDto? result = await service.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetDeliveryByOrderIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryExtendedDto? result = await service.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateDeliveryAsync_ReturnsDeliveryDto()
        {
            Courier courier = MockCourier();
            CreateUpdateDeliveryDto createDto = new()
            {
                OrderId = Guid.NewGuid(),
                CourierId = courier.Id,
                Email = "aa@b.com",
                FirstName = "Petr",
                LastName = "Novak",
                PhoneNumber = "123456789",
                Street = "Nusle 50",
                City = "Prague 4",
                PostalCode = "140 00",
                State = "Czech Republic"
            };
            Delivery delivery = MockDelivery(createDto.OrderId, courier.Id);
            DeliveryDto expectedDto = delivery.DeliveryToDeliveryDto();

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(d => d.AddAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto result = await service.CreateDeliveryAsync(createDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedAt));

            deliveryRepositoryMock.Verify(d => d.AddAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateDeliveryAsync_ReturnsDeliveryDto_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();

            CreateUpdateDeliveryDto updateDto = new()
            {
                OrderId = orderId,
                CourierId = courier.Id,
                Email = "aa@b.com",
                FirstName = "Jan",
                LastName = "Novak",
                PhoneNumber = "123456789",
                Street = "Nusle 50",
                City = "Prague 4",
                PostalCode = "140 00",
                State = "Czech Republic",
            };

            Delivery delivery = MockDelivery(orderId, courier.Id);
            DeliveryDto expectedDto = delivery.DeliveryToDeliveryDto() with { FirstName = updateDto.FirstName };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);



            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto? result = await service.UpdateDeliveryAsync(delivery.Id, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.UpdatedAt));
            result!.UpdatedAt.Should().NotBeNull();
            result.FirstName.Should().Be(updateDto.FirstName);

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateDeliveryAsync_ReturnsNull_WhenNotExists()
        {
            Guid deliveryId = Guid.NewGuid();
            CreateUpdateDeliveryDto updateDto = new() { OrderId = Guid.NewGuid() };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(r => r.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto? result = await service.UpdateDeliveryAsync(deliveryId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteDeliveryAsync_ReturnsTrue_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);


            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            bool result = await service.DeleteDeliveryAsync(delivery.Id, It.IsAny<CancellationToken>());

            result.Should().BeTrue();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.Remove(delivery), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteDeliveryAsync_ReturnsFalse_WhenNotExists()
        {
            Guid deliveryId = Guid.NewGuid();

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(r => r.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            bool result = await service.DeleteDeliveryAsync(deliveryId, It.IsAny<CancellationToken>());

            result.Should().BeFalse();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.Remove(It.IsAny<Delivery>()), Times.Never);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ReturnsUpdated_WhenPendingToInProgress()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.InProgress };
            DeliveryDto expectedDto = delivery.DeliveryToDeliveryDto() with { Status = DeliveryStatus.InProgress };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(delivery.Id, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.UpdatedAt));
            result!.UpdatedAt.Should().NotBeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_PublishesDeliveredEvent_WhenInProgressToDelivered()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);
            delivery.ChangeStatus(DeliveryStatus.InProgress);
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Delivered };
            DeliveryDto expectedDto = delivery.DeliveryToDeliveryDto() with { Status = DeliveryStatus.Delivered };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(delivery.Id, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.UpdatedAt).Excluding(x => x.DeliveredAt));
            result!.UpdatedAt.Should().NotBeNull();
            result.DeliveredAt.Should().NotBeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_PublishesCanceledEvent_WhenPendingToCanceled_AndUserExists()
        {
            Guid orderId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Canceled };
            DeliveryDto expectedDto = delivery.DeliveryToDeliveryDto() with { Status = DeliveryStatus.Canceled };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderReadClientMock
                .Setup(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(delivery.Id, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.UpdatedAt));
            result!.UpdatedAt.Should().NotBeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_PublishesCanceledEvent_WhenInProgressToCanceled_AndUserExists()
        {
            Guid orderId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);
            delivery.ChangeStatus(DeliveryStatus.InProgress);
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Canceled };
            DeliveryDto expectedDto = delivery.DeliveryToDeliveryDto() with { Status = DeliveryStatus.Canceled };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderReadClientMock
                .Setup(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(delivery.Id, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.UpdatedAt));
            result!.UpdatedAt.Should().NotBeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ReturnsNull_WhenDeliveryNotExists()
        {
            Guid deliveryId = Guid.NewGuid();
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.InProgress };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ThrowsDomainException_WhenInvalidTransition_FromPendingToDelivered()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Delivered };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            await service.Invoking(s => s.ChangeDeliveryStatusAsync(delivery.Id, changeDto, It.IsAny<CancellationToken>()))
                .Should().ThrowAsync<DomainException>();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ThrowsDomainException_WhenInvalidTransition_FromDeliveredToInProgress()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);
            delivery.ChangeStatus(DeliveryStatus.InProgress);
            delivery.ChangeStatus(DeliveryStatus.Delivered);
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.InProgress };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            await service.Invoking(s => s.ChangeDeliveryStatusAsync(delivery.Id, changeDto, It.IsAny<CancellationToken>()))
                .Should().ThrowAsync<DomainException>();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ReturnsNull_WhenCanceled_AndUserNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = MockCourier();
            Delivery delivery = MockDelivery(orderId, courier.Id);
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Canceled };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderReadClientMock
                .Setup(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(delivery.Id, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(delivery.Id, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}