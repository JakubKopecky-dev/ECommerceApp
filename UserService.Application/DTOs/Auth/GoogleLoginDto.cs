using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.DTOs.Auth
{
    public sealed record GoogleLoginDto(string IdToken);

}
