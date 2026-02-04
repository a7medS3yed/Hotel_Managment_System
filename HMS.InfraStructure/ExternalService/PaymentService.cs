using HMS.Core.Contracts;
using HMS.Core.Entities.BookingModule;
using HMS.ServiceAbstraction;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HMS.InfraStructure.ExternalService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId)
        {
            var response = new GenericResponse<string>();

            var booking = await _unitOfWork.Repository<Booking, Guid>()
                .GetByIdAsync(bookingId, null, [b => b.Guest]);

            if (booking == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Booking not found.";
                return response;
            }

            // Here you would integrate with the actual payment gateway to create a payment URL.

            // Get auth token from PayMob
            var authToken = await GetAuthTokenAsync();

            // Create payment order [Intent]
            var orderId = await CreatePaymentOrderAsync(authToken, booking.TotalAmount, booking.Currency);

            if (string.IsNullOrEmpty(orderId))
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Failed to create payment order.";
                return response;
            }

            booking.PaymobOrderId = orderId;

            // Create payment key
            var paymentKey = await CreatePaymentKeyAsync(authToken,
                orderId,
                booking.TotalAmount,
                booking.Currency,
                booking.Guest.Email!,
                booking.Guest.FullName,
                booking.Guest.PhoneNumber!);

            if (paymentKey is null)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Failed to create payment order.";
                return response;
            }

            booking.PaymobPaymentKey = paymentKey;

            _unitOfWork.Repository<Booking, Guid>().Update(booking);
            booking.UpdatedAt = DateTime.Now;

            booking.Status = BookingStatus.Paid;
            booking.PaidDate = DateTime.Now;

            var result = await _unitOfWork.SaveChangesAsync() > 0;
            
            if (result)
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success to create payment url";
                response.Data = $"{_configuration["PayMob:BaseUrl"]}/acceptance/iframes/{_configuration["PayMob:IFrameId"]}?payment_token={paymentKey}";
            }
            else
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Faild to create payment url";
                
            }
            return response;

        }

        private async Task<string> GetAuthTokenAsync()
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["PayMob:BaseUrl"]}/auth/tokens",
                new { api_key = _configuration["PayMob:ApiKey"] });

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("token").GetString()!;
        }

        private async Task<string> CreatePaymentOrderAsync(string authToken, decimal amount, string currency)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["PayMob:BaseUrl"]}/ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = "false",
                    amount_cents = (int)(amount * 100), // Convert to cents
                    currency,
                    items = Array.Empty<object>()
                });
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("id").GetInt32().ToString();
        }

        private async Task<string> CreatePaymentKeyAsync(string authToken, string orderId, decimal amount, string currency, string email, string fullName, string phoneNumber)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["PayMob:BaseUrl"]}/acceptance/payment_keys",
                new
                {
                    auth_token = authToken,
                    amount_cents = (int)(amount * 100), // Convert to cents
                    currency,
                    order_id = orderId,
                    expiration = 3600,
                    integration_id = int.Parse(_configuration["PayMob:IntegrationId"]!),
                    billing_data = new
                    {
                        email,
                        first_name = fullName.Split(' ')[0],
                        last_name = fullName.Split(' ')[1],
                        phone_number = phoneNumber,
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        city = "Cairo",
                        country = "EG",
                        state = "Cairo"
                    },
                }
                );

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("token").GetString()!;
        }

    }
}