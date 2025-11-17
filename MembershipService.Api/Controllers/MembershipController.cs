using AutoMapper;
using MembershipService.Api.Models;
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

        [HttpGet("skus")] 
        public async Task<ActionResult<IEnumerable<MembershipResponse>>> GetActiveMembership([FromQuery] int page)
        {
            _logger.LogInformation(LogMessageConstants.processingMembershipInfoEndpoint);
            if (page < 1)
                return BadRequest("Page value should be greater than 0");

            var result = await _membershipDataService.GetActiveMembershipData(page);
            if (result == null) 
                return StatusCode(StatusCodes.Status500InternalServerError,"An unexpected error occurred");
            if (result.Memberships == null || !result.Memberships.Any()) 
                return NotFound("No subscriptions found.");

            var membershipModels = _mapper.Map<IEnumerable<MembershipResponse>>(result.Memberships);

            return Ok(new PaginatedMembershipResponse(membershipModels,result.TotalCount));
        }
    }
}

