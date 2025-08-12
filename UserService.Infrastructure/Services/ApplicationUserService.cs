using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enum;
using UserService.Infrastructure.Identity;

namespace UserService.Infrastructure.Services
{
    public class ApplicationUserService(UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<ApplicationUserService> logger) : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ApplicationUserService> _logger = logger;



        public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all users.");

            List<ApplicationUser> users = await _userManager.Users.ToListAsync(ct);
            _logger.LogInformation("Retrieved all users. Count: {Count}.", users.Count);

            return _mapper.Map<List<UserDto>>(users);
        }



        public async Task<UserDto?> GetUserAsync(Guid userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User not found. UserId: {UserId}.", userId);
                return null;
            }

            _logger.LogInformation("User found. UserId: {UserId}.", userId);

            return _mapper.Map<UserDto>(user);
        }



        public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user. UserEmail: {UserEmail}.", createUserDto.Email);

            ApplicationUser user = _mapper.Map<ApplicationUser>(createUserDto);
            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);

                if (user.IsAdmin)
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);

                _logger.LogInformation("User created. UserId: {UserId}, UserEmail: {UserEmail}.", user.Id, user.Email);

                return _mapper.Map<UserDto>(user);
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

            _mapper.Map<UpdateUserDto, ApplicationUser>(updateUserDto, user);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User updated. UserId: {UserId}", userId);

                return _mapper.Map<UserDto>(user);
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

            user.IsAdmin = changeDto.IsAdmin;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (!changeDto.IsAdmin && wasAdmin)
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);

                else if (changeDto.IsAdmin && !wasAdmin)
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);

                _logger.LogInformation("User updated. UserId: {UserId}", userId);

                return _mapper.Map<UserDto>(user);
            }

            _logger.LogWarning("User wasn't updated. UserId: {UserId}", userId);

            return null;
        }



        public async Task<UserDto?> DeleteUserAsync(Guid userId)
        {
            _logger.LogInformation("Deleting user. UserId: {UserId}.", userId);

            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogWarning("Cannot delete. User not found.");
                return null;
            }

            UserDto deletedUser = _mapper.Map<UserDto>(user);

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(user, roles);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User deleted. UserId: {UserId}.", userId);
                return deletedUser;
            }

            _logger.LogWarning("User wasn't deleted. UserId: {UserId}.", userId);
            return null;
        }



    }
}
