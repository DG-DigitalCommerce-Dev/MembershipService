using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Integrations.Interfaces;
//using MembershipService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace MembershipService.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IVtexClient _vtexClient;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(IVtexClient vtexClient, ILogger<SubscriptionService> logger)
        {
            _vtexClient = vtexClient;
            _logger = logger;
        }

        public async Task<SubscriptionResponse> GetSubscriptionPlansAsync()
        {
            try
            {
                var result = await _vtexClient.GetSubscriptionPlansAsync();

                if (result == null || result.Subscriptions.Count == 0)
                {
                    return new SubscriptionResponse
                    {
                        Subscriptions = new(),
                        Error = "NOT_FOUND"
                    };
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "VTEX API call failed.");
                return new SubscriptionResponse { Error = "SERVICE_UNAVAILABLE" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in SubscriptionService.");
                return new SubscriptionResponse { Error = "INTERNAL_ERROR" };
            }
        }
    }
}

