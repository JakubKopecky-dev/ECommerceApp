using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.Interfaces.External;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Contracts.Events;
using DeliveryServiceService = DeliveryService.Application.Services.DeliveryService;

namespace DeliveryService.UnitTests.Services
{
    public class DeliveryServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetDeliveryByOrderIdAsync_ReturnsDeliveryExtendedDto_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = Guid.NewGuid(), OrderId = orderId, Courier = courier };
            DeliveryExtendedDto expectedDto = new() { Id = delivery.Id, OrderId = orderId, Courier = new() };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            mapperMock
                .Setup(m => m.Map<DeliveryExtendedDto>(delivery))
                .Returns(expectedDto);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryExtendedDto? result = await service.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryExtendedDto>(delivery), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetDeliveryByOrderIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryExtendedDto? result = await service.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindDeliveryByOrderIdIncludeCourierAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryExtendedDto>(It.IsAny<Delivery>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateDeliveryAsync_ReturnsDeliveryDto()
        {
            CreateUpdateDeliveryDto createDto = new() { OrderId = Guid.NewGuid() };

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() {Id = Guid.Empty, OrderId = createDto.OrderId, Courier = courier , CreatedAt = DateTime.UtcNow};
            Delivery createdDelivery = new() { Id = Guid.NewGuid(), OrderId = createDto.OrderId, Courier = courier, CreatedAt = delivery.CreatedAt};
            DeliveryDto expectedDto = new() { Id = createdDelivery.Id, OrderId = createDto.OrderId, CreatedAt =  createdDelivery.CreatedAt};

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<Delivery>(createDto))
                .Returns(delivery);

            deliveryRepositoryMock
                .Setup(d => d.InsertAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDelivery);

            mapperMock
                .Setup(m => m.Map<DeliveryDto>(createdDelivery))
                .Returns(expectedDto);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto result = await service.CreateDeliveryAsync(createDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<Delivery>(createDto), Times.Once);
            deliveryRepositoryMock.Verify(d => d.InsertAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<Delivery>(createDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateDeliveryAsync_ReturnsDeliveryDto_WhenExists()
        {
            Guid deliveryId = Guid.NewGuid();
            CreateUpdateDeliveryDto updateDto = new() { OrderId = Guid.NewGuid() };

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery deliveryDb = new() { Id = deliveryId, Courier = courier, UpdatedAt = DateTime.UtcNow };
            DeliveryDto expectedDto = new() { Id = deliveryId, OrderId = updateDto.OrderId, UpdatedAt = deliveryDb.UpdatedAt };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(deliveryDb);

            mapperMock
                .Setup(m => m.Map<CreateUpdateDeliveryDto, Delivery>(updateDto, deliveryDb))
                .Returns(deliveryDb);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<DeliveryDto>(deliveryDb))
                .Returns(expectedDto);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto? result = await service.UpdateDeliveryAsync(deliveryId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateDeliveryDto, Delivery>(updateDto, deliveryDb), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(deliveryDb), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateDeliveryAsync_ReturnsNull_WhenNotExists()
        {
            Guid deliveryId = Guid.NewGuid();
            CreateUpdateDeliveryDto updateDto = new() { OrderId = Guid.NewGuid() };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            deliveryRepositoryMock
                .Setup(r => r.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto? result = await service.UpdateDeliveryAsync(deliveryId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateDeliveryDto, Delivery>(updateDto, It.IsAny<Delivery>()), Times.Never);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<DeliveryDto>(It.IsAny<Delivery>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteDeliveryAsync_ReturnsDeliveryDto_WhenExists()
        {
            Guid deliveryId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Courier = courier };
            DeliveryDto expectedDto = new() { Id = deliveryId, CourierId = courier.Id };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            mapperMock
                .Setup(m => m.Map<DeliveryDto>(delivery))
                .Returns(expectedDto);

            deliveryRepositoryMock
                .Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto? result = await service.DeleteDeliveryAsync(deliveryId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(delivery),Times.Once);
            deliveryRepositoryMock.Verify(d => d.Remove(delivery), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteDeliveryAsync_ReturnsNull_WhenNotExists()
        {
            Guid deliveryId = Guid.NewGuid();

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            deliveryRepositoryMock
                .Setup(r => r.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IOrderReadClient>().Object
            );


            DeliveryDto? result = await service.DeleteDeliveryAsync(deliveryId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(It.IsAny<Delivery>()), Times.Never);
            deliveryRepositoryMock.Verify(d => d.Remove(It.IsAny<Delivery>()), Times.Never);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ReturnsUpdated_WhenPendingToInProgress()
        {
            Guid deliveryId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Status = DeliveryStatus.Pending, OrderId = Guid.NewGuid(), Courier = courier };
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.InProgress };
            DeliveryDto expectedDto = new() { Id = deliveryId, Status = DeliveryStatus.InProgress };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            mapperMock
                .Setup(m => m.Map<DeliveryDto>(delivery))
                .Returns(expectedDto);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(delivery), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_PublishesDeliveredEvent_WhenInProgressToDelivered()
        {
            Guid deliveryId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Status = DeliveryStatus.InProgress, OrderId = orderId, Courier = courier };
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Delivered };
            DeliveryDto expectedDto = new() { Id = deliveryId, Status = DeliveryStatus.Delivered };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            mapperMock
                .Setup(m => m.Map<DeliveryDto>(delivery))
                .Returns(expectedDto);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(delivery), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_PublishesCanceledEvent_WhenPendingToCanceled_AndUserExists()
        {
            Guid deliveryId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Status = DeliveryStatus.Pending, OrderId = orderId, Courier = courier };
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Canceled };
            DeliveryDto expectedDto = new() { Id = deliveryId, Status = DeliveryStatus.Canceled };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            orderReadClientMock
                .Setup(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            mapperMock
                .Setup(m => m.Map<DeliveryDto>(delivery))
                .Returns(expectedDto);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(delivery), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_PublishesCanceledEvent_WhenInProgressToCanceled_AndUserExists()
        {
            Guid deliveryId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Status = DeliveryStatus.InProgress, OrderId = orderId, Courier = courier };
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Canceled };
            DeliveryDto expectedDto = new() { Id = deliveryId, Status = DeliveryStatus.Canceled };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            orderReadClientMock
                .Setup(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            mapperMock
                .Setup(m => m.Map<DeliveryDto>(delivery))
                .Returns(expectedDto);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(delivery), Times.Once);
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
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Delivery?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<DeliveryDto>(It.IsAny<Delivery>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ReturnsNull_WhenInvalidTransition_FromPendingToDelivered()
        {
            Guid deliveryId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Status = DeliveryStatus.Pending, OrderId = Guid.NewGuid(), Courier = courier };
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Delivered };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<DeliveryDto>(It.IsAny<Delivery>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ReturnsNull_WhenInvalidTransition_FromDeliveredToInProgress()
        {
            Guid deliveryId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Status = DeliveryStatus.Delivered, OrderId = Guid.NewGuid(), Courier = courier };
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.InProgress };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<DeliveryDto>(It.IsAny<Delivery>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatusAsync_ReturnsNull_WhenCanceled_AndUserNotExists()
        {
            Guid deliveryId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();

            Courier courier = new() { Id = Guid.NewGuid(), Name = "DHL" };
            Delivery delivery = new() { Id = deliveryId, Status = DeliveryStatus.Pending, OrderId = orderId, Courier = courier };
            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Canceled };

            Mock<IDeliveryRepository> deliveryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IOrderReadClient> orderReadClientMock = new();

            deliveryRepositoryMock
                .Setup(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(delivery);

            orderReadClientMock
                .Setup(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid?)null);

            DeliveryServiceService service = new(
                deliveryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<DeliveryServiceService>>().Object,
                publishEndpointMock.Object,
                orderReadClientMock.Object
            );


            DeliveryDto? result = await service.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            deliveryRepositoryMock.Verify(d => d.FindByIdAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
            deliveryRepositoryMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            orderReadClientMock.Verify(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DeliveryDto>(It.IsAny<Delivery>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryDeliveredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }



    }
}
