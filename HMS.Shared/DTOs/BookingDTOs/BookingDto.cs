using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.BookingDTOs
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string GuestFullName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string GuestEmail { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public int RoomId  { get; set; }
    }
}
