using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IRoomService
    {
        Task<GenericResponse<IEnumerable<RoomDTO>>> GetAllRoomsForGuestAsync(string? roomType, string? sort);

        Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailsAsync(int roomId);
    }
}
