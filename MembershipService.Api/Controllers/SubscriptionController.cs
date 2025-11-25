using AutoMapper;
using MembershipService.Api.Models;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using MembershipService.Application.DTOs;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/subscription")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _service;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IMapper _mapper;

        public SubscriptionController(ISubscriptionService service, ILogger<SubscriptionController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("plans")]
        public async Task<ActionResult<IEnumerable<SubscriptionResponse>>> GetPlans()
        {
            _logger.LogInformation(LogMessages.RequestReceived);
            var result = await _service.GetAllSubscriptionsAsync();

            if (result == null || !result.Any())
                return NotFound(LogMessageConstants.NoSubscriptionsFound);

            var response = _mapper.Map<IEnumerable<SubscriptionResponse>>(result);
            _logger.LogInformation(LogMessages.SendingResponse);
            return Ok(response);
        }
        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation(LogMessageConstants.ProcessingCancelRequest, request.SubscriptionId);
            var response = await _service.CancelSubscriptionAsync(request);

            _logger.LogInformation(LogMessageConstants.CancelRequestProcessed, request.SubscriptionId);
            return StatusCode(response.HttpCode, response);
        }
    }
}
