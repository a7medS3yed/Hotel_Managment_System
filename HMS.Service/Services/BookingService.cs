using AutoMapper;
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
        private readonly IMapper _mapper;


        public BookingService(IUnitOfWork unitOfWork, ILogger<Booking> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<GenericResponse<bool>> CancelBookingAsync(Guid bookingId)
        {
            var response = new GenericResponse<bool>();

            try
            {
                var booking = await _unitOfWork.Repository<Booking, Guid>().GetByIdAsync(bookingId, null, [B => B.Guest]);

                if (booking is null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Booking not found.";
                    response.Data = false;
                    return response;
                }

                if (booking.Status == BookingStatus.Paid)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Cannot cancel a paid booking.";
                    response.Data = false;
                    return response;
                }

                booking.Status = BookingStatus.Cancelled;
                _unitOfWork.Repository<Booking, Guid>().Update(booking);
                booking.UpdatedAt = DateTime.UtcNow;
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Booking cancelled successfully.";
                    response.Data = true;

                    return response;

                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Failed to cancel booking. Please try again later.";
                    response.Data = false;
                    return response;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occurred while cancelling the booking: {ex.Message}");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = $"An error occurred while cancelling the booking: {ex.Message}";
                response.Data = false;
                return response;
            }
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

        public async Task<GenericResponse<IEnumerable<BookingDto>>> GetAllBookingForAdminAsync()
        {
            var response = new GenericResponse<IEnumerable<BookingDto>>();

            var bookings = await _unitOfWork.Repository<Booking, Guid>().GetAllAsync(null, null, null, [B => B.Guest]);

            if (bookings is null || !bookings.Any())
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "No bookings found.";
                return response;
            }

            var mappedBookings = _mapper.Map<IEnumerable<BookingDto>>(bookings);

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Bookings retrieved successfully.";
            response.Data = mappedBookings;

            return response;
        }

        public async Task<GenericResponse<IEnumerable<MyBookingDto>>> GetMyBookingAsync(string userId)
        {
            var response = new GenericResponse<IEnumerable<MyBookingDto>>();

            var bookings = await _unitOfWork.Repository<Booking, Guid>()
                .GetAllAsync(B => B.HotelUserId == userId, null, X => X.CreatedAt);

            if (bookings is null || !bookings.Any())
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "No bookings found for the user.";
                return response;
            }
            var mappedBookings = _mapper.Map<IEnumerable<MyBookingDto>>(bookings);

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Bookings retrieved successfully.";
            response.Data = mappedBookings;
            return response;
        }
    }

}