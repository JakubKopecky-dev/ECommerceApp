using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enum;
using Microsoft.AspNetCore.Authentication;


namespace UserService.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, IExternalAuthService externalAuthService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IExternalAuthService _externalAuthService = externalAuthService;



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



        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var response = await _externalAuthService.LoginWithGoogleAsync(dto.IdToken);

            return response is not null ? Ok(response) : Unauthorized();
        }


        /*
         
        1) https://developers.google.com/oauthplayground/
        2) Into scope: openid email profile
        3) Login with google account
        4) Click blue button (Exchange authorazion code for tokens)
        5) Copy id_token from response
        
        */


    }
}
