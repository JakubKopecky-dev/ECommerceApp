using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.Api.Controller;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Services;

namespace UserService.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task UserRegister_ReturnsOk_WhenSucceeded()
        {
            AuthRegisterDto registerDto = new()
            {
                Email = "user@test.com",
                Password = "P@ssw0rd",
                FirstName = "James",
                LastName = "Cook",
                Street = "Vinohradska 80",
                City = "Prague",
                PostalCode = "111",
                Country = "CZ"
            };

            AuthResponseDto expectedDto = new()
            {
                User = new UserDto { Id = Guid.NewGuid(), Email = registerDto.Email },
                Token = "jwt-token"
            };

            Mock<IAuthService> authServiceMock = new();

            authServiceMock
                .Setup(a => a.RegisterUserAsync(registerDto))
                .ReturnsAsync(expectedDto);

            AuthController controller = new(authServiceMock.Object);


            var result = await controller.UserRegister(registerDto);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            authServiceMock.Verify(a => a.RegisterUserAsync(registerDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UserRegister_ReturnsBadRequest_WhenFailed()
        {
            AuthRegisterDto registerDto = new() { Email = "fail@test.com", Password = "x" };

            Mock<IAuthService> authServiceMock = new();

            authServiceMock
                .Setup(a => a.RegisterUserAsync(registerDto))
                .ReturnsAsync((AuthResponseDto?)null);

            AuthController controller = new(authServiceMock.Object);


            var result = await controller.UserRegister(registerDto);

            result.Should().BeOfType<BadRequestResult>();

            authServiceMock.Verify(a => a.RegisterUserAsync(registerDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task LoginUser_ReturnsOk_WhenSucceeded()
        {
            AuthLoginDto loginDto = new() { Email = "user@test.com", Password = "P@ssw0rd" };

            AuthResponseDto expectedDto = new()
            {
                User = new UserDto { Id = Guid.NewGuid(), Email = loginDto.Email },
                Token = "jwt-token"
            };

            Mock<IAuthService> authServiceMock = new();

            authServiceMock
                .Setup(a => a.LoginUserAsync(loginDto))
                .ReturnsAsync(expectedDto);

            AuthController controller = new(authServiceMock.Object);


            var result = await controller.LoginUser(loginDto);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            authServiceMock.Verify(a => a.LoginUserAsync(loginDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task LoginUser_ReturnsBadRequest_WhenFailed()
        {
            AuthLoginDto loginDto = new() { Email = "user@test.com", Password = "bad" };

            Mock<IAuthService> authServiceMock = new();

            authServiceMock
                .Setup(a => a.LoginUserAsync(loginDto))
                .ReturnsAsync((AuthResponseDto?)null);

            AuthController controller = new(authServiceMock.Object);


            var result = await controller.LoginUser(loginDto);

            result.Should().BeOfType<BadRequestResult>();

            authServiceMock.Verify(a => a.LoginUserAsync(loginDto), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task Me_ReturnsOk_WhenUserExists()
        {
            ClaimsPrincipal principal = new(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())], "mock"));

            UserDto expectedDto = new() { Id = Guid.NewGuid(), Email = "me@test.com" };

            Mock<IAuthService> authServiceMock = new();

            authServiceMock
                .Setup(a => a.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(expectedDto);

            AuthController controller = new(authServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = principal }
                }
            };


            var result = await controller.Me();

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            authServiceMock.Verify(a => a.GetCurrentUserAsync(principal), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task Me_ReturnsBadRequest_WhenUserNull()
        {
            ClaimsPrincipal principal = new(new ClaimsIdentity());

            Mock<IAuthService> authServiceMock = new();

            authServiceMock
                .Setup(a => a.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((UserDto?)null);

            AuthController controller = new(authServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = principal }
                }
            };


            var result = await controller.Me();

            result.Should().BeOfType<BadRequestResult>();

            authServiceMock.Verify(a => a.GetCurrentUserAsync(principal), Times.Once);
        }



    }
}
