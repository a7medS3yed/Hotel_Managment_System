using HMS.Shared.SharedEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.RoomDTOs
{
    public class CreateRoomDTO
    {
        [Required(ErrorMessage ="Room type is required.")]
        public RoomType RoomType { get; set; }
        [Required(ErrorMessage = "Price per night is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per night must be greater than zero.")]
        public decimal PricePerNight { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; } = null!;
        [Required(ErrorMessage = "Amentities are required.")]
        public string Amenities { get; set; } = null!;
    }
}
