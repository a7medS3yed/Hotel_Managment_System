using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.FeedbackDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.ExternalService
{
    public class FakeAiModerationService : IAiModerationService
    {
     
        async Task<GenericResponse<AiDto>> IAiModerationService.AnalyzeAsync(string text)
        {
            var response = new GenericResponse<AiDto>();
            var bannedWords = new[] { "shit", "fuck", "idiot" };

            if (bannedWords.Any(w => text.ToLower().Contains(w)))
            {
                response.StatusCode = 400;
                response.Message = "Offensive language detected";
                response.Data = new AiDto { IsApproved = false, Reason = "Offensive language detected" };
            }
            else
            {
                response.StatusCode = 200;
                response.Message = "Clean content";
                response.Data = new AiDto { IsApproved = true, Reason = "Clean content" };
            }
            return response;


        }
    }
}
