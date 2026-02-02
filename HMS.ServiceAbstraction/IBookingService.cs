using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IBookingService
    {
        Task<GenericResponse<string>> CreateBookingAsync(string userId, CreateBookingDto createBookingDto);
    }
}
