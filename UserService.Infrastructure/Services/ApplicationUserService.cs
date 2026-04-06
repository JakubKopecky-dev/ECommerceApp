using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enums;
using UserService.Infrastructure.Identity;

namespace UserService.Infrastructure.Services
{
    public class ApplicationUserService(UserManager<ApplicationUser> userManager, ILogger<ApplicationUserService> logger) : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ApplicationUserService> _logger = logger;



        public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all users.");

            List<ApplicationUser> users = await _userManager.Users.ToListAsync(ct);
            _logger.LogInformation("Retrieved all users. Count: {Count}.", users.Count);

            return [.. users.Select(x => x.UserToUserDto())];
        }



        public async Task<UserDto?> GetUserAsync(Guid userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                _logger.LogWarning("User not found. UserId: {UserId}.", userId);
            else
                _logger.LogInformation("User found. UserId: {UserId}.", userId);

            return user?.UserToUserDto();
        }



        public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user. UserEmail: {UserEmail}.", createUserDto.Email);

            ApplicationUser user = ApplicationUser.Create(createUserDto.Email, createUserDto.FirstName, createUserDto.LastName, createUserDto.PhoneNumber, createUserDto.Street,
                createUserDto.City, createUserDto.PostalCode, createUserDto.Country, createUserDto.IsAdmin);

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);

                if (user.IsAdmin)
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);

                _logger.LogInformation("User created. UserId: {UserId}, UserEmail: {UserEmail}.", user.Id, user.Email);

                return user.UserToUserDto();
            }

            _logger.LogWarning("User wasn't created.");

            return null;
        }



        public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user. UserId: {UserId}.", userId);

            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                _logger.LogWarning("Cannot update. User not found. UserId: {UserId}.", userId);
                return null;
            }

            user.Update(updateUserDto.FirstName,updateUserDto.LastName,updateUserDto.PhoneNumber,updateUserDto.Street,updateUserDto.City, updateUserDto.PostalCode,updateUserDto.City);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User updated. UserId: {UserId}", userId);

                return user.UserToUserDto();
            }

            _logger.LogWarning("User wasn't updated. UserId: {UserId}", userId);

            return null;
        }



        public async Task<UserDto?> ChangeIsAdminAsync(Guid userId, ChangeIsAdminDto changeDto)
        {
            _logger.LogInformation("Updating IsAdmin on user. UserId: {UserId}.", userId);

            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogWarning("Cannot update IsAdmin. User not found. UserId: {UserId}.", userId);
                return null;
            }

            bool wasAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);

            user.ChangeIsAdmin(changeDto.IsAdmin);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (!changeDto.IsAdmin && wasAdmin)
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);

                else if (changeDto.IsAdmin && !wasAdmin)
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);

                _logger.LogInformation("User updated. UserId: {UserId}", userId);

                return user.UserToUserDto();
            }

            _logger.LogWarning("User wasn't updated. UserId: {UserId}", userId);

            return null;
        }



        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            _logger.LogInformation("Deleting user. UserId: {UserId}.", userId);

            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogWarning("Cannot delete. User not found.");
                return false;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(user, roles);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User deleted. UserId: {UserId}.", userId);
                return true;
            }

            _logger.LogWarning("User wasn't deleted. UserId: {UserId}.", userId);
            return false;
        }



    }
}
