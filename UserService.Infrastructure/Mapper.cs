using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.User;
using UserService.Infrastructure.Identity;

namespace UserService.Infrastructure
{
    public static class Mapper
    {

        public static UserDto UserToUserDto(this ApplicationUser user) =>
            new()
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                City = user.City,
                Country = user.Country,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PostalCode = user.PostalCode,
                Street = user.Street
            };



    }
}
