using MembershipService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/subscription")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetSubscriptionPlans()
        {
            var result = await _subscriptionService.GetSubscriptionPlansAsync();

            if (!string.IsNullOrEmpty(result.Error))
            {
                return result.Error switch
                {
                    "NOT_FOUND" => NotFound(result),
                    "INVALID_REQUEST" => BadRequest(result),
                    "SERVICE_UNAVAILABLE" => StatusCode(503, result),
                    _ => StatusCode(500, result)
                };
            }

            return Ok(result);
        }
    }
}