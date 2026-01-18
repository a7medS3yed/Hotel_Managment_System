using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParamaters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
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
        Task<GenericResponse<IEnumerable<RoomForAdminDto>>> GetAllRoomsForAdminAsync(RoomQueryParam? roomQueryParam);
        Task<GenericResponse<bool>> CreateRoomAsync(CreateRoomDTO createRoomDTO);
        Task<GenericResponse<bool>> UpdateRoomAsync(int id, UpdateRoomDTO updateRoomDTO);
        Task<GenericResponse<bool>> DeleteRoomAsync(int id);
        Task<GenericResponse<bool>> UploadImagesAsync(int rooId, List<IFormFile> images);
        Task<GenericResponse<bool>> DeleteImageAsync(int roomId, int imageId);
    }
}
