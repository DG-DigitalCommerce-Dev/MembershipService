using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MembershipService.Api.Controllers;
using MembershipService.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MembershipService.Api.Tests.Controllers
{
    public class SubscriptionControllerTests
    {
        private readonly Mock<SubscriptionService> _mockService;
        private readonly SubscriptionController _controller;

        public SubscriptionControllerTests()
        {
            // Mock the SubscriptionService dependency
            _mockService = new Mock<SubscriptionService>(null, null, null);
            _controller = new SubscriptionController(_mockService.Object);
        }

        [Fact]
        public async Task GetSubscriptions_ReturnsOk_WhenDataExists()
        {
            // Arrange
            var fakeResponse = new List<object>
            {
                new { RefId = "dg-plus-sub-monthly", PriceDetails = new { basePrice = 120.0 } }
            };

            _mockService.Setup(s => s.GetSubscriptionsWithPricingAsync(It.IsAny<List<string>>()))
                        .ReturnsAsync(fakeResponse);

            // Act
            var result = await _controller.GetSubscriptions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetSubscriptions_ReturnsNotFound_WhenNoData()
        {
            // Arrange
            _mockService.Setup(s => s.GetSubscriptionsWithPricingAsync(It.IsAny<List<string>>()))
                        .ReturnsAsync(new List<object>());

            // Act
            var result = await _controller.GetSubscriptions();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = notFoundResult.Value as dynamic;

            Assert.Equal("NOT_FOUND", value.error.code);
        }

        [Fact]
        public async Task GetSubscriptions_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockService.Setup(s => s.GetSubscriptionsWithPricingAsync(It.IsAny<List<string>>()))
                        .ThrowsAsync(new Exception("VTEX API timeout"));

            // Act
            var result = await _controller.GetSubscriptions();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
