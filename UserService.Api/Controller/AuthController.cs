using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enum;

namespace UserService.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;



        [HttpPost("register")]
        public async Task<IActionResult> UserRegister(AuthRegisterDto authRegisterDto)
        {
            AuthResponseDto? response = await _authService.RegisterUserAsync(authRegisterDto);

            return response is not null ? Ok(response) : BadRequest();
        }



        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(AuthLoginDto authLoginDto)
        {
            AuthResponseDto? response = await _authService.LoginUserAsync(authLoginDto);

            return response is not null ? Ok(response) : BadRequest();
        }



        [Authorize(Roles = UserRoles.User)]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            UserDto? user = await _authService.GetCurrentUserAsync(User);

           return user is not null ? Ok(user) : BadRequest();
        }



    }
}
