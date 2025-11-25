using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Constants;
using MembershipService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using MembershipService.Application.Common.Models;
namespace MembershipService.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IVtexSubscriptionClient _vtexClient;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IMapper _mapper;
        public SubscriptionService(IVtexSubscriptionClient vtexClient, ILogger<SubscriptionService> logger, IMapper mapper)
        {
            _vtexClient = vtexClient;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync()
        {
            _logger.LogInformation(LogMessages.FetchingFromVtex);
            var subscriptionResponse = await _vtexClient.GetSubscriptionPlansAsync();
            if (subscriptionResponse == null || subscriptionResponse.Subscriptions.Count == 0)
            {
                return Enumerable.Empty<SubscriptionDto>();
            }
            _logger.LogInformation(LogMessages.TransformingToDto);
            var subscriptionList = _mapper.Map<List<SubscriptionDto>>(subscriptionResponse.Subscriptions);

            _logger.LogInformation(LogMessages.SubscriptionsRetrievedSuccessfully);

            return subscriptionList;
        }

        public async Task<CancelResponseModel> CancelSubscriptionAsync(CancelRequestDto request)
        {
            _logger.LogInformation("Cancel request received for subscription {id}", request.SubscriptionId);

            var subscription = await _vtexClient.GetSubscriptionAsync(request.SubscriptionId);

            if (subscription == null)
                return CancelResponseModel.Error(LogMessageConstants.SubscriptionNotFound, 404);

            if (!subscription.Status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
                return CancelResponseModel.Error("Subscription not ACTIVE", 400);

            bool hasUserUsedTrial = await _vtexClient.HasUserUsedTrialAsync(request.CustomerEmail);

            int remainingDays = 0;

            remainingDays = (subscription.NextPurchaseDate - DateTime.UtcNow).Days;

            bool cancelled = await _vtexClient.CancelSubscriptionAsync(request.SubscriptionId);

            if (!cancelled)
                return CancelResponseModel.Error(LogMessageConstants.VtexCancellationFailed, 500);

            _logger.LogInformation(LogMessages.SubscriptionCancelledSuccessfully, request.SubscriptionId);
            return CancelResponseModel.Success(remainingDays);

        }

        public async Task ProcessTrialExpiryAsync()
        {
            // Logic executed by scheduler
            await Task.CompletedTask;
        }
    }
}