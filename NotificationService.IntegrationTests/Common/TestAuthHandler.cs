using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NotificationService.IntegrationTests.Common
{
    public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public static Guid FixedUserId { get; set; } = Guid.Empty;

        public static List<string> FixedRoles { get; set; } = new() { "User" };

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userId = FixedUserId == Guid.Empty ? Guid.NewGuid() : FixedUserId;

            List<Claim> claims =
            [
                new (ClaimTypes.NameIdentifier, userId.ToString()),
                new (ClaimTypes.Name, "TestUser")
            ];


            // Přidej všechny nastavené role
            claims.AddRange(FixedRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
