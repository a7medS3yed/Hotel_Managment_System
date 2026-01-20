using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.RoomDTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomType { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = null!;
        public string RoomStatus { get; set; } = null!;
    }
}
