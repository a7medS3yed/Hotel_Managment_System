using HMS.Core.Contracts;
using HMS.Core.Entities.FeedbackModule;
using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.FeedbackDTOs;
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
    public class FeedbackService : IFeedbackService
    {
        private readonly IAiModerationService _aiService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Feedback> _logger;

        public FeedbackService( IAiModerationService aiService,
            IUnitOfWork unitOfWork,
            ILogger<Feedback> logger)
        {
            _aiService = aiService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<GenericResponse<bool>> CreateFeedbackAsync(string userId, CreateFeedbackDto feedbackDto)
        {
            var response = new GenericResponse<bool>();

            try
            {
                var aiResult = await _aiService.AnalyzeAsync(feedbackDto.Content);

                if (feedbackDto.Content is null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Feedback content cannot be empty.";
                    response.Data = false;
                    return response;
                }

                if (!aiResult!.Data!.IsApproved)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = $"Feedback rejected: {aiResult.Data.Reason}";
                    response.Data = false;
                    return response;
                }

                var feedback = new Feedback
                {
                    UserId = userId,
                    Content = feedbackDto.Content,
                    Status = FeedbackStatus.Approved,
                    AiResult = "Approved",
                    AiReason = aiResult.Data.Reason
                };

                await _unitOfWork.Repository<Feedback, int>().AddAsync(feedback);
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    response.StatusCode = StatusCodes.Status201Created;
                    response.Message = "Feedback created successfully.";
                    response.Data = true;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "An error occurred while creating feedback.";
                    response.Data = false;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback for user {UserId}", userId);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An unexpected error occurred while creating feedback.";
                response.Data = false;
                return response;

            }
        }

        public async Task<GenericResponse<IEnumerable<FeedbackAdminDto>>> GetAllFeedbackForAdminAsync()
        {
            var response = new GenericResponse<IEnumerable<FeedbackAdminDto>>();

            var feedbacks = await _unitOfWork.Repository<Feedback, int>().GetAllAsync(null, null, (f => f.CreatedAt), [f => f.Guest]);

            if (feedbacks == null || !feedbacks.Any())
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "No feedback found.";
                response.Data = null;
                return response;
            }

            var feedbackAdminDtos = feedbacks.Select(f => new FeedbackAdminDto
            {
                Id = f.Id,
                UserEmail = f.Guest.Email!,
                Content = f.Content,
                Status = f.Status.ToString(),
                AiResult = f.AiResult,
                AiReason = f.AiReason,
                CreatedAt = f.CreatedAt
            }).ToList();

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Feedback retrieved successfully.";
            response.Data = feedbackAdminDtos;
            return response;
        }
    }
}
