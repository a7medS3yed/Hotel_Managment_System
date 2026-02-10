using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.RoomDTOs
{
    public class RoomDetailsDTO
    {
        public string RoomType { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = null!;
        public string RoomStatus { get; set; } = null!;
        public List<string> Iamges { get; set; } = [];
        public string Description { get; set; } = null!;
    }
}
