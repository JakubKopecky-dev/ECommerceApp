namespace UserService.Application.DTOs.User
{
    public sealed record UserDto
    {
        public Guid Id { get; init; }

        public string Email { get; init; } = "";

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? PhoneNumber { get; init; }

        public string? Street { get; init; }

        public string? City { get; init; }

        public string? PostalCode { get; init; }

        public string? Country { get; init; }

        public bool IsAdmin { get; init; }
    }
}
