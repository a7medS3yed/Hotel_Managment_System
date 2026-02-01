using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.AuthDTOs
{
    public class ProfileUserDto
    {
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;

    }
}
