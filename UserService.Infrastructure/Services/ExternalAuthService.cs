using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.JwtToken;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Enums;
using UserService.Infrastructure.Identity;
using Google.Apis.Auth;

namespace UserService.Infrastructure.Services
{
    public class ExternalAuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, ILogger<ExternalAuthService> logger, IMapper mapper) : IExternalAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly ILogger<ExternalAuthService> _logger = logger;
        private readonly IMapper _mapper = mapper;


        public async Task<AuthResponseDto?> LoginWithGoogleAsync(string idToken)
        {
            _logger.LogInformation("Validating Google ID token...");

            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid Google token.");
                return null;
            }

            string email = payload.Email;
            string googleId = payload.Subject;

            _logger.LogInformation("Google login for email {Email}, GoogleId {GoogleId}", email, googleId);

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new()
                {
                    Email = email,
                    UserName = email
                };

                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, UserRoles.User);

                _logger.LogInformation("Created new user {UserId} from Google login.", user.Id);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.UserName!, roles);

            var userDto = _mapper.Map<UserDto>(user);

            return new()
            {
                User = userDto,
                Token = token
            };

        }








    }
}
