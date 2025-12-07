using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Interfaces.JwtToken;

namespace UserService.Infrastructure.Auth
{
    public class JwtGenerator(IOptions<JwtSettings> jwtSettings) : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public string GenerateToken(Guid userId, string email, string userName, IEnumerable<string> roles)
        {
            // 1) Load ES256 private key
            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(_jwtSettings.PrivateKey);

            ECDsaSecurityKey signingKey = new(ecdsa)
            {
                KeyId = Guid.NewGuid().ToString(),
            };

            SigningCredentials credentials = new(signingKey, SecurityAlgorithms.EcdsaSha256);


            // 2) Build claims
            List<Claim> claims =
            [
                // ASP.NET Identity compatible
                new (ClaimTypes.NameIdentifier, userId.ToString()),
                new (ClaimTypes.Name, userName),
                new (ClaimTypes.Email, email),

                // JWT/OIDC standard compatible
                new (JwtRegisteredClaimNames.Sub, userId.ToString()),
                new (JwtRegisteredClaimNames.UniqueName, userName),
                new (JwtRegisteredClaimNames.Email, email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
            ];

            // Add role to claim
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                    claims.Add(new Claim("role", role));
                }
            }


            // Token expiry
            var expiry = DateTime.Now.AddMinutes(_jwtSettings.ExpiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
