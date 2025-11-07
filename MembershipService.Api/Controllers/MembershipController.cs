using MembershipService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class MembershipController : Controller
    {
        private IMembershipInfoService _membershipInfoService;
        public MembershipController(IMembershipInfoService membershipInfoService) {
            _membershipInfoService = membershipInfoService;
        
        }

        [HttpGet("membership/skus")]
        public async Task<IActionResult> GetActiveMembership(
            [FromHeader(Name = "X-VTEX-API-AppToken")] string xVtexAPIAppToken,
            [FromHeader(Name = "X-VTEX-API-AppKey")] string xVtexAPIAppKey,
            string status

            )
        {
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
    }
}

