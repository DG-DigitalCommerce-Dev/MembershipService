﻿using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Constants;
using MembershipService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
namespace MembershipService.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IVtexSubscriptionClient _vtexClient;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IMapper _mapper;
        public SubscriptionService(IVtexSubscriptionClient vtexClient,ILogger<SubscriptionService> logger,IMapper mapper)
        {
            _vtexClient = vtexClient;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync()
        {
            _logger.LogInformation(LogMessages.FetchingFromVtex);
            var subscriptionresponse = await _vtexClient.GetSubscriptionPlansAsync();
            if (subscriptionresponse == null || subscriptionresponse.Subscriptions.Count == 0)
            {
                return Enumerable.Empty<SubscriptionDto>();
            }
            _logger.LogInformation(LogMessages.TransformingToDto);
            var subscriptionList = _mapper.Map<List<SubscriptionDto>>(subscriptionresponse.Subscriptions);
            return subscriptionList;
        }
    }
}