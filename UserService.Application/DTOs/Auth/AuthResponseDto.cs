using UserService.Application.DTOs.User;

namespace UserService.Application.DTOs.Auth
{
    public sealed record AuthResponseDto
    {
        public required UserDto User { get; init; }

        public string Token { get; init; } = "";
    }
}
