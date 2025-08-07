namespace UserService.Application.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public bool IsAdmin { get; set; }
    }
}
