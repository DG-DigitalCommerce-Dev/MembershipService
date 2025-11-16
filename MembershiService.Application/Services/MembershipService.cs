using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Common.Models;
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

        public async Task<IEnumerable<MembershipDto>> GetActiveMembershipData()
        {
            _logger.LogInformation(LogMessageConstants.requestingMembershipData);
            var result = await _vtexMembershipRepository.GetActiveMembershipData();

            if (result == null)
            {
                return Enumerable.Empty<MembershipDto>();
            }

            _logger.LogInformation(LogMessageConstants.membershipInfoReceived);
            
            var resultDto = _mapper.Map<IEnumerable<MembershipDto>>(result);
            return resultDto;
        }
    }
}
