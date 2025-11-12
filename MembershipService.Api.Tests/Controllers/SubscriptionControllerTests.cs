using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MembershipService.Api.Controllers;
using MembershipService.Application.Services;
using System.Collections.Generic;
using System;

namespace MembershipService.Api.Tests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private Mock<ISubscriptionService> _mockService;
        private SubscriptionController _controller;

        [SetUp]
        public void Setup()
        {
            // Initialize the Mock Service
            _mockService = new Mock<ISubscriptionService>();
            _controller = new SubscriptionController(_mockService.Object);
        }

        // Helper method to create a realistic response object from the Service
        private List<object> GetMockServiceResponse()
        {
            var mockProduct = new
            {
                RefId = "dg-plus-sub-monthly",
                ProductDetails = new { ShowWithoutStock = true },
                PriceDetails = new { basePrice = 99.99 }
            };
            return new List<object> { mockProduct };
        }

        //=SUCCESS SCENARIO
        [Test]
        public async Task GetSubscriptions_Success_ReturnsOkWithTransformedData()
        {
            // Arrange
            _mockService.Setup(s => s.GetSubscriptionsWithPricingAsync(It.IsAny<List<string>>()))
                        .ReturnsAsync(GetMockServiceResponse());

            // Act
            var result = await _controller.GetSubscriptions();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

           
            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            var parsed = System.Text.Json.JsonDocument.Parse(json).RootElement;

            Assert.That(parsed.TryGetProperty("subscriptions", out var subs), Is.True);
            Assert.That(subs.GetArrayLength(), Is.EqualTo(1));

            var subscription = subs[0];
            Assert.That(subscription.GetProperty("planType").GetString(), Is.EqualTo("MONTHLY"));
            Assert.That(subscription.GetProperty("frequency").GetString(), Is.EqualTo("1 month"));

            var sku = subscription.GetProperty("skus")[0];
            Assert.That(sku.GetProperty("price").GetDouble(), Is.EqualTo(99.99));
            Assert.That(sku.GetProperty("status").GetString(), Is.EqualTo("ACTIVE"));
            Assert.That(sku.GetProperty("stockAvailable").GetBoolean(), Is.True);
        }

        //NOT FOUND SCENARIO
        [Test]
        public async Task GetSubscriptions_ServiceReturnsEmpty_ReturnsNotFound404()
        {
            // Arrange
            _mockService.Setup(s => s.GetSubscriptionsWithPricingAsync(It.IsAny<List<string>>()))
                        .ReturnsAsync(new List<object>());

            // Act
            var result = await _controller.GetSubscriptions();

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));

            // Deserialize
            var json = System.Text.Json.JsonSerializer.Serialize(notFoundResult.Value);
            var parsed = System.Text.Json.JsonDocument.Parse(json).RootElement;

            Assert.That(parsed.GetProperty("error").GetProperty("code").GetString(), Is.EqualTo("NOT_FOUND"));
            Assert.That(parsed.GetProperty("error").GetProperty("message").GetString(), Is.EqualTo("No subscriptions found."));
        }

        // INTERNAL SERVER ERROR (500)
        [Test]
        public async Task GetSubscriptions_ServiceThrowsException_ReturnsInternalServerError500()
        {
            // Arrange
            _mockService.Setup(s => s.GetSubscriptionsWithPricingAsync(It.IsAny<List<string>>()))
                        .ThrowsAsync(new Exception("Database connection pool exhausted."));

            // Act
            var result = await _controller.GetSubscriptions();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));

            // Deserialize
            var json = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
            var parsed = System.Text.Json.JsonDocument.Parse(json).RootElement;

            Assert.That(parsed.GetProperty("error").GetProperty("code").GetString(), Is.EqualTo("SERVICE_UNAVAILABLE"));
            var message = parsed.GetProperty("error").GetProperty("message").GetString();
            Assert.That(message, Does.Contain("Unable to retrieve subscriptions"));
            Assert.That(message, Does.Contain("Database connection pool exhausted."));
        }
    }
}
