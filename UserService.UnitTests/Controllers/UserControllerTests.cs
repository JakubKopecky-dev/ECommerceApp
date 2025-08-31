using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.Api.Controller;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Services;

namespace UserService.UnitTests.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllUsers_ReturnsUserDtoList_WhenExists()
        {
            List<UserDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), Email = "a@test.com" },
                new() { Id = Guid.NewGuid(), Email = "b@test.com" }
            ];

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            UserController controller = new(userServiceMock.Object);


            IReadOnlyList<UserDto> result = await controller.GetAllUsers(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            userServiceMock.Verify(u => u.GetAllUsersAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUser_ReturnsOk_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            UserDto expectedDto = new() { Id = userId, Email = "user@test.com" };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.GetUserAsync(userId))
                .ReturnsAsync(expectedDto);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.GetUser(userId);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            userServiceMock.Verify(u => u.GetUserAsync(userId), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUser_ReturnsNotFound_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.GetUserAsync(userId))
                .ReturnsAsync((UserDto?)null);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.GetUser(userId);

            result.Should().BeOfType<NotFoundResult>();

            userServiceMock.Verify(u => u.GetUserAsync(userId), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUser_ReturnsOk_WhenSucceeded()
        {
            CreateUserDto createDto = new() { Email = "new@test.com", Password = "P@ssw0rd" };
            UserDto expectedDto = new() { Id = Guid.NewGuid(), Email = createDto.Email };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.CreateUserAsync(createDto))
                .ReturnsAsync(expectedDto);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.CreateUser(createDto);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            userServiceMock.Verify(u => u.CreateUserAsync(createDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUser_ReturnsBadRequest_WhenFailed()
        {
            CreateUserDto createDto = new() { Email = "fail@test.com", Password = "123" };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.CreateUserAsync(createDto))
                .ReturnsAsync((UserDto?)null);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.CreateUser(createDto);

            result.Should().BeOfType<BadRequestResult>();

            userServiceMock.Verify(u => u.CreateUserAsync(createDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUser_ReturnsOk_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            UpdateUserDto updateDto = new() { FirstName = "John", LastName = "Doe" };
            UserDto expectedDto = new() { Id = userId, FirstName = updateDto.FirstName, LastName = updateDto.LastName };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.UpdateUserAsync(userId, updateDto))
                .ReturnsAsync(expectedDto);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.UpdateUser(userId, updateDto);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            userServiceMock.Verify(u => u.UpdateUserAsync(userId, updateDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUser_ReturnsNotFound_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            UpdateUserDto updateDto = new() { FirstName = "John" };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.UpdateUserAsync(userId, updateDto))
                .ReturnsAsync((UserDto?)null);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.UpdateUser(userId, updateDto);

            result.Should().BeOfType<NotFoundResult>();

            userServiceMock.Verify(u => u.UpdateUserAsync(userId, updateDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUser_ReturnsOk_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            UserDto expectedDto = new() { Id = userId, Email = "delete@test.com" };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.DeleteUserAsync(userId))
                .ReturnsAsync(expectedDto);

            UserController controller = new(userServiceMock.Object);

                
            var result = await controller.DeleteUser(userId);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            userServiceMock.Verify(u => u.DeleteUserAsync(userId), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUser_ReturnsNotFound_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.DeleteUserAsync(userId))
                .ReturnsAsync((UserDto?)null);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.DeleteUser(userId);

            result.Should().BeOfType<NotFoundResult>();

            userServiceMock.Verify(u => u.DeleteUserAsync(userId), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeIsAdminAsync_ReturnsOk_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            ChangeIsAdminDto changeDto = new() { IsAdmin = true };
            UserDto expectedDto = new() { Id = userId, Email = "admin@test.com", IsAdmin = true };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.ChangeIsAdminAsync(userId, changeDto))
                .ReturnsAsync(expectedDto);

            UserController controller = new(userServiceMock.Object);


            var  result = await controller.ChangeIsAdminAsync(userId, changeDto);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            userServiceMock.Verify(u => u.ChangeIsAdminAsync(userId, changeDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeIsAdminAsync_ReturnsNotFound_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            ChangeIsAdminDto changeDto = new() { IsAdmin = true };

            Mock<IApplicationUserService> userServiceMock = new();

            userServiceMock
                .Setup(u => u.ChangeIsAdminAsync(userId, changeDto))
                .ReturnsAsync((UserDto?)null);

            UserController controller = new(userServiceMock.Object);


            var result = await controller.ChangeIsAdminAsync(userId, changeDto);

            result.Should().BeOfType<NotFoundResult>();

            userServiceMock.Verify(u => u.ChangeIsAdminAsync(userId, changeDto), Times.Once);
        }



    }
}
