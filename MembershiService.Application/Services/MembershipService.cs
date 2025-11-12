using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Interfaces;
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
        private readonly ILogger<MembershipInfoService> _logger;
        private readonly IVtexMembershipClient _client;
        public MembershipInfoService(ILogger<MembershipInfoService> logger, IVtexMembershipClient client)
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

            _logger.LogInformation("Attempting to get Membership information from VTEX endpoint");
            var result = await _client.GetActiveMembershipInfo(
                                            xVtexAPIAppToken,
                                            xVtexAPIAppKey,
                                            status
                                       );
            
            if (result.Error != null) return result;

            _logger.LogInformation($"Membership Information Received from VTEX endpoint");
            
            if (result.MembershipInfos == null || result.MembershipInfos.Count == 0)
            {
                _logger.LogWarning("List of Membership Information from VTEX endpoint is an empty list or null");
                result.Error = "NOT_FOUND";
                return result;
            }

            return result;

        }
    }
}
