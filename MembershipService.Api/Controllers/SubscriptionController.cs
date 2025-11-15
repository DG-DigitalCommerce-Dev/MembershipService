using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Models;
using MembershipService.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/subscription")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _service;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IMapper _mapper;

        public SubscriptionController(
            ISubscriptionService service,
            ILogger<SubscriptionController> logger,
            IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            _logger.LogInformation(LogMessages.RequestReceived);

            var dtoList = await _service.GetAllAsync();

            var response = _mapper.Map<List<SubscriptionResponseModel>>(dtoList);

            _logger.LogInformation(LogMessages.SendingResponse);

            return Ok(new { subscriptions = response, error = (string)null });
        }
    }
}
