using UserService.Application.DTOs.User;

namespace UserService.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public required UserDto User { get; set; }

        public string Token { get; set; } = "";
    }
}
