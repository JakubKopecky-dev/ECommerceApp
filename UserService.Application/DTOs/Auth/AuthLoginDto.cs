using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTOs.Auth
{
    public sealed record AuthLoginDto
    {
        [EmailAddress]
        public string Email { get; init; } = "";

        public string Password { get; init; } = "";

    }
}
