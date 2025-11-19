using NUnit.Framework;
using Moq;
using Moq.Protected; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Interfaces; 
using MembershipService.Infrastructure.Constants; 
using MembershipService.Domain.Constants; 
using MembershipService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json; 
using System.Threading;
using System.Threading.Tasks;

namespace MembershipService.Infrastructure.Tests
{
    [TestFixture]
    public class VtexMembershipRepositoryTests
    {
        private Mock<ILogger<VtexMembershipRepository>> _mockLogger;
        private Mock<IConfiguration> _mockConfig;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private VtexMembershipRepository _repository;

        private const string FakeBaseUrl = "https://fake-vtex.com";
        private const string FakeAppToken = "test-token-123";
        private const string FakeApiKey = "test-key-456";

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<VtexMembershipRepository>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.SetupGet(c => c["baseUrl"]).Returns(FakeBaseUrl);
            _mockConfig.SetupGet(c => c["vtexAppToken"]).Returns(FakeAppToken);
            _mockConfig.SetupGet(c => c["vtexApiKey"]).Returns(FakeApiKey);
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _repository = new VtexMembershipRepository(_httpClient,_mockLogger.Object,_mockConfig.Object);
        }

        [Test]
        public async Task GetActiveMembershipData_WhenApiCallSucceeds_ReturnsParsedResponse()
        {
            // Arrange
            int page = 1;
            int expectedTotalCount = 2;
            string expectedUrl = $"{FakeBaseUrl}/api/rns/pub/subscriptions?status=ACTIVE&page={page}";

            var membershipData = new List<MembershipData>
            {
                new MembershipData { Id = "123", CustomerEmail = "test1@email.com" },
                new MembershipData { Id = "123", CustomerEmail = "test2@email.com" }
            };
            var jsonContent = JsonSerializer.Serialize(membershipData);

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };
            httpResponse.Headers.Add("X-Total-Count", expectedTotalCount.ToString());

            HttpRequestMessage capturedRequest = null;
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _repository.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<VtexMembershipResponse>());
            Assert.That(result.TotalCount, Is.EqualTo(expectedTotalCount));
            Assert.That(result.MembershipList.Count(), Is.EqualTo(2));
            Assert.That(result.MembershipList.First().Id, Is.EqualTo("123"));
            Assert.That(result.MembershipList.First().CustomerEmail, Is.EqualTo("test1@email.com"));
        }

        [Test]
        public async Task GetActiveMembershipData_WhenApiReturnsNonSuccess_ReturnsNull()
        {
            // Arrange
            int page = 1;
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
            
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _repository.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetActiveMembershipData_WhenHttpClientThrowsException_LogsErrorAndReturnsNull()
        {
            // Arrange
            int page = 1;
            var expectedException = new HttpRequestException("Simulated network failure");

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(expectedException);

            // Act
            var result = await _repository.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Null);
            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(LogMessageConstants.ErrorOnVtexMembershipApi)),
                    It.Is<HttpRequestException>(ex => ex == expectedException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task GetActiveMembershipData_WhenTotalCountHeaderIsMissing_ReturnsNull()
        {
            // Arrange
            int page = 1;
            var membershipData = new List<MembershipData> { new MembershipData { Id = "123" } };
            var jsonContent = JsonSerializer.Serialize(membershipData);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _repository.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetActiveMembershipData_WhenTotalCountHeaderIsInvalid_ReturnsNull()
        {
            // Arrange
            int page = 1;
            var membershipData = new List<MembershipData> { new MembershipData { Id = "123" } };
            var jsonContent = JsonSerializer.Serialize(membershipData);

            var httpResponse = new HttpResponseMessage{StatusCode = HttpStatusCode.OK,Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")};
            httpResponse.Headers.Add("X-Total-Count", "abc-not-a-number");

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _repository.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}