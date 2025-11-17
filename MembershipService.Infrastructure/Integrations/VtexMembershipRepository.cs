
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Interfaces;
using MembershipService.Infrastructure.Constants;
using MembershipService.Domain.Constants;

namespace MembershipService.Infrastructure.Integrations
{
    public class VtexMembershipRepository : IVtexMembershipRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VtexMembershipRepository> _logger;
        private readonly IConfiguration _config;

        public VtexMembershipRepository(HttpClient httpClient, ILogger<VtexMembershipRepository> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config;
        }
        public async Task<VtexMembershipResponse> GetActiveMembershipData(int page)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_config["baseUrl"]}/api/rns/pub/subscriptions?status=ACTIVE&page={page}");
            request.Headers.Add(VtexConstants.acceptHeader, VtexConstants.acceptHeaderValue); 
            request.Headers.Add(VtexConstants.appTokenHeader, _config["vtexAppToken"]);
            request.Headers.Add(VtexConstants.apiKeyHeader, _config["vtexApiKey"]);

            try
            {
                _logger.LogInformation(LogMessageConstants.callingVtexMembershipApi); 
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                response.Headers.TryGetValues("X-Total-Count", out IEnumerable<string>? totalValues);
                response.Headers.TryGetValues("X-Page-Count", out IEnumerable<string>? pageValues);
                int.TryParse(totalValues?.FirstOrDefault(), out int totalCount);
                int.TryParse(pageValues?.FirstOrDefault(), out int pageCount);
                var membershipDataList = await response.Content.ReadFromJsonAsync<List<MembershipData>>();
                return new VtexMembershipResponse(membershipDataList, totalCount, pageCount); 
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, LogMessageConstants.errorOnVtexMembershipApi);
                return null;
            }
        }            
    }
}
