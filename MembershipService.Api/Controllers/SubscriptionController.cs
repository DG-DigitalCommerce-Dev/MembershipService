using AutoMapper;
using MembershipService.Api.Models;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace MembershipService.Api.Controllers
{
    /// <summary>
    /// Handles subscription-related API requests such as retrieving subscription plans.
    /// </summary>
    [ApiController]
    [Route("api/v1/membership")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _service;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionController"/>.
        /// </summary>
        /// <param name="service">The subscription service that retrieves subscription data.</param>
        /// <param name="logger">Logger instance for capturing request flow and errors.</param>
        /// <param name="mapper">AutoMapper instance used to convert DTOs into API response models.</param>
        public SubscriptionController(ISubscriptionService service, ILogger<SubscriptionController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all subscription plans including their SKU and pricing details.
        /// </summary>
        /// <returns>
        /// Returns <see cref="IEnumerable{SubscriptionResponse}"/> when data exists,  
        /// or 404 Not Found when no subscriptions are available.
        /// </returns>
        [HttpGet("skus")]
        public async Task<ActionResult<IEnumerable<SubscriptionResponse>>> Getskus()
        {
            _logger.LogInformation(LogMessages.RequestReceived);
            var result = await _service.GetAllSubscriptionsAsync();
            if (result == null || !result.Any())
            {
                return NotFound("No subscriptions found.");
            }
            var response = _mapper.Map<IEnumerable<SubscriptionResponse>>(result);
            _logger.LogInformation(LogMessages.SendingResponse);
            return Ok(response);
        }
    }
}
