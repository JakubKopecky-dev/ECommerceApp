using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Interfaces.JwtToken
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, string userName, IList<string> roles);
    }
}
