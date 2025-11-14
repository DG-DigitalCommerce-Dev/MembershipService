using MembershipService.Application.Common.Interfaces;
using MembershipService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using MembershipService.Domain.Constants;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipInfoService _membershipInfoService;
        private readonly ILogger<MembershipController> _logger;
        private readonly IConfiguration _config;
        public MembershipController(IMembershipInfoService membershipInfoService, ILogger<MembershipController> logger)
        {
            _membershipInfoService = membershipInfoService;
            _logger = logger;
        }

        [HttpGet("skus")] // Endpoint name 
        public async Task<ActionResult<IEnumerable<SubscriptionResponse>>> GetActiveMembership()
        {
            _logger.LogInformation(LogMessageConstants.processingMembershipInfoEndpoint);
            var result = await _membershipInfoService.GetActiveMembershipInfo();

            if (result == null || !result.Any()) 
            {
                return NotFound("No subscriptions found.");
            }

            var response = result.Select(dto => new SubscriptionResponse
            {
                Id = dto.Id,
                Customer = dto.CustomerId,
                Status = dto.Status,
                PlanId = dto.PlanDto.Id

            });
            return Ok(response);
        }
    }
}

