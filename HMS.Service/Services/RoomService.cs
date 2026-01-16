using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.RoomModule;
using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
    }
}
