using HMS.Shared.DTOs.FeedbackDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IAiModerationService
    {
        Task<GenericResponse<AiDto>> AnalyzeAsync(string text);
    }
}
