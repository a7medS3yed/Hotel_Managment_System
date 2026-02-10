using HMS.Shared.SharedEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.RoomDTOs
{
    public class UpdateRoomDTO
    {
        public int Id { get; set; }
        public RoomType RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public string Description { get; set; } = null!;
        public string Amenities { get; set; } = null!;
        public RoomStatus RoomStatus { get; set; }
    }
}
