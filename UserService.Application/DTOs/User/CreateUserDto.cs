using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTOs.User
{
    public class CreateUserDto
    {
        [EmailAddress]
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public bool IsAdmin { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

    }
}
