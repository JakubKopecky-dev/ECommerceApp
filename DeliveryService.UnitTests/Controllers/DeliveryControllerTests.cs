using System;
using System.Threading;
using System.Threading.Tasks;
using DeliveryService.Api.Controllers;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DeliveryService.UnitTests.Controllers
{
    public class DeliveryControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetDelivery_ReturnsOk_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            DeliveryExtendedDto expectedDto = new() { Id = Guid.NewGuid(), OrderId = orderId, Courier = new() };

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(d => d.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.GetDelivery(orderId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            deliveryServiceMock.Verify(d => d.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetDelivery_ReturnsNotFound_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(d => d.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryExtendedDto?)null);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.GetDelivery(orderId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            deliveryServiceMock.Verify(d => d.GetDeliveryByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateDelivery_ReturnsCreatedAtAction_WithDeliveryDto()
        {
            CreateUpdateDeliveryDto createDto = new() { OrderId = Guid.NewGuid(), CourierId = Guid.NewGuid() };

            DeliveryDto expectedDto = new() { Id = Guid.NewGuid(), OrderId = createDto.OrderId, CourierId = createDto.CourierId };

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(d => d.CreateDeliveryAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.CreateDelivery(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(DeliveryController.GetDelivery));
            createdResult.RouteValues!["orderId"].Should().Be(expectedDto.OrderId);
            createdResult.Value.Should().BeEquivalentTo(expectedDto);

            deliveryServiceMock.Verify(d => d.CreateDeliveryAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateDelivery_ReturnsOk_WhenExists()
        {
            Guid deliveryId = Guid.NewGuid();

            CreateUpdateDeliveryDto updateDto = new() { OrderId = Guid.NewGuid(), CourierId = Guid.NewGuid() };
            DeliveryDto expectedDto = new() { Id = deliveryId, OrderId = updateDto.OrderId, CourierId = updateDto.CourierId };

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(s => s.UpdateDeliveryAsync(deliveryId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.UpdateDelivery(deliveryId, updateDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            deliveryServiceMock.Verify(s => s.UpdateDeliveryAsync(deliveryId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateDelivery_ReturnsNotFound_WhenNotExists()
        {
            Guid deliveryId = Guid.NewGuid();

            CreateUpdateDeliveryDto updateDto = new() { OrderId = Guid.NewGuid(), CourierId = Guid.NewGuid() };

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(s => s.UpdateDeliveryAsync(deliveryId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryDto?)null);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.UpdateDelivery(deliveryId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            deliveryServiceMock.Verify(s => s.UpdateDeliveryAsync(deliveryId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteDelivery_ReturnsOk_WhenExists()
        {
            Guid deliveryId = Guid.NewGuid();

            DeliveryDto expectedDto = new() { Id = deliveryId, OrderId = Guid.NewGuid(), CourierId = Guid.NewGuid() };

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(s => s.DeleteDeliveryAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.DeleteDelivery(deliveryId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            deliveryServiceMock.Verify(s => s.DeleteDeliveryAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteDelivery_ReturnsNotFound_WhenNotExists()
        {
            Guid deliveryId = Guid.NewGuid();

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(s => s.DeleteDeliveryAsync(deliveryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryDto?)null);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.DeleteDelivery(deliveryId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            deliveryServiceMock.Verify(s => s.DeleteDeliveryAsync(deliveryId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatus_ReturnsOk_WhenExists()
        {
            Guid deliveryId = Guid.NewGuid();

            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.InProgress };
            DeliveryDto expectedDto = new() { Id = deliveryId, Status = DeliveryStatus.InProgress };

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(s => s.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.ChangeDeliveryStatus(deliveryId, changeDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            deliveryServiceMock.Verify(s => s.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeDeliveryStatus_ReturnsNotFound_WhenNotExists()
        {
            Guid deliveryId = Guid.NewGuid();

            ChangeDeliveryStatusDto changeDto = new() { Status = DeliveryStatus.Canceled };

            Mock<IDeliveryService> deliveryServiceMock = new();

            deliveryServiceMock
                .Setup(s => s.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryDto?)null);

            DeliveryController controller = new(deliveryServiceMock.Object);


            var result = await controller.ChangeDeliveryStatus(deliveryId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            deliveryServiceMock.Verify(s => s.ChangeDeliveryStatusAsync(deliveryId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
