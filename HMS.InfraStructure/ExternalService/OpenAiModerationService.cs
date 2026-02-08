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
    public class OpenAiModerationService : IAiModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAiModerationService> _logger;

        public OpenAiModerationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<OpenAiModerationService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _configuration["OpenAI:ApiKey"]
                );
        }

        public async Task<(bool IsApproved, string Reason)> AnalyzeAsync(string text)
        {
            var request = new OpenAiModerationRequest
            {
                Model = _configuration["OpenAI:Model"]!,
                Input = text
            };

            int maxRetries = 3;
            int delayMilliseconds = 1000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("moderations", request);

                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (i < maxRetries - 1)
                        {
                            // Check for Retry-After header
                            if (response.Headers.TryGetValues("Retry-After", out var values))
                            {
                                if (int.TryParse(values.First(), out int retryAfter))
                                {
                                    delayMilliseconds = retryAfter * 1000;
                                }
                            }

                            _logger.LogWarning("Rate limit hit. Retrying after {Delay}ms (attempt {Attempt}/{MaxRetries})",
                                delayMilliseconds, i + 1, maxRetries);

                            await Task.Delay(delayMilliseconds);
                            delayMilliseconds *= 2; // Exponential backoff
                            continue;
                        }

                        return (false, "Rate limit exceeded. Please try again later.");
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("OpenAI API error: {StatusCode} - {Error}",
                            response.StatusCode, errorContent);
                        return (false, "AI moderation service failed");
                    }

                    var result = await response.Content.ReadFromJsonAsync<OpenAiModerationResponseDto>();
                    var moderation = result!.Results.First();

                    if (moderation.Flagged)
                        return (false, "Content violates moderation policies");

                    return (true, "Content approved by AI");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calling OpenAI moderation API (attempt {Attempt}/{MaxRetries})",
                        i + 1, maxRetries);

                    if (i == maxRetries - 1)
                        throw;

                    await Task.Delay(delayMilliseconds);
                    delayMilliseconds *= 2;
                }
            }

            return (false, "AI moderation service failed after retries");
        }
    }
}
