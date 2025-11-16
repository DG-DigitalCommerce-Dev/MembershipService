using AutoMapper;
using MembershipService.Application.Common.Models;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Constants;
using MembershipService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipDataService _membershipDataService;
        private readonly ILogger<MembershipController> _logger;
        private readonly IMapper _mapper;
        public MembershipController(IMembershipDataService membershipDataService, ILogger<MembershipController> logger, IMapper mapper)
        {
            _membershipDataService = membershipDataService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("membership/skus")] 
        public async Task<ActionResult<IEnumerable<MembershipResponse>>> GetActiveMembership()
        {
            _logger.LogInformation(LogMessageConstants.processingMembershipInfoEndpoint);
            var result = await _membershipDataService.GetActiveMembershipData();

            if (result == null || !result.Any()) 
            {
                return NotFound("No subscriptions found.");
            }

            var response = _mapper.Map<IEnumerable<MembershipResponse>>(result);
            return Ok(response);
        }
    }
}

