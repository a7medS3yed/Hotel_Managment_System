using HMS.Core.Entities.SecurityModul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Entities.ServiceModule
{
    public class ServiceRequest : BaseEntity<int>
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public string GuestId { get; set; } = null!;
        public HotelUser Guest { get; set; } = null!;

        public string? StaffId { get; set; }
        public HotelUser? Staff { get; set; }

        public ServiceRequestStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
