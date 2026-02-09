using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.FeedbackDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.ExternalService
{
    public class HuggingFaceModerationService : IAiModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HuggingFaceModerationService> _logger;

        public HuggingFaceModerationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<HuggingFaceModerationService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _configuration["HuggingFace:ApiKey"]
                );
        }

        public async Task<(bool IsApproved, string Reason)> AnalyzeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (false, "Empty content");

            var requestBody = new
            {
                inputs = text
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    _configuration["HuggingFace:ModelUrl"],
                    requestBody
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("HF error: {Error}", error);
                    return (false, "AI moderation service failed");
                }

                var result = await response.Content.ReadFromJsonAsync<List<List<HuggingFaceResultDto>>>();
                var predictions = result!.First();

                var toxicScore = predictions
                    .FirstOrDefault(p => p.Label == "toxic")?.Score ?? 0;

                if (toxicScore > 0.6)
                    return (false, "Content contains offensive or toxic language");

                return (true, "Content approved by AI");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling HuggingFace API");
                return (false, "AI moderation error");
            }
        }
    }

}
