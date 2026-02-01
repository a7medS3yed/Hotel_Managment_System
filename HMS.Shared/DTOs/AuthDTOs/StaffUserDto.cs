using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.AuthDTOs
{
    public class StaffUserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Enter valid phone number")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Specialities is required")]
        public string specialities { get; set; } = null!;
    }
}
