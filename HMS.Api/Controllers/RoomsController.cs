using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParamaters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    
    public class RoomsController(IRoomService roomService) : BaseApiController
    {
        [HttpGet("Public")] // api: baseUrl/api/Rooms/Public
        public async Task<ActionResult<GenericResponse<IEnumerable<RoomDTO>>>> GetAllRooms(string? roomType, string? sort)
        {
            var result = await roomService.GetAllRoomsForGuestAsync(roomType, sort);

            return HandleResponse(result);
        }

        [HttpGet("{id}")] // api: baseUrl/api/Rooms/102
        public async Task<ActionResult<GenericResponse<RoomDetailsDTO>>> GetRoomDetails(int id)
        {
            var result = await roomService.GetRoomDetailsAsync(id);

            return HandleResponse(result);
        }

        [HttpGet("Admin")] // api: baseUrl/api/Rooms/Admin
        public async Task<ActionResult<GenericResponse<IEnumerable<RoomForAdminDto>>>> GetAllRoomsForAdmin([FromQuery] RoomQueryParam roomQueryParam)
        {
            
            var result = await roomService.GetAllRoomsForAdminAsync(roomQueryParam);
            return HandleResponse(result);
        }

        [HttpPost] // api: baseUrl/api/Rooms
        public async Task<ActionResult<GenericResponse<bool>>> CreateRoom([FromBody] CreateRoomDTO createRoomDTO)
        {
            var result = await roomService.CreateRoomAsync(createRoomDTO);
            return HandleResponse(result);
        }

        [HttpPut("{id}")] // api: baseUrl/api/Rooms/102
        public async Task<ActionResult<GenericResponse<bool>>> UpdateRoom([FromRoute] int id, [FromBody] UpdateRoomDTO updateRoomDTO)
        {
            var result = await roomService.UpdateRoomAsync(id, updateRoomDTO);
            return HandleResponse(result);
        }

        [HttpDelete("{id}")] // api: baseUrl/api/Rooms/102
        public async Task<ActionResult<GenericResponse<bool>>> DeleteRoom([FromRoute] int id)
        {
            var result = await roomService.DeleteRoomAsync(id);
            return HandleResponse(result);
        }

        [HttpPost("{roomId}/Images")] // api: baseUrl/api/Rooms/102/Images
        public async Task<ActionResult<GenericResponse<bool>>> UploadImages([FromRoute] int roomId, [FromForm] List<IFormFile> images)
        {
            var result = await roomService.UploadImagesAsync(roomId, images);
            return HandleResponse(result);
        }
        [HttpDelete("{Id}/Images/{imageId}")] // api: baseUrl/api/Rooms/102/Images/5
        public async Task<ActionResult<GenericResponse<bool>>> DeleteImage([FromRoute] int roomId, [FromRoute] int imageId)
        {
            var result = await roomService.DeleteImageAsync(roomId, imageId);
            return HandleResponse(result);
        }
    }
}
