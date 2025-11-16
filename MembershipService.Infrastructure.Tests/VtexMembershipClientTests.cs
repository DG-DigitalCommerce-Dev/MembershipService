using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MembershipService.Domain.Models; 
using MembershipService.Infrastructure.Integrations; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace MembershipService.Infrastructure.Tests
{
    [TestFixture] 
    public class VtexMembershipRepositoryNUnitTests
    {
        private Mock<ILogger<VtexMembershipRepository>> _mockLogger;
        private Mock<IConfiguration> _mockConfig;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private VtexMembershipRepository _repository;

        private const string _baseUrl = "https://fake.vtex.api";
        private const string _appToken = "fake-token";
        private const string _apiKey = "fake-key";

        [SetUp] 
        public void Setup()
        {
            // 1. Mock Dependencies
            _mockLogger = new Mock<ILogger<VtexMembershipRepository>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // 2. Setup HttpClient with the mocked handler
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            // 3. Setup Configuration
            _mockConfig.Setup(c => c["baseUrl"]).Returns(_baseUrl); 
            _mockConfig.Setup(c => c["vtexAppToken"]).Returns(_appToken);
            _mockConfig.Setup(c => c["vtexApiKey"]).Returns(_apiKey);

            // 4. Instantiate the class under test
            _repository = new VtexMembershipRepository(_httpClient, _mockLogger.Object, _mockConfig.Object);
        }

        [Test] 
        public async Task GetActiveMembershipData_WhenApiReturnsSuccess_ReturnsMembershipData()
        {
            // Arrange 
            var expectedData = new List<MembershipData> { new MembershipData() { Id = "1" }, new MembershipData() { Id = "2" } };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(expectedData) };
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _repository.GetActiveMembershipData();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.AssignableTo<IEnumerable<MembershipData>>());
        }

        [Test] 
        public async Task GetActiveMembershipData_WhenApiReturnsFailure_ReturnsNull()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _repository.GetActiveMembershipData();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test] 
        public async Task GetActiveMembershipData_WhenHttpClientThrowsException_ReturnsNull()
        {
            // Arrange
            var apiException = new HttpRequestException("Network error");
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(apiException); 

            // Act
            var result = await _repository.GetActiveMembershipData(); 

            // Assert
            Assert.That(result, Is.Null); 
        }
    }
}