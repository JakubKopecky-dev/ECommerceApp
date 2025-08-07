namespace UserService.Application.Interfaces.JwtToken
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, string userName, IList<string> roles);
    }
}
