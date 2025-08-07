using Microsoft.AspNetCore.Identity;

namespace UserService.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public bool IsAdmin { get; set; }

    }
}
