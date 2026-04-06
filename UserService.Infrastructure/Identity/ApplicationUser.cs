using Microsoft.AspNetCore.Identity;
using UserService.Domain.Common;

namespace UserService.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; private set; }

        public string? LastName { get; private set; }

        public string? Street { get; private set; }

        public string? City { get; private set; }

        public string? PostalCode { get; private set; }

        public string? Country { get; private set; }

        public bool IsAdmin { get; private set; }



        private ApplicationUser() { }



        public static ApplicationUser Create(string email, string? firstName, string? lastName,string? phoneNumber ,string? street, string? city, string? postalCode, string? country, bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required");



            return new()
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Street = street,
                City = city,
                PostalCode = postalCode,
                Country = country,
                IsAdmin = isAdmin,
                PhoneNumber = phoneNumber,
                UserName = email
            };
        }



        public void ChangeIsAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }



        public void Update(string? firstName, string? lastName, string? phoneNumber, string? street, string? city, string? postalCode, string? country)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }





    }
}
