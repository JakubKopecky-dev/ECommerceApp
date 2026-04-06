using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.JwtToken;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enums;
using UserService.Infrastructure.Identity;

namespace UserService.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator tokenGenerator,  ILogger<AuthService> logger) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
        private readonly ILogger<AuthService> _logger = logger;



        public async Task<AuthResponseDto?> RegisterUserAsync(AuthRegisterDto authRegisterDto)
        {
            _logger.LogInformation("Registering user. UserEmail: {Email}.", authRegisterDto.Email);

            ApplicationUser newUser = ApplicationUser.Create(authRegisterDto.Email, authRegisterDto.FirstName, authRegisterDto.LastName,
                authRegisterDto.PhoneNumber, authRegisterDto.Street, authRegisterDto.City, authRegisterDto.PostalCode, authRegisterDto.Country, false);

            var result = await _userManager.CreateAsync(newUser, authRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                IEnumerable<string> roles = await _userManager.GetRolesAsync(newUser);
                string token = _tokenGenerator.GenerateToken(newUser.Id, newUser.Email!, newUser.UserName!, roles);

                _logger.LogInformation("User registred. UserEmail: {Email}, UserId: {Id}.", newUser.Email, newUser.Id);

                AuthResponseDto response = new() { User = newUser.UserToUserDto(), Token = token };

                return response;
            }

            _logger.LogWarning("User wasn't registred. UserEmail: {Email}", authRegisterDto.Email);

            return null;
        }



        public async Task<AuthResponseDto?> LoginUserAsync(AuthLoginDto authLoginDto)
        {
            _logger.LogInformation("Logging in user. UserEmail: {Email}.", authLoginDto.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(authLoginDto.Email);
            if (user is null)
            {
                _logger.LogWarning("Login failed. User not found. UserEmail: {Email}.", authLoginDto.Email);
                return null;
            }

            bool isPasswrodValid = await _userManager.CheckPasswordAsync(user,authLoginDto.Password);
            if (!isPasswrodValid)
            {
                _logger.LogWarning("Login failed. Invalid password. UserEmail: {Email}.", authLoginDto.Email);
                return null;
            }

            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            string token = _tokenGenerator.GenerateToken(user.Id,user.Email!,user.UserName!,roles);

            _logger.LogInformation("User successfully logged in. UserEmail: {Email}.", authLoginDto.Email);


            AuthResponseDto response = new() { User = user.UserToUserDto(), Token = token };

            return response;
        }



        public async Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(claimsPrincipal);

            return user?.UserToUserDto();
        }



    }
}
