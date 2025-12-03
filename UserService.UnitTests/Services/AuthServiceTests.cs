using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.JwtToken;
using UserService.Infrastructure.Identity;
using UserService.Infrastructure.Services;
using UserService.Domain.Enums;

namespace UserService.UnitTests.Services
{
    public class AuthServiceTests
    {
        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                Array.Empty<IUserValidator<ApplicationUser>>(),
                Array.Empty<IPasswordValidator<ApplicationUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object
            );
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterUserAsync_ReturnsAuthResponse_WhenCreateSucceeded()
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

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IJwtTokenGenerator> tokenGenMock = new();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.User))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync([UserRoles.User]);

            tokenGenMock
                .Setup(t => t.GenerateToken(It.IsAny<Guid>(), registerDto.Email, registerDto.Email, It.IsAny<IEnumerable<string>>()))
                .Returns("jwt-token");

            mapperMock
                .Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()))
                .Returns(new UserDto { Id = Guid.NewGuid(), Email = registerDto.Email });

            AuthService service = new(
                userManagerMock.Object,
                tokenGenMock.Object,
                mapperMock.Object,
                new Mock<ILogger<AuthService>>().Object
            );


            AuthResponseDto? result = await service.RegisterUserAsync(registerDto);

            result.Should().NotBeNull();
            result.Token.Should().Be("jwt-token");
            result.User.Email.Should().Be(registerDto.Email);

            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.User), Times.Once);
            userManagerMock.Verify(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Once);
            tokenGenMock.Verify(t => t.GenerateToken(It.IsAny<Guid>(), registerDto.Email, registerDto.Email, It.IsAny<IEnumerable<string>>()), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterUserAsync_ReturnsNull_WhenCreateFails()
        {
            AuthRegisterDto registerDto = new() { Email = "x@test.com", Password = "a" };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IJwtTokenGenerator> tokenGenMock = new();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "fail" }));

            AuthService service = new(
                userManagerMock.Object,
                tokenGenMock.Object,
                mapperMock.Object,
                new Mock<ILogger<AuthService>>().Object
            );


            AuthResponseDto? result = await service.RegisterUserAsync(registerDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            tokenGenMock.Verify(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task LoginUserAsync_ReturnsAuthResponse_WhenSuccess()
        {
            AuthLoginDto loginDto = new() { Email = "user@test.com", Password = "P@ssw0rd" };
            ApplicationUser user = new() { Id = Guid.NewGuid(), Email = loginDto.Email, UserName = loginDto.Email };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IJwtTokenGenerator> tokenGenMock = new();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(u => u.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);

            userManagerMock
                .Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync([UserRoles.User]);

            tokenGenMock
                .Setup(t => t.GenerateToken(user.Id, user.Email!, user.UserName!, It.IsAny<IEnumerable<string>>()))
                .Returns("jwt-token");

            mapperMock
                .Setup(m => m.Map<UserDto>(user))
                .Returns(new UserDto { Id = user.Id, Email = user.Email });

            AuthService service = new(
                userManagerMock.Object,
                tokenGenMock.Object,
                mapperMock.Object,
                new Mock<ILogger<AuthService>>().Object
            );


            AuthResponseDto? result = await service.LoginUserAsync(loginDto);

            result.Should().NotBeNull();
            result!.Token.Should().Be("jwt-token");
            result.User.Email.Should().Be(loginDto.Email);

            userManagerMock.Verify(u => u.FindByEmailAsync(loginDto.Email), Times.Once);
            userManagerMock.Verify(u => u.CheckPasswordAsync(user, loginDto.Password), Times.Once);
            userManagerMock.Verify(u => u.GetRolesAsync(user), Times.Once);
            tokenGenMock.Verify(t => t.GenerateToken(user.Id, user.Email!, user.UserName!, It.IsAny<IEnumerable<string>>()), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task LoginUserAsync_ReturnsNull_WhenUserNotFound()
        {
            AuthLoginDto loginDto = new() { Email = "missing@test.com", Password = "x" };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IJwtTokenGenerator> tokenGenMock = new();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApplicationUser?)null);

            AuthService service = new(
                userManagerMock.Object,
                tokenGenMock.Object,
                mapperMock.Object,
                new Mock<ILogger<AuthService>>().Object
            );


            AuthResponseDto? result = await service.LoginUserAsync(loginDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            tokenGenMock.Verify(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task LoginUserAsync_ReturnsNull_WhenPasswordInvalid()
        {
            AuthLoginDto loginDto = new() { Email = "user@test.com", Password = "wrong" };
            ApplicationUser user = new() { Id = Guid.NewGuid(), Email = loginDto.Email, UserName = loginDto.Email };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IJwtTokenGenerator> tokenGenMock = new();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(u => u.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(false);

            AuthService service = new(
                userManagerMock.Object,
                tokenGenMock.Object,
                mapperMock.Object,
                new Mock<ILogger<AuthService>>().Object
            );

            AuthResponseDto? result = await service.LoginUserAsync(loginDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Never);
            tokenGenMock.Verify(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCurrentUserAsync_ReturnsUserDto_WhenExists()
        {
            ClaimsPrincipal principal = new(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())], "mock"));
            ApplicationUser user = new() { Id = Guid.NewGuid(), Email = "me@test.com" };
            UserDto expectedDto = new() { Id = user.Id, Email = user.Email };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IJwtTokenGenerator> tokenGenMock = new();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.GetUserAsync(principal))
                .ReturnsAsync(user);

            mapperMock
                .Setup(m => m.Map<UserDto>(user))
                .Returns(expectedDto);

            AuthService service = new(
                userManagerMock.Object,
                tokenGenMock.Object,
                mapperMock.Object,
                new Mock<ILogger<AuthService>>().Object
            );

            UserDto? result = await service.GetCurrentUserAsync(principal);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.GetUserAsync(principal), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenNotExists()
        {
            ClaimsPrincipal principal = new(new ClaimsIdentity());

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IJwtTokenGenerator> tokenGenMock = new();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.GetUserAsync(principal))
                .ReturnsAsync((ApplicationUser?)null);

            AuthService service = new(
                userManagerMock.Object,
                tokenGenMock.Object,
                mapperMock.Object,
                new Mock<ILogger<AuthService>>().Object
            );

            UserDto? result = await service.GetCurrentUserAsync(principal);

            result.Should().BeNull();

            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



    }
}
