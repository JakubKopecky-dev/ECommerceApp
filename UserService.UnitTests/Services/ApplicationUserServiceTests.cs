using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Application.DTOs.User;
using UserService.Domain.Enums;
using UserService.Infrastructure;
using UserService.Infrastructure.Identity;
using UserService.Infrastructure.Services;

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


        private static ApplicationUser MockUser(string email) =>
            ApplicationUser.Create(email, "John", null, null, null, null, null, null, false);



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllUsersAsync_ReturnsUserDtoList_WhenExists()
        {
            List<ApplicationUser> users =
            [
                MockUser("a@test.com"),
                MockUser("b@test.com")
            ];

            List<UserDto> expectedDto =
            [
                users[0].UserToUserDto(),
                users[1].UserToUserDto()
            ];

            var usersQueryMock = users.BuildMockDbSet();


            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.Users)
                .Returns(usersQueryMock.Object);


            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            IReadOnlyList<UserDto> result = await service.GetAllUsersAsync(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.Users, Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUserAsync_ReturnsUserDto_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            ApplicationUser user = MockUser("a@test.com");
            UserDto expectedDto = user.UserToUserDto();

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);


            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.GetUserAsync(userId);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
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


            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.GetUserAsync(userId);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUserAsync_ReturnsUserDto_WhenSucceeded()
        {
            CreateUserDto createDto = new() { Email = "new@test.com", Password = "P@ssw0rd" };

            ApplicationUser user = MockUser(createDto.Email);
            UserDto expectedDto = new() { Email = createDto.Email, IsAdmin = false };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();


            userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.User))
                .ReturnsAsync(IdentityResult.Success);

            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.CreateUserAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto, x => x.Excluding(x => x.Id));

            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.User), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUserAsync_ReturnsNull_WhenFailed()
        {
            CreateUserDto createDto = new() { Email = "fail@test.com", Password = "123" };

            ApplicationUser user = MockUser(createDto.Email);

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();


            userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "fail" }));

            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.CreateUserAsync(createDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUserAsync_ReturnsUserDto_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            UpdateUserDto updateDto = new()
            {
                FirstName = "Petr", 
            };

            ApplicationUser user = MockUser("a@b.cz");


            UserDto expectedDto = new() {Id = user.Id, FirstName = updateDto.FirstName,Email = user.Email!, IsAdmin = user.IsAdmin};

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);


            userManagerMock
                .Setup(u => u.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);


            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.UpdateUserAsync(userId, updateDto);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
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

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.UpdateUserAsync(userId, updateDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeIsAdminAsync_ReturnsUserDto_WhenSucceeded()
        {
            Guid userId = Guid.NewGuid();

            ChangeIsAdminDto changeDto = new() { IsAdmin = true };

            ApplicationUser user = MockUser("a@b.cz");
            UserDto expectedDto = new() {Id = user.Id,FirstName = user.FirstName,Email = user.Email!, IsAdmin = true};

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

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


            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.ChangeIsAdminAsync(userId, changeDto);

            result.Should().BeEquivalentTo(expectedDto);

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            userManagerMock.Verify(u => u.IsInRoleAsync(user, UserRoles.Admin), Times.Once);
            userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(user, UserRoles.Admin), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeIsAdminAsync_ReturnsNull_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            ChangeIsAdminDto changeDto = new() { IsAdmin = true };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            UserDto? result = await service.ChangeIsAdminAsync(userId, changeDto);

            result.Should().BeNull();

            userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUserAsync_ReturnsTrue_WhenSucceeded()
        {

            ApplicationUser user = MockUser("delete@test.com");
            Guid userId = user.Id;

            UserDto expectedDto = new() { Id = userId, Email = user.Email! };

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync([]);

            userManagerMock
                .Setup(u => u.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);


            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            bool result = await service.DeleteUserAsync(userId);

            result.Should().BeTrue();

            userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
            userManagerMock.Verify(u => u.DeleteAsync(user), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUserAsync_ReturnsFalse_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<UserManager<ApplicationUser>> userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            ApplicationUserService service = new(
                userManagerMock.Object,
                new Mock<ILogger<ApplicationUserService>>().Object
            );


            bool result = await service.DeleteUserAsync(userId);

            result.Should().BeFalse();

            userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }
    }
}
