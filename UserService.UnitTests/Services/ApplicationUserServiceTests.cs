using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MockQueryable.Moq;
using UserService.Application.DTOs.User;
using UserService.Domain.Enums;
using UserService.Infrastructure.Identity;
using UserService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using MockQueryable;

namespace UserService.UnitTests.Services
{
    public class ApplicationUserServiceTests
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
        public async Task GetAllUsersAsync_ReturnsUserDtoList_WhenExists()
        {
            List<ApplicationUser> users =
            [
                new() { Id = Guid.NewGuid(), Email = "a@test.com" },
                new() { Id = Guid.NewGuid(), Email = "b@test.com" }
            ];

            List<UserDto> expectedDto =
            [
                new() { Id = users[0].Id, Email = users[0].Email! },
                new() { Id = users[1].Id, Email = users[1].Email! }
            ];

            var usersQueryMock = users.BuildMockDbSet();


            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.Users)
                .Returns(usersQueryMock.Object);

            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<List<UserDto>>(It.IsAny<List<ApplicationUser>>()))
                .Returns(expectedDto);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            IReadOnlyList<UserDto> result = await service.GetAllUsersAsync(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.Users, Times.Once);
            mapperMock.Verify(m => m.Map<List<UserDto>>(It.IsAny<List<ApplicationUser>>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUserAsync_ReturnsUserDto_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            ApplicationUser user = new() { Id = userId, Email = "user@test.com" };
            UserDto expectedDto = new() { Id = userId, Email = user.Email! };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<UserDto>(user))
                .Returns(expectedDto);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.GetUserAsync(userId);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUserAsync_ReturnsNull_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            Mock<IMapper> mapperMock = new();

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.GetUserAsync(userId);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUserAsync_ReturnsUserDto_WhenSucceeded()
        {
            CreateUserDto createDto = new() { Email = "new@test.com", Password = "P@ssw0rd" };

            ApplicationUser user = new() { Id = Guid.NewGuid(), Email = createDto.Email };
            UserDto expectedDto = new() { Id = user.Id, Email = user.Email! };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<ApplicationUser>(createDto))
                .Returns(user);

            userManagerMock
                .Setup(u => u.CreateAsync(user, createDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(u => u.AddToRoleAsync(user, UserRoles.User))
                .ReturnsAsync(IdentityResult.Success);

            mapperMock
                .Setup(m => m.Map<UserDto>(user))
                .Returns(expectedDto);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.CreateUserAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.CreateAsync(user, createDto.Password), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(user, UserRoles.User), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUserAsync_ReturnsNull_WhenFailed()
        {
            CreateUserDto createDto = new() { Email = "fail@test.com", Password = "123" };

            ApplicationUser user = new() { Id = Guid.NewGuid(), Email = createDto.Email };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<ApplicationUser>(createDto))
                .Returns(user);

            userManagerMock
                .Setup(u => u.CreateAsync(user, createDto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "fail" }));

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.CreateUserAsync(createDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUserAsync_ReturnsUserDto_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            UpdateUserDto updateDto = new()
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123456789",
                Street = "Main St",
                City = "Prague",
                PostalCode = "12345",
                Country = "CZ"
            };

            ApplicationUser user = new()
            {
                Id = userId,
                Email = "old@test.com",
                FirstName = "Old",
                LastName = "Name",
                PhoneNumber = "000",
                Street = "Old St",
                City = "Old City",
                PostalCode = "00000",
                Country = "OldLand"
            };

            UserDto expectedDto = new()
            {
                Id = userId,
                Email = user.Email!,
                FirstName = updateDto.FirstName,
                LastName = updateDto.LastName,
                PhoneNumber = updateDto.PhoneNumber,
                Street = updateDto.Street,
                City = updateDto.City,
                PostalCode = updateDto.PostalCode,
                Country = updateDto.Country
            };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            mapperMock
                .Setup(m => m.Map(updateDto, user))
                .Returns(user);

            userManagerMock
                .Setup(u => u.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            mapperMock
                .Setup(m => m.Map<UserDto>(user))
                .Returns(expectedDto);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.UpdateUserAsync(userId, updateDto);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUserAsync_ReturnsNull_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            UpdateUserDto updateDto = new()
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123456789",
                Street = "Main St",
                City = "Prague",
                PostalCode = "12345",
                Country = "CZ"
            };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.UpdateUserAsync(userId, updateDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeIsAdminAsync_ReturnsUserDto_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            ChangeIsAdminDto changeDto = new() { IsAdmin = true };

            ApplicationUser user = new() { Id = userId, Email = "user@test.com", IsAdmin = false };
            UserDto expectedDto = new() { Id = userId, Email = user.Email! };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(u => u.IsInRoleAsync(user, UserRoles.Admin))
                .ReturnsAsync(false);

            userManagerMock
                .Setup(u => u.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(u => u.AddToRoleAsync(user, UserRoles.Admin))
                .ReturnsAsync(IdentityResult.Success);

            mapperMock
                .Setup(m => m.Map<UserDto>(user))
                .Returns(expectedDto);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.ChangeIsAdminAsync(userId, changeDto);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            userManagerMock.Verify(u => u.IsInRoleAsync(user, UserRoles.Admin), Times.Once);
            userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(user, UserRoles.Admin), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeIsAdminAsync_ReturnsNull_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            ChangeIsAdminDto changeDto = new() { IsAdmin = true };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.ChangeIsAdminAsync(userId, changeDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUserAsync_ReturnsUserDto_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            ApplicationUser user = new() { Id = userId, Email = "delete@test.com" };
            UserDto expectedDto = new() { Id = userId, Email = user.Email! };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync([]);

            userManagerMock
                .Setup(u => u.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            mapperMock
                .Setup(m => m.Map<UserDto>(user))
                .Returns(expectedDto);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.DeleteUserAsync(userId);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            userManagerMock.Verify(u => u.DeleteAsync(user), Times.Once);
            mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUserAsync_ReturnsNull_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();
            Mock<IMapper> mapperMock = new();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            ApplicationUserService service = new(
                userManagerMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.DeleteUserAsync(userId);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
            mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
        }
    }
}
