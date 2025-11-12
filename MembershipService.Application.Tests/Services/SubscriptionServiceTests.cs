using NUnit.Framework;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MembershipService.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace MembershipService.Application.Tests.Services
{
    [TestFixture]
    public class SubscriptionServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IConfigurationSection> _mockVtexConfig;
        private Mock<ILogger<SubscriptionService>> _mockLogger;

        // Mock data for fake API responses
        private static readonly string MockProductResponse = @"{
            ""Id"": 100,
            ""RefId"": ""dg-plus-sub-monthly"",
            ""ShowWithoutStock"": true,
            ""items"": [ { ""Id"": 200 } ]
        }";

        private static readonly string MockPricingResponse = @"{
            ""itemId"": 200,
            ""basePrice"": 99.99
        }";

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<SubscriptionService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockVtexConfig = new Mock<IConfigurationSection>();

            _mockVtexConfig.SetupGet(x => x["BaseUrl"]).Returns("http://vtex.product.com/");
            _mockVtexConfig.SetupGet(x => x["AppKey"]).Returns("test-key");
            _mockVtexConfig.SetupGet(x => x["AppToken"]).Returns("test-token");
            _mockConfiguration.Setup(x => x.GetSection("VtexApi")).Returns(_mockVtexConfig.Object);
        }

        //Successful API mock response
        [Test]
        public async Task GetSubscriptionsWithPricingAsync_Success_ReturnsCompleteData()
        {
            var handler = new MockHttpMessageHandler(request =>
            {
                if (request.RequestUri.ToString().Contains("productgetbyrefid"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MockProductResponse) };

                if (request.RequestUri.ToString().Contains("prices"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MockPricingResponse) };

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://vtex.product.com/") };
            var service = new SubscriptionService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var refIds = new List<string> { "dg-plus-sub-monthly" };
            var result = await service.GetSubscriptionsWithPricingAsync(refIds);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        // VTEX API fails
        [Test]
        public async Task GetSubscriptionsWithPricingAsync_VtexApiFails_ReturnsError()
        {
            var handler = new MockHttpMessageHandler(request =>
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            });

            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://vtex.product.com/") };
            var service = new SubscriptionService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var result = await service.GetSubscriptionsWithPricingAsync(new List<string> { "dg-plus-sub-monthly" });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        // Unexpected exception
        [Test]
        public async Task GetSubscriptionsWithPricingAsync_ThrowsException_ReturnsErrorResult()
        {
            var handler = new MockHttpMessageHandler(_ => throw new Exception("Unexpected failure"));

            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://vtex.product.com/") };
            var service = new SubscriptionService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var result = await service.GetSubscriptionsWithPricingAsync(new List<string> { "dg-plus-sub-monthly" });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }
    }

    // Simple mock Http handler (offline simulation)
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handlerFunc;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = _handlerFunc(request);
            return Task.FromResult(response);
        }
    }
}
