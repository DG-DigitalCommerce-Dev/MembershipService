using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Constants;
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
    public class MembershipDataService : IMembershipDataService
    {
        private readonly ILogger<MembershipDataService> _logger;
        private readonly IVtexMembershipRepository _vtexMembershipRepository;
        private readonly IMapper _mapper;
        public MembershipDataService(ILogger<MembershipDataService> logger, IVtexMembershipRepository vtexMembershipRepository, IMapper mapper)
        {
            _logger = logger;
            _vtexMembershipRepository = vtexMembershipRepository;
            _mapper = mapper;
        }
        public async Task<MembershipResponseDto> GetActiveMembershipData(int page)
        {
            _logger.LogInformation(LogMessageConstants.requestingMembershipData);
            var result = await _vtexMembershipRepository.GetActiveMembershipData(page);

            if (result == null)
            {
                return null;
            }

            _logger.LogInformation(LogMessageConstants.membershipInfoReceived);
            
            var membershipDtos = _mapper.Map<IEnumerable<MembershipDto>>(result.Memberships);
            return new MembershipResponseDto(membershipDtos, result.TotalCount, result.PageCount);
        }
    }
}
