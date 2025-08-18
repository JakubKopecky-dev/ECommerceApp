using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTOs.Auth
{
    public sealed record AuthRegisterDto
    {
        [EmailAddress]
        public string Email { get; init; } = "";

        public string Password { get; init; } = "";

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? PhoneNumber { get; init; }

        public string? Street { get; init; }

        public string? City { get; init; }

        public string? PostalCode { get; init; }

        public string? Country { get; init; }
    }
}
