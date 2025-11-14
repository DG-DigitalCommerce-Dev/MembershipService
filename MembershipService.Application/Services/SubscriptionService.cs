using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Constants;
using MembershipService.Infrastructure.Integrations.Interfaces;
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

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            // Log VTEX fetch starting
            _logger.LogInformation(LogMessages.FetchingFromVtex);

            var domainResponse = await _vtexClient.GetSubscriptionPlansAsync();

            // If no subscriptions returned
            if (domainResponse == null || domainResponse.Subscriptions.Count == 0)
            {
                _logger.LogWarning(LogMessages.NoSubscriptionsFound);
                return Enumerable.Empty<SubscriptionDto>();
            }

            
            _logger.LogInformation(LogMessages.TransformingToDto);

            var dtoList = domainResponse.Subscriptions.Select(plan =>
                new SubscriptionDto
                {
                    PlanType = plan.PlanType,
                    Frequency = plan.Frequency,
                    Skus = plan.Skus.Select(s => new SkuDto
                    {
                        SkuId = s.SkuId,
                        Price = s.Price,
                        Status = s.Status,
                        StockAvailable = s.StockAvailable
                    }).ToList()
                }
            ).ToList();

            return dtoList;
        }
    }
}
