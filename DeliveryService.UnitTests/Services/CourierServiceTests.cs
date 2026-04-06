using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DeliveryService.Application;
using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Application.Services;
using DeliveryService.Domain.Entities;
using Moq;
using FluentAssertions;

namespace DeliveryService.UnitTests.Services
{
    public class CourierServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCourierAsync_ReturnsCourierDtoList_WhenExists()
        {
            List<Courier> couriers =
            [
               Courier.Create("DHL", null, null),
               Courier.Create("UPS", null, null)
            ];

            List<CourierDto> expectedDto =
            [
                couriers[0].CourierToCourierDto(),
                couriers[1].CourierToCourierDto(),
            ];

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(couriers);

            CourierService service = new(
                courierRepositoryMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            IReadOnlyList<CourierDto> result = await service.GetAllCouriesAsync();

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCourierAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<Courier> couriers = [];
            List<CourierDto> expectedDto = [];

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(couriers);

            CourierService service = new(
                courierRepositoryMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            IReadOnlyList<CourierDto> result = await service.GetAllCouriesAsync();

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourierByIdAsync_ReturnsCourierDto_WhenExists()
        {
            Courier courier = Courier.Create("UPS", null, null);
            Guid courierId = courier.Id;

            CourierDto expectedDto = courier.CourierToCourierDto();

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courier);

            CourierService service = new(
                courierRepositoryMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            CourierDto? result = await service.GetCourierByIdAsync(courierId);

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourierByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Courier?)null);

            CourierService service = new(
                courierRepositoryMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            CourierDto? result = await service.GetCourierByIdAsync(courierId);

            result.Should().BeNull();

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCourierAsync_ReturnsCourierDto()
        {
            CreateUpdateCourierDto createDto = new() { Name = "UPS" };

            Courier courier = Courier.Create("UPS", null, null);
            CourierDto expectedDto = courier.CourierToCourierDto();

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.AddAsync(It.IsAny<Courier>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            courierRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CourierService service = new(
                courierRepositoryMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            CourierDto result = await service.CreateCourierAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedAt));

            courierRepositoryMock.Verify(c => c.AddAsync(It.IsAny<Courier>(), It.IsAny<CancellationToken>()), Times.Once);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCourierAsync_ReturnsCourierDto_WhenExists()
        {
            CreateUpdateCourierDto updateDto = new() { Name = "UPS" };

            Courier courier = Courier.Create("DHL", null, null);
            Guid courierId = courier.Id;

            CourierDto expectedDto = courier.CourierToCourierDto() with { Name = updateDto.Name };

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courier);

            courierRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CourierService service = new(
               courierRepositoryMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            CourierDto? result = await service.UpdateCourierAsync(courierId, updateDto);

            result.Should().BeEquivalentTo(expectedDto, o => o.Excluding(x => x.UpdatedAt));
            result!.UpdatedAt.Should().NotBeNull();
            result.Name.Should().Be(updateDto.Name);

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCourierAsync_ReturnsNull_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();
            CreateUpdateCourierDto updateDto = new() { Name = "UPS" };

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Courier?)null);

            CourierService service = new(
               courierRepositoryMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            CourierDto? result = await service.UpdateCourierAsync(courierId, updateDto);

            result.Should().BeNull();

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCourierAsync_ReturnsTrue_WhenExists()
        {
            Courier courier = Courier.Create("DHL", null, null);
            Guid courierId = courier.Id;

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courier);

            courierRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CourierService service = new(
               courierRepositoryMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            bool result = await service.DeleteCourierAsync(courierId);

            result.Should().BeTrue();

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            courierRepositoryMock.Verify(c => c.Remove(courier), Times.Once);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCourierAsync_ReturnsFalse_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();

            Mock<ICourierRepository> courierRepositoryMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Courier?)null);

            CourierService service = new(
               courierRepositoryMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            bool result = await service.DeleteCourierAsync(courierId);

            result.Should().BeFalse();

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            courierRepositoryMock.Verify(c => c.Remove(It.IsAny<Courier>()), Times.Never);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}