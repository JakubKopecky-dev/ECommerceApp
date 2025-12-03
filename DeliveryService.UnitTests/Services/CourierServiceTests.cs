using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
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
                new() {Id = Guid.NewGuid(), Name = "DHL"},
                new() {Id = Guid.NewGuid(), Name = "UPS"}
            ];

            List<CourierDto> expectedDto =
            [
                new() {Id = couriers[0].Id, Name = couriers[0].Name},
                new() {Id = couriers[1].Id, Name = couriers[1].Name},
            ];

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(couriers);

            mapperMock
                .Setup(m => m.Map<List<CourierDto>>(couriers))
                .Returns(expectedDto);

            CourierService service = new(
                courierRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            IReadOnlyList<CourierDto> result = await service.GetAllCouriesAsync();

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<CourierDto>>(couriers), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCourierAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<Courier> couriers = [];
            List<CourierDto> expectedDto = [];

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(couriers);

            mapperMock
                .Setup(m => m.Map<List<CourierDto>>(couriers))
                .Returns(expectedDto);

            CourierService service = new(
                courierRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            IReadOnlyList<CourierDto> result = await service.GetAllCouriesAsync();

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<CourierDto>>(couriers), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourierByIdAsync_ReturnsCourierDto_WhenExists()
        {
            Guid courierId = Guid.NewGuid();

            Courier courier = new() { Id = courierId, Name = "UPS" };
            CourierDto expectedDto = new() { Id = courierId, Name = "UPS" };

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courier);

            mapperMock
                .Setup(m => m.Map<CourierDto>(courier))
                .Returns(expectedDto);

            CourierService service = new(
                courierRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            CourierDto? result = await service.GetCourierByIdAsync(courierId);

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CourierDto>(courier), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourierByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Courier?)null);

            CourierService service = new(
                courierRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            CourierDto? result = await service.GetCourierByIdAsync(courierId);

            result.Should().BeNull();

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CourierDto>(It.IsAny<Courier>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCourierAsync_ReturnsCourierDto()
        {
            CreateUpdateCourierDto createDto = new() { Name = "UPS" };

            Courier courier = new() { Id = Guid.Empty, Name = "UPS", CreatedAt = DateTime.UtcNow };
            Courier createdCourier = new() { Id = Guid.NewGuid(), Name = "UPS", CreatedAt = courier.CreatedAt };
            CourierDto expectedDto = new() { Id = createdCourier.Id, Name = "UPS", CreatedAt = createdCourier.CreatedAt };

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<Courier>(createDto))
                .Returns(courier);

            courierRepositoryMock
                .Setup(c => c.InsertAsync(courier, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCourier);

            mapperMock
                .Setup(m => m.Map<CourierDto>(createdCourier))
                .Returns(expectedDto);

            CourierService service = new(
                courierRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CourierService>>().Object
                );


            CourierDto result = await service.CreateCourierAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<Courier>(createDto), Times.Once);
            courierRepositoryMock.Verify(c => c.InsertAsync(courier, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CourierDto>(createdCourier), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCourierAsync_ReturnsCourierDto_WhenExists()
        {
            Guid courierId = Guid.NewGuid();
            CreateUpdateCourierDto updateDto = new() { Name = "UPS" };

            Courier courierDb = new() { Id = courierId, Name = "DHL", UpdatedAt = DateTime.UtcNow };
            CourierDto expectedDto = new() { Id = courierId, Name = updateDto.Name, UpdatedAt = courierDb.UpdatedAt };

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courierDb);

            mapperMock
                .Setup(m => m.Map<CreateUpdateCourierDto, Courier>(updateDto, courierDb))
                .Returns(courierDb);

            courierRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<CourierDto>(courierDb))
                .Returns(expectedDto);

            CourierService service = new(
               courierRepositoryMock.Object,
               mapperMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            CourierDto? result = await service.UpdateCourierAsync(courierId, updateDto);

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateCourierDto, Courier>(updateDto, courierDb), Times.Once);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CourierDto>(courierDb), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCourierAsync_ReturnsNull_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();
            CreateUpdateCourierDto updateDto = new() { Name = "UPS" };

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Courier?)null);

            CourierService service = new(
               courierRepositoryMock.Object,
               mapperMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            CourierDto? result = await service.UpdateCourierAsync(courierId, updateDto);

            result.Should().BeNull();

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateCourierDto, Courier>(updateDto, It.IsAny<Courier>()), Times.Never);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CourierDto>(It.IsAny<Courier>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCourierAsync_ReturnsCourierDto_WhenExists()
        {
            Guid courierId = Guid.NewGuid();

            Courier courier = new() { Id = courierId, Name = "DHL" };
            CourierDto expectedDto = new() { Id = courierId, Name = courier.Name };

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courier);

            mapperMock
                .Setup(m => m.Map<CourierDto>(courier))
                .Returns(expectedDto);

            courierRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CourierService service = new(
               courierRepositoryMock.Object,
               mapperMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            CourierDto? result = await service.DeleteCourierAsync(courierId);

            result.Should().BeEquivalentTo(expectedDto);

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CourierDto>(courier), Times.Once);
            courierRepositoryMock.Verify(c => c.Remove(courier), Times.Once);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCourierAsync_ReturnsNull_WhenNotExists()
        {
            Guid courierId = Guid.NewGuid();

            Mock<ICourierRepository> courierRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            courierRepositoryMock
                .Setup(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Courier?)null);

            CourierService service = new(
               courierRepositoryMock.Object,
               mapperMock.Object,
               new Mock<ILogger<CourierService>>().Object
               );


            CourierDto? result = await service.DeleteCourierAsync(courierId);

            result.Should().BeNull();

            courierRepositoryMock.Verify(c => c.FindByIdAsync(courierId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CourierDto>(It.IsAny<Courier>()), Times.Never);
            courierRepositoryMock.Verify(c => c.Remove(It.IsAny<Courier>()), Times.Never);
            courierRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }







    }
}
