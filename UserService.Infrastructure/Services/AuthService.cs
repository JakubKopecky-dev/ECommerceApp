using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.JwtToken;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enum;
using UserService.Infrastructure.Identity;

namespace UserService.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator tokenGenerator, IMapper mapper, ILogger<AuthService> logger) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthService> _logger = logger;



        public async Task<AuthResponseDto?> RegisterUserAsync(AuthRegisterDto authRegisterDto)
        {
            _logger.LogInformation("Registering user. UserEmail: {Email}.", authRegisterDto.Email);

            ApplicationUser newUser = new()
            {
                UserName = authRegisterDto.Email,
                Email = authRegisterDto.Email,
                FirstName = authRegisterDto.FirstName,
                LastName = authRegisterDto.LastName,
                Street = authRegisterDto.Street,
                City = authRegisterDto.City,
                PostalCode = authRegisterDto.PostalCode,
                Country = authRegisterDto.Country
            };

            var result = await _userManager.CreateAsync(newUser, authRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                var roles = await _userManager.GetRolesAsync(newUser);
                string token = _tokenGenerator.GenerateToken(newUser.Id, newUser.Email, newUser.UserName, roles);

                _logger.LogInformation("User registred. UserEmail: {Email}, UserId: {Id}.", newUser.Email, newUser.Id);

                UserDto userDto = _mapper.Map<UserDto>(newUser);

                AuthResponseDto response = new() { User = userDto, Token = token };

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

            var roles = await _userManager.GetRolesAsync(user);
            string token = _tokenGenerator.GenerateToken(user.Id,user.Email!,user.UserName!,roles);

            _logger.LogInformation("User successfully logged in. UserEmail: {Email}.", authLoginDto.Email);

            UserDto userDto = _mapper.Map<UserDto>(user);

            AuthResponseDto response = new() { User = userDto, Token = token };

            return response;
        }



        public async Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(claimsPrincipal);

            return user is not null ? _mapper.Map<UserDto>(user) : null;
        }



    }
}
