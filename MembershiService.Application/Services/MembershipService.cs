using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Constants;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using MembershipService.Application.DTOs;
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
        private readonly IVtexMembershipRepository _vtexMembershipRepository;
        public MembershipInfoService(ILogger<MembershipInfoService> logger, IVtexMembershipRepository vtexMembershipRepository)
        {
            _logger = logger;
            _vtexMembershipRepository = vtexMembershipRepository;
        }

        public async Task<IEnumerable<MembershipDto>> GetActiveMembershipInfo()
        {
            _logger.LogInformation(LogMessageConstants.requestingMembershipData);
            var result = await _vtexMembershipRepository.GetActiveMembershipInfo();

            if (result == null)
            {
                return Enumerable.Empty<MembershipDto>();
            }

            _logger.LogInformation(LogMessageConstants.membershipInfoReceived);
            var resultDto = new List<MembershipDto>();
            
            foreach (var membershipInfo in result) 
            {
                resultDto.Add(new MembershipDto(membershipInfo));
            }
            return resultDto;
        }
    }
}
