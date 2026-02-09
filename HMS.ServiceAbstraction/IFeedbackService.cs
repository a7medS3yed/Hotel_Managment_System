using HMS.Shared.DTOs.FeedbackDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IFeedbackService
    {
        Task<GenericResponse<bool>> CreateFeedbackAsync(string userId, CreateFeedbackDto feedback);
        Task<GenericResponse<IEnumerable<FeedbackAdminDto>>> GetAllFeedbackForAdminAsync();
    }
}
