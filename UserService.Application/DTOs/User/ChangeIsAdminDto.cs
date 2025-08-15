namespace UserService.Application.DTOs.User
{
    public sealed record ChangeIsAdminDto
    {
        public bool IsAdmin { get; init; }
    }
}
