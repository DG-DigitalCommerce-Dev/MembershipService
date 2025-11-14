using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MembershipService.Api.Controllers; 
using MembershipService.Api.Models; 
using MembershipService.Application.Common.Interfaces; 
using MembershipService.Application.DTOs; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework; 

namespace MembershipService.Api.Tests
{
    [TestFixture] 
    public class MembershipControllerTests
    {
        private Mock<IMembershipInfoService> _mockMembershipInfoService;
        private Mock<ILogger<MembershipController>> _mockLogger;
        private MembershipController _controller;

        [SetUp] 
        public void Setup()
        {
            _mockMembershipInfoService = new Mock<IMembershipInfoService>();
            _mockLogger = new Mock<ILogger<MembershipController>>();
            _controller = new MembershipController(_mockMembershipInfoService.Object, _mockLogger.Object);
        }

        [Test] 
        public async Task GetActiveMembership_WhenServiceReturnsData_ReturnsOkWithMappedResponse()
        {
            // Arrange
            var mockPlanDto = new PlanDto { Id = "plan_basic_123" };
            var mockServiceResult = new List<MembershipDto> { new MembershipDto { Id = "sub_001", CustomerId = "cust_abc", Status = "active", PlanDto = mockPlanDto } };

            _mockMembershipInfoService.Setup(s => s.GetActiveMembershipInfo()).ReturnsAsync(mockServiceResult);

            // Act
            var actionResult = await _controller.GetActiveMembership();

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult.Result;

            Assert.That(okResult.Value, Is.AssignableTo<IEnumerable<SubscriptionResponse>>());
            var responseValue = (IEnumerable<SubscriptionResponse>)okResult.Value;

            Assert.That(responseValue, Has.Exactly(1).Items); 

            var firstItem = responseValue.First();
            Assert.That(firstItem.Id, Is.EqualTo("sub_001")); 
            Assert.That(firstItem.Customer, Is.EqualTo("cust_abc"));
            Assert.That(firstItem.Status, Is.EqualTo("active"));
            Assert.That(firstItem.PlanId, Is.EqualTo("plan_basic_123"));
        }

        [Test] // <-- Replaces [Fact]
        public async Task GetActiveMembership_WhenServiceReturnsEmptyList_ReturnsNotFound()
        {
            // Arrange
            var mockServiceResult = new List<MembershipDto>();

            _mockMembershipInfoService.Setup(s => s.GetActiveMembershipInfo()).ReturnsAsync(mockServiceResult);

            // Act
            var actionResult = await _controller.GetActiveMembership();

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = (NotFoundObjectResult)actionResult.Result;

            Assert.That(notFoundResult.Value, Is.EqualTo("No subscriptions found.")); 
        }

        [Test] 
        public async Task GetActiveMembership_WhenServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            _mockMembershipInfoService.Setup(s => s.GetActiveMembershipInfo()).ReturnsAsync((List<MembershipDto>)null);

            // Act
            var actionResult = await _controller.GetActiveMembership();

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = (NotFoundObjectResult)actionResult.Result;

            Assert.That(notFoundResult.Value, Is.EqualTo("No subscriptions found.")); // Replaces Assert.Equal
        }
    }
}