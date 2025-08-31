using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Api.Controllers;
using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DeliveryService.UnitTests.Controllers
{
    public class CourierControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCouriers_ReturnsCourierDtoList_WhenExists()
        {
            IReadOnlyList<CourierDto> expectedDto =
            [
                new() {Id = Guid.NewGuid(), Name = "UPS"},
                new() {Id = Guid.NewGuid(), Name = "DHL"}
            ];

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.GetAllCouriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CourierController controller = new(courierServiceMock.Object);


            IReadOnlyList<CourierDto> result = await controller.GetAllCouriers(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);
            courierServiceMock.Verify(c => c.GetAllCouriesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCouriers_ReturnsEmptyList_WhenNotExists()
        {
            IReadOnlyList<CourierDto> expectedDto = [];

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.GetAllCouriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CourierController controller = new(courierServiceMock.Object);


            IReadOnlyList<CourierDto> result = await controller.GetAllCouriers(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            courierServiceMock.Verify(c => c.GetAllCouriesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourier_ReturnsOk_WhenExists()
        {
            Guid courierId = Guid.NewGuid();

            CourierDto expectedDto = new() { Id = courierId, Name = "UPS" };

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.GetCourierByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CourierController controller = new(courierServiceMock.Object);


            var result = await controller.GetCourier(courierId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            courierServiceMock.Verify(c => c.GetCourierByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourier_ReturnsNotFound_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.GetCourierByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourierDto?)null);

            CourierController controller = new(courierServiceMock.Object);


            var result = await controller.GetCourier(courierId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            courierServiceMock.Verify(c => c.GetCourierByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCourier_ReturnsCreatedAtAction_WithCourierDto()
        {
            CreateUpdateCourierDto createDto = new() { Name = "DHL" };

            CourierDto expectedDto = new() { Id = Guid.NewGuid(), Name = "DHL" };

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.CreateCourierAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CourierController controller = new(courierServiceMock.Object);


            var result = await controller.CreateCourier(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(CourierController.GetCourier));
            createdResult.RouteValues!["courierId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().Be(expectedDto);

            courierServiceMock.Verify(c => c.CreateCourierAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCourier_ReturnsOk_WhenExists()
        {
            Guid courierId = Guid.NewGuid();

            CreateUpdateCourierDto updateDto = new() { Name = "UPS" };
            CourierDto expectedDto = new() { Id = courierId, Name = updateDto.Name };

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.UpdateCourierAsync(courierId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CourierController controller = new(courierServiceMock.Object);


            var result = await controller.UpdateCourier(courierId, updateDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            courierServiceMock.Verify(c => c.UpdateCourierAsync(courierId,updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCourier_ReturnsNoFound_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();
            CreateUpdateCourierDto updateDto = new() { Name = "UPS" };

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.UpdateCourierAsync(courierId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourierDto?)null);

            CourierController controller = new(courierServiceMock.Object);


            var result = await controller.UpdateCourier(courierId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            courierServiceMock.Verify(c => c.UpdateCourierAsync(courierId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCourier_ReturnsOk_WhenExists()
        {
            Guid courierId = Guid.NewGuid();

            CourierDto expectedDto = new() { Id = courierId, Name = "DHL" };

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.DeleteCourierAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CourierController controller = new(courierServiceMock.Object);


            var result = await controller.DeleteCourier(courierId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            courierServiceMock.Verify(c => c.DeleteCourierAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCourier_ReturnsNotFound_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();

            Mock<ICourierService> courierServiceMock = new();

            courierServiceMock
                .Setup(c => c.DeleteCourierAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourierDto?)null);

            CourierController controller = new(courierServiceMock.Object);


            var result = await controller.DeleteCourier(courierId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            courierServiceMock.Verify(c => c.DeleteCourierAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
