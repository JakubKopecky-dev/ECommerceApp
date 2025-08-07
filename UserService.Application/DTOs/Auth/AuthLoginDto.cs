using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTOs.Auth
{
    public class AuthLoginDto
    {
        [EmailAddress]
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";


    }
}
