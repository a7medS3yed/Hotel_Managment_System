using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Api.Controllers
{
    public class BookingsController : BaseApiController
    {
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;

        public BookingsController(IBookingService bookingService, IPaymentService paymentService)
        {
            _bookingService = bookingService;
            _paymentService = paymentService;
        }

        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<Guid>>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CreateBookingAsync(userId!, createBookingDto);

            return HandleResponse(result);
        }

        [HttpPost("{id}/pay")]
        public async Task<ActionResult<GenericResponse<string>>> PayForBooking([FromRoute] Guid id)
        {
            var result = await _paymentService.CreatePaymentUrlAsync(id);
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<GenericResponse<IEnumerable<BookingDto>>>> GetAllBookingsForAdmin()
        {
            var result = await _bookingService.GetAllBookingForAdminAsync();
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<GenericResponse<bool>>> CancelBooking([FromRoute] Guid id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            return HandleResponse(result);
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("my")]
        public async Task<ActionResult<GenericResponse<IEnumerable<MyBookingDto>>>> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.GetMyBookingAsync(userId!);
            return HandleResponse(result);
        }
    }
}