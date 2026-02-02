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
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize(Roles ="Guest")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<string>>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CreateBookingAsync(userId!, createBookingDto);

            return HandleResponse(result);
        }
    }
}
