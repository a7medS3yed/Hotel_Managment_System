using HMS.Core.Contracts;
using HMS.Core.Entities.BookingModule;
using HMS.Core.Entities.RoomModule;
using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Booking> _logger;

        public BookingService(IUnitOfWork unitOfWork, ILogger<Booking> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<GenericResponse<Guid>> CreateBookingAsync(string userId, CreateBookingDto createBookingDto)
        {
                var response = new GenericResponse<Guid>();
            try
            {

                if (createBookingDto is null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid booking data.";

                    return response;
                }

                if (createBookingDto.CheckInDate < DateTime.UtcNow || createBookingDto.CheckInDate >= createBookingDto.CheckOutDate)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid check-in or check-out dates.";

                    return response;
                }

                var room = await _unitOfWork.Repository<Room, int>()
                    .GetByIdAsync(createBookingDto.RoomId, null, [R => R.Bookings]);

                if (room is null || room.RoomStatus == RoomStatus.NotExit || room.RoomStatus == RoomStatus.Maintenance)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Room not found or unavailable.";
                    return response;
                }

                var isRoomBooked = room.Bookings.Any(booking =>
                    (booking.Status == BookingStatus.PendingPayment || booking.Status == BookingStatus.Paid) &&
                    (booking.CheckInDate < createBookingDto.CheckOutDate) &&
                    (booking.CheckOutDate > createBookingDto.CheckInDate)
                );

                if (isRoomBooked)
                {
                    response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = "Room is already booked for the selected dates.";

                    return response;
                }
                var numberOfNights = (createBookingDto.CheckOutDate - createBookingDto.CheckInDate).Days;
                var totalAmount = room.PricePerNight * numberOfNights;

                var newBooking = new Booking
                {
                    Id = Guid.NewGuid(),
                    HotelUserId = userId,
                    TotalAmount = totalAmount,
                    RoomId = createBookingDto.RoomId,
                    CheckInDate = createBookingDto.CheckInDate,
                    CheckOutDate = createBookingDto.CheckOutDate,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Booking, Guid>().AddAsync(newBooking);
                var saveResult = await _unitOfWork.SaveChangesAsync() > 0;

                if (saveResult)
                {
                    response.StatusCode = StatusCodes.Status201Created;
                    response.Message = "Booking created successfully.";
                    response.Data = newBooking.Id;
                    return response;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Failed to create booking. Please try again later.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating booking for user {UserId}", userId);

                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = $"An error occurred while creating the booking: {ex.Message}";
                return response;
            }
        }
    }
}
