//using MembershipService.Domain.Models;
//using MembershipService.Infrastructure.Integrations.Interfaces;
//using MembershipService.Infrastructure.Interfaces;
//using MembershipService.Infrastructure.Models;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Net.Http.Json;

//namespace MembershipService.Infrastructure.Integrations
//{
//    public class VtexClient : IVtexClient
//    {
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<VtexClient> _logger;
//        private readonly VtexSettings _settings;

//        public VtexClient(HttpClient httpClient, IOptions<VtexSettings> options, ILogger<VtexClient> logger)
//        {
//            _httpClient = httpClient;
//            _settings = options.Value;
//            _logger = logger;
//        }

//        public async Task<SubscriptionResponse?> GetSubscriptionPlansAsync()
//        {
//            var endpoint = $"{_settings.BaseUrl}/api/v1/subscription/plans";

//            try
//            {
//                var response = await _httpClient.GetAsync(endpoint);
//                if (!response.IsSuccessStatusCode)
//                {
//                    _logger.LogWarning("VTEX API returned non-success status code: {StatusCode}", response.StatusCode);
//                    return new SubscriptionResponse { Error = "VTEX_ERROR" };
//                }

//                return await response.Content.ReadFromJsonAsync<SubscriptionResponse>();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error calling VTEX API endpoint: {Endpoint}", endpoint);
//                throw;
//            }
//        }
//    }
//}
