using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.FeedbackDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Api.Controllers
{
    public class FeedbacksController : BaseApiController
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<ActionResult> CreateFeedback([FromBody] CreateFeedbackDto feedbackDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _feedbackService.CreateFeedbackAsync(userId!, feedbackDto);
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> GetAllFeedbacks()
        {
            var result = await _feedbackService.GetAllFeedbackForAdminAsync();
            return HandleResponse(result);
        }
    }
}
