using System.ComponentModel.DataAnnotations;

namespace HMS.Shared.DTOs.AuthDTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage ="Enter valid email")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; } = null!;
    }
}
