using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enum;

namespace UserService.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.User)]

    public class UserController(IApplicationUserService userService) : ControllerBase
    {
        private readonly IApplicationUserService _userService = userService;



        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<IReadOnlyList<UserDto>> GetAllUsers() => await _userService.GetAllUsersAsync();


        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            UserDto? user = await _userService.GetUserAsync(userId);

            return user is not null ? Ok(user) : NotFound();
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            UserDto? user = await _userService.CreateUserAsync(createUserDto);

            return user is not null ? Ok(user) : BadRequest();
        }



        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
        {
            UserDto? user = await _userService.UpdateUserAsync(userId, updateUserDto);

            return user is not null ? Ok(user) : NotFound();
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            UserDto? user = await _userService.DeleteUserAsync(userId);

            return user is not null ? Ok(user) : NotFound();
        }


       // [Authorize(Roles = UserRoles.Admin)]
        [HttpPatch("{userId}")]
        public async Task<IActionResult> ChangeIsAdminAsync(Guid userId, [FromBody] ChangeIsAdminDto changeIsAdminDto)
        {
            UserDto? user = await _userService.ChangeIsAdminAsync(userId, changeIsAdminDto);

            return user is not null ? Ok(user) : NotFound();
        }





    }
}
