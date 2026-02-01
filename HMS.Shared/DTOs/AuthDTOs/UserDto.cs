namespace HMS.Shared.DTOs.AuthDTOs
{
    public class UserDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
