using HMS.Core.Entities.RoomModule;
using HMS.Core.Entities.SecurityModul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Entities.BookingModule
{
    public class Booking : BaseEntity<Guid>
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EGP";
        public BookingStatus Status { get; set; } = BookingStatus.PendingPayment;
        public string? PaymobOrderId { get; set; }
        public string? PaymobPaymentKey { get; set; }
        public DateTime? PaidDate { get; set; }
        public Room Room { get; set; } = null!;
        public int RoomId { get; set; }
        public HotelUser Guest { get; set; } = null!;
        public string HotelUserId { get; set; } = null!;
    }
}
