using MembershipService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/")] 
    public class MembershipController : Controller
    {
        private readonly IMembershipInfoService _membershipInfoService;
        private readonly ILogger<MembershipController> _logger;
        public MembershipController(IMembershipInfoService membershipInfoService, ILogger<MembershipController> logger) {
            _membershipInfoService = membershipInfoService;
            _logger = logger;

        }

        [HttpGet("membership/skus")] 
        public async Task<IActionResult> GetActiveMembership(
            [FromHeader(Name = "X-VTEX-API-AppToken")] string xVtexAPIAppToken,
            [FromHeader(Name = "X-VTEX-API-AppKey")] string xVtexAPIAppKey,
            string status

            )
        {
            try
            {
                _logger.LogInformation($"Start processing of {Request.Path.Value} end point");
                var result = await _membershipInfoService.GetActiveMembershipInfo(
                                                        xVtexAPIAppToken,
                                                        xVtexAPIAppKey,
                                                        status
                                                        );

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

                return Ok(result.MembershipInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Issue in retrieving membership information: Message={ex.Message} StackTrace={ex.StackTrace}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

