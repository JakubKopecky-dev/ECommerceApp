using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.User;

namespace UserService.Application.Interfaces.Services
{
    public interface IApplicationUserService
    {
        Task<UserDto?> ChangeIsAdminAsync(Guid userId, ChangeIsAdminDto changeDto);
        Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> DeleteUserAsync(Guid userId);
        Task<IReadOnlyList<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserAsync(Guid userId);
        Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
    }
}
