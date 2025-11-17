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
        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            _logger.LogInformation(LogMessages.FetchingFromVtex);
            var domainResponse = await _vtexClient.GetSubscriptionPlansAsync();
            if (domainResponse == null || domainResponse.Subscriptions.Count == 0)
            {
                _logger.LogWarning(LogMessages.NoSubscriptionsFound);
                return Enumerable.Empty<SubscriptionDto>();
            }
            _logger.LogInformation(LogMessages.TransformingToDto);
            var SubscriptiondtoList = _mapper.Map<List<SubscriptionDto>>(domainResponse.Subscriptions);
            return SubscriptiondtoList;
        }
    }
}