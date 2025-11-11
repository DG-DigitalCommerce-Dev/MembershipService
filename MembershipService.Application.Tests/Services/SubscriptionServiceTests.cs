using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MembershipService.Application.Services;

namespace MembershipService.Application.Tests.Services
{
    public class SubscriptionServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IConfigurationSection> _mockConfigSection;
        private readonly Mock<ILogger<SubscriptionService>> _mockLogger;
        private readonly HttpClient _mockHttpClient;
        private readonly SubscriptionService _service;

        public SubscriptionServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfigSection = new Mock<IConfigurationSection>();
            _mockLogger = new Mock<ILogger<SubscriptionService>>();

            _mockConfigSection.Setup(x => x["BaseUrl"]).Returns("https://dummyapi.vtex.com/");
            _mockConfigSection.Setup(x => x["AppKey"]).Returns("testKey");
            _mockConfigSection.Setup(x => x["AppToken"]).Returns("testToken");
            _mockConfig.Setup(x => x.GetSection("VtexApi")).Returns(_mockConfigSection.Object);

            _mockHttpClient = new HttpClient(new FakeHttpMessageHandler());

            _service = new SubscriptionService(_mockHttpClient, _mockConfig.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetSubscriptionsWithPricingAsync_ShouldReturnEmpty_WhenNoRefIds()
        {
            var refIds = new List<string>();

            var result = await _service.GetSubscriptionsWithPricingAsync(refIds);

            Assert.Empty(result);
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var json = "{}";
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };
                return Task.FromResult(response);
            }
        }
    }
}
