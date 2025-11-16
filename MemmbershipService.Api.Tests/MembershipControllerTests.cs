using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MembershipService.Api.Controllers;
using MembershipService.Application.Common.Models;
using MembershipService.Application.Common.Interfaces; 
using MembershipService.Application.DTOs; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using NUnit.Framework; 

namespace MembershipService.Api.Tests
{
    [TestFixture] 
    public class MembershipControllerTests
    {
        private Mock<IMembershipDataService> _mockMembershipDataService;
        private Mock<ILogger<MembershipController>> _mockLogger;
        private Mock<IMapper> _mockMapper;
        private MembershipController _controller;

        [SetUp] 
        public void Setup()
        {
            _mockMembershipDataService = new Mock<IMembershipDataService>();
            _mockLogger = new Mock<ILogger<MembershipController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new MembershipController(_mockMembershipDataService.Object, _mockLogger.Object,_mockMapper.Object);
        }

        [Test] 
        public async Task GetActiveMembership_WhenServiceReturnsData_ReturnsOkWithMappedResponse()
        {
            // Arrange
            var mockServiceResult = new List<MembershipDto> { new MembershipDto { Id = "sub_001", CustomerId = "cust_abc", Status = "ACTIVE"} };

            _mockMembershipDataService.Setup(s => s.GetActiveMembershipData()).ReturnsAsync(mockServiceResult);
            _mockMapper.Setup(s => s.Map<IEnumerable<MembershipResponse>>(It.IsAny<IEnumerable<MembershipDto>>()))
                .Returns((IEnumerable<MembershipDto> membershipDto) => new List<MembershipResponse> { new MembershipResponse { Id = membershipDto.First().Id, CustomerId = membershipDto.First().CustomerId, Status = membershipDto.First().Status }});
            // Act
            var actionResult = await _controller.GetActiveMembership();

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult.Result;

            Assert.That(okResult.Value, Is.AssignableTo<IEnumerable<MembershipResponse>>());
            var responseValue = (IEnumerable<MembershipResponse>)okResult.Value;

            Assert.That(responseValue, Has.Exactly(1).Items); 

            var firstItem = responseValue.First();
            Assert.That(firstItem.Id, Is.EqualTo("sub_001")); 
            Assert.That(firstItem.CustomerId, Is.EqualTo("cust_abc"));
            Assert.That(firstItem.Status, Is.EqualTo("ACTIVE"));
        }

        [Test] 
        public async Task GetActiveMembership_WhenServiceReturnsEmptyList_ReturnsNotFound()
        {
            // Arrange
            var mockServiceResult = new List<MembershipDto>();

            _mockMembershipDataService.Setup(s => s.GetActiveMembershipData()).ReturnsAsync(mockServiceResult);

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
            _mockMembershipDataService.Setup(s => s.GetActiveMembershipData()).ReturnsAsync((List<MembershipDto>)null);

            // Act
            var actionResult = await _controller.GetActiveMembership();

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = (NotFoundObjectResult)actionResult.Result;

            Assert.That(notFoundResult.Value, Is.EqualTo("No subscriptions found.")); 
        }
    }
}