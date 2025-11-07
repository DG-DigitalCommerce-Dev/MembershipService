using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Integrations.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipService.Application.Services
{
    public class MembershipInfoService : IMembershipInfoService
    {
        private ILogger<MembershipInfoService> _logger;
        private VtexMembershipClient _client;
        public MembershipInfoService(ILogger<MembershipInfoService> logger, VtexMembershipClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<MembershipResponse> GetActiveMembershipInfo(
            string xVtexAPIAppToken,
            string xVtexAPIAppKey,
            string status
        )
        {
            try
            {
                var result = await _client.GetActiveMembershipInfo(
                                                xVtexAPIAppToken,
                                                xVtexAPIAppKey,
                                                status
                                           );

                if (result == null || result.MembershipInfos.Count == 0)
                {
                    return new MembershipResponse
                    {
                        Error = "NOT_FOUND"
                    };
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "VTEX API call failed.");
                return new MembershipResponse { Error = "SERVICE_UNAVAILABLE" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in SubscriptionService.");
                return new MembershipResponse { Error = "INTERNAL_ERROR" };
            }
        }
    }
}
