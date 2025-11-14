using MembershipService.Application.Common.Interfaces;
using MembershipService.Api.Models;
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

        public SubscriptionController(ISubscriptionService service, ILogger<SubscriptionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            // Log when API request hits controller
            _logger.LogInformation(LogMessages.RequestReceived);

            var dtoList = await _service.GetAllAsync();

            var response = dtoList.Select(plan => new SubscriptionResponseModel
            {
                PlanType = plan.PlanType,
                Frequency = plan.Frequency,
                Skus = plan.Skus.Select(s => new SkuResponseModel
                {
                    SkuId = s.SkuId,
                    Price = s.Price,
                    Status = s.Status,
                    StockAvailable = s.StockAvailable
                }).ToList()
            }).ToList();

            // Log before sending response
            _logger.LogInformation(LogMessages.SendingResponse);

            return Ok(new { subscriptions = response, error = (string)null });
        }
    }
}
