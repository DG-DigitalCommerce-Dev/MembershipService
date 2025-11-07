using MembershipService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MembershipService.Infrastructure.Integrations
{
    public class VtexMembershipClient
    {
        private readonly HttpClient _httpClient;
        private static readonly Random rand = new Random();

        public VtexMembershipClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    result.MembershipInfos = await response.Content.ReadFromJsonAsync<List<MembershipInfo>>();

                    return result; // Success! Exit the method.
                }
                catch (HttpRequestException ex)
                {

                    // If VTEX does not connect after 3 tries - Stop
                    if (attemptCount == maxAttempts) throw;

                    // --- This is the exponential backoff logic ---
                    int jitterMs = rand.Next(0, (int)(baseDelay.TotalMilliseconds / 2));
                    double backoffMs = Math.Pow(2, attemptCount - 1) * baseDelay.TotalMilliseconds;
                    TimeSpan delay = TimeSpan.FromMilliseconds(backoffMs + jitterMs);
                    await Task.Delay(delay);

                }
                catch (Exception ex)
                {
                    throw; // Unexpected Error
                }

                attemptCount++;
            }

            return result;
        }
    }
}