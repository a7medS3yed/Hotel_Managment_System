using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.RoomModule;
using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParamaters;
using HMS.Shared.Responses;
using HMS.Shared.SharedEnums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RoomStatus = HMS.Core.Entities.RoomModule.RoomStatus;
using RoomType = HMS.Core.Entities.RoomModule.RoomType;

namespace HMS.Service.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;
        private readonly IAttachmentService _attachmentService;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomService> logger, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _attachmentService = attachmentService;
        }

        public async Task<GenericResponse<bool>> CreateRoomAsync(CreateRoomDTO createRoomDTO)
        {
            var response = new GenericResponse<bool>();

            try
            {
                if (createRoomDTO == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid room data.";
                    response.Data = false;
                    return response;
                }

                var room = _mapper.Map<Room>(createRoomDTO);

                await _unitOfWork.Repository<Room, int>().AddAsync(room);

                var result = await _unitOfWork.SaveChangesAsync();

                if (result > 0)
                {
                    response.StatusCode = StatusCodes.Status201Created;
                    response.Message = "Room created successfully.";
                    response.Data = true;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Failed to create room.";
                    response.Data = false;

                }

                return response;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while creating a room.");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while creating the room.";
                response.Data = false;

                return response;
            }
        }

        public async Task<GenericResponse<bool>> DeleteImageAsync(int roomId, int imageId)
        {
            var response = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.Repository<Room, int>().GetByIdAsync(roomId, null, [R => R.RoomImages]);

                if (room is null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Room not found.";
                    response.Data = false;
                    return response;
                }

                if (room.RoomImages is null || !room.RoomImages.Any())
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No images found for the specified room.";
                    response.Data = false;
                    return response;
                }

                var roomImage = room.RoomImages.FirstOrDefault(RI => RI.Id == imageId);


                if (roomImage is null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No image with this id found to delete.";
                    response.Data = false;
                    return response;
                }

                // Delete from storage
                _attachmentService.Delete("Rooms",roomImage.ImageUrl);

                _unitOfWork.Repository<RoomImage, int>().Delete(roomImage);
               var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Image deleted successfully.";
                response.Data = true;
                
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Failed to delete image.";
                    response.Data = false;
                   

                }
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a room image.");

                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while deleting the image.";
                response.Data = false;
                return response;
            }
        }


        public async Task<GenericResponse<bool>> DeleteRoomAsync(int id)
        {
            var response = new GenericResponse<bool>();
            try
            {
                var repo = _unitOfWork.Repository<Room, int>();

                // Not Completed Logic
                var room = await repo.GetByIdAsync(id);
                if (room is null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Room not found.";
                    response.Data = false;
                }
                room?.RoomStatus.Equals(RoomStatus.NotExit);

                repo.Update(room!);

                var result = await _unitOfWork.SaveChangesAsync();

                if (result > 0)
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Room deleted successfully.";
                    response.Data = true;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Failed to delete room.";
                    response.Data = false;
                }
                return response;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An unexpected behavior happened when try to delete room");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while deleting the room.";
                response.Data = false;

                return response;
            }
        }

        public async Task<GenericResponse<IEnumerable<RoomForAdminDto>>>
         GetAllRoomsForAdminAsync(RoomQueryParam? query)
        {
            var response = new GenericResponse<IEnumerable<RoomForAdminDto>>();
            var repo = _unitOfWork.Repository<Room, int>();

            Expression<Func<Room, bool>>? filter = null;
            Expression<Func<Room, object>>? orderBy = r => r.Id;
            Expression<Func<Room, object>>? orderByDesc = null;

            if (query is not null)
            {
                RoomType? roomType = null;
                RoomStatus? status = null;

                if (!string.IsNullOrWhiteSpace(query.RoomType) &&
                    Enum.TryParse(query.RoomType, true, out Core.Entities.RoomModule.RoomType rt))
                    roomType = rt;

                if (!string.IsNullOrWhiteSpace(query.RoomStatus) &&
                    Enum.TryParse(query.RoomStatus, true, out RoomStatus rs))
                    status = rs;

                filter = r =>
                    (!roomType.HasValue || r.RoomType == roomType) &&
                    (!status.HasValue || r.RoomStatus == status);

                if (!string.IsNullOrWhiteSpace(query.Sort))
                {
                    switch (query.Sort)
                    {
                        case "priceAsc":
                            orderBy = r => r.PricePerNight;
                            break;
                        case "priceDesc":
                            orderBy = null;
                            orderByDesc = r => r.PricePerNight;
                            break;
                    }
                }
            }

            var rooms = filter is null
                ? await repo.GetAllAsync()
                : await repo.GetAllAsync(filter, orderBy, orderByDesc);

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = rooms.Any()
                ? "Rooms retrieved successfully."
                : "No rooms found.";

            response.Data = _mapper.Map<IEnumerable<RoomForAdminDto>>(rooms);

            return response;
        }

        public async Task<GenericResponse<IEnumerable<RoomDTO>>> GetAllRoomsForGuestAsync(string? roomType, string? sort)
        {
            GenericResponse<IEnumerable<RoomDTO>> response = new GenericResponse<IEnumerable<RoomDTO>>();

            Enum.TryParse<RoomType>(roomType, out var parsedRoomType);

            Expression<Func<Room, bool>> filter = R =>
                (roomType == null || R.RoomType == parsedRoomType) 
                && (R.RoomStatus == RoomStatus.Available || R.RoomStatus == RoomStatus.Reserved);

            Expression<Func<Room, object>>? orderBy = null;
            Expression<Func<Room, object>>? orderByDescending = null;

            if (sort is not null)
            {
                switch(sort)
                {
                    case "priceAsc":
                        orderBy = R => R.PricePerNight;
                        break;
                    case "priceDesc":
                        orderByDescending = R => R.PricePerNight;
                        break;
                    default:
                        orderBy = R => R.Id;
                        break;

                }
            }
            else 
                orderBy = R => R.Id;

            var rooms = await _unitOfWork.Repository<Room, int>()
                .GetAllAsync(filter, orderBy, orderByDescending);

            if (rooms is null || !rooms.Any())
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "No rooms found.";
            }
            else
            {
              var data = _mapper.Map<IEnumerable<RoomDTO>>(rooms);

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Rooms retrieved successfully.";
                response.Data = data;
            }

            return response;

        }

        public async Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailsAsync(int roomId)
        {
            var response = new GenericResponse<RoomDetailsDTO>();

            Expression<Func<Room, bool>> filter = R =>
            R.RoomStatus == RoomStatus.Available || R.RoomStatus == RoomStatus.Reserved;

            var room = await _unitOfWork.Repository<Room, int>()
                .GetByIdAsync(roomId, filter, [R => R.RoomImages]);

            if (room is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Room not found.";
            }
            else
            {
                var data = _mapper.Map<RoomDetailsDTO>(room);

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Room details retrieved successfully.";
                response.Data = data;
            }

            return response;
        }

        public async Task<GenericResponse<bool>> UpdateRoomAsync(int id, UpdateRoomDTO updateRoomDTO)
        {
            var response = new GenericResponse<bool>();
            try
            {
                var repo = _unitOfWork.Repository<Room, int>();
                var room = await repo.GetByIdAsync(id);
                if (room is null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Room not found.";
                    response.Data = false;
                }

                var updatedRoom = _mapper.Map<Room>(updateRoomDTO);

                repo.Update(updatedRoom);

                var result = await _unitOfWork.SaveChangesAsync();

                if (result > 0)
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Room updated successfully.";
                    response.Data = true;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Failed to update room.";
                    response.Data = false;
                }

                return response;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while updating a room.");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while updating the room.";
                response.Data = false;

                return response;
            }


        }

        public async Task<GenericResponse<bool>> UploadImagesAsync(int roomId, List<IFormFile> images)
        {
            var response = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.Repository<Room, int>().GetByIdAsync(roomId);

                if (room is null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Room not found.";
                    response.Data = false;
                    return response;
                }

                // ADD THIS: Check if images list is empty
                if (images == null || !images.Any())
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "No images provided.";
                    response.Data = false;
                    return response;
                }

                int uploadedCount = 0;
                List<string> errors = new List<string>(); // Track errors

                foreach (var image in images)
                {
                    // ADD THIS: Log file details
                    _logger.LogInformation($"Processing file: {image.FileName}, Size: {image.Length} bytes");

                    var fileName = await _attachmentService.UploadFileAsync(image, "Rooms");

                    if (fileName is null)
                    {
                        // ADD THIS: Log why it failed
                        _logger.LogWarning($"Failed to upload: {image.FileName}");
                        errors.Add(image.FileName);
                        continue;
                    }

                    var roomImage = new RoomImage
                    {
                        RoomId = roomId,
                        ImageUrl = fileName
                    };

                    await _unitOfWork.Repository<RoomImage, int>().AddAsync(roomImage);
                    uploadedCount++;
                }

                var result = await _unitOfWork.SaveChangesAsync();

                if (result > 0)
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = $"Successfully uploaded {uploadedCount} image(s).";
                    response.Data = true;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = $"Failed to upload images. Rejected files: {string.Join(", ", errors)}";
                    response.Data = false;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding room images");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while uploading room images.";
                response.Data = false;

                return response;
            }
        }
    }
}
