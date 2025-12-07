namespace UserService.Infrastructure.Auth
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "";

        public string Audience { get; set; } = "";

        public string PrivateKey { get; set; } = "";  // ES256 private key (PEM)

        public string PublicKey { get; set; } = "";   // ES256 public key (PEM)

        public int ExpiresInMinutes { get; set; }
    }
}
