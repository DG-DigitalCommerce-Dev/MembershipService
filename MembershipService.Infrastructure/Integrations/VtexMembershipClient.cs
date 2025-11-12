using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MembershipService.Infrastructure.Integrations
{
    public class VtexMembershipClient : IVtexMembershipClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VtexMembershipClient> _logger;
        private static readonly Random rand = new Random();

        public VtexMembershipClient(HttpClient httpClient, ILogger<VtexMembershipClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Function to get membership information of users who have an active subscription
        public async Task<MembershipResponse> GetActiveMembershipInfo(
            string xVtexAPIAppToken,
            string xVtexAPIAppKey,
            string status
        )
        {
            var result = new MembershipResponse();
            int attemptCount = 1;
            const int maxAttempts = 3;
            TimeSpan baseDelay = TimeSpan.FromMilliseconds(200); // Base delay for backoff

            while (attemptCount <= maxAttempts)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://globallogicpartnerus.myvtex.com.br/api/rns/pub/subscriptions?status={status}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("X-VTEX-API-AppToken", xVtexAPIAppToken);
                request.Headers.Add("X-VTEX-API-AppKey", xVtexAPIAppKey);

                try
                {
                    _logger.LogInformation("Calling VTEX endpoint for Membership Information");
                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    result.MembershipInfos = await response.Content.ReadFromJsonAsync<List<MembershipInfo>>();

                    return result; 
                }
                catch (HttpRequestException ex)
                {


                    // If VTEX does not connect after 3 tries - Stop
                    if (attemptCount == maxAttempts)
                    {
                        _logger.LogError(ex,$"Unable get response from VTEX Endpoint, Message = {ex.Message}, Stack Trace = {ex.StackTrace}");
                        result.Error = "SERVICE_UNAVAILABLE";
                        return result;
                    }

                    // Exponential backoff logic
                    int jitterMs = rand.Next(0, (int)(baseDelay.TotalMilliseconds / 2));
                    double backoffMs = Math.Pow(2, attemptCount - 1) * baseDelay.TotalMilliseconds;
                    TimeSpan delay = TimeSpan.FromMilliseconds(backoffMs + jitterMs);
                    await Task.Delay(delay);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unexpected Error when attempting to get membership information from VTEX endpoint, Message = {ex.Message}, Stack Trace = {ex.StackTrace}");
                    result.Error = "INTERNAL_ERROR";
                    return result;
                }

                attemptCount++;
            }

            return result;
        }
    }
}