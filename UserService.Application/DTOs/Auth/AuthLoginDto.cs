using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.DTOs.Auth
{
    public class AuthLoginDto
    {
        [EmailAddress]
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";


    }
}
