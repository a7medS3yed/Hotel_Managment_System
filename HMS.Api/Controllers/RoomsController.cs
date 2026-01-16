using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController(IRoomService roomService) : ControllerBase
    {
        [HttpGet("Public")] // api: baseUrl/api/Rooms/Public
        public async Task<ActionResult<GenericResponse<IEnumerable<RoomDTO>>>> GetAllRooms(string? roomType, string? sort)
        {
            var result = await roomService.GetAllRoomsForGuestAsync(roomType, sort);

            return Ok(result);
        }

        [HttpGet("{id}")] // api: baseUrl/api/Rooms/102
        public async Task<ActionResult<GenericResponse<RoomDetailsDTO>>> GetRoomDetails(int id)
        {
            var result = await roomService.GetRoomDetailsAsync(id);

            return Ok(result);
        }
    }
}
