using AutoMapper;
using MembershipService.Api.Controllers;
using MembershipService.Api.Models;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _controller = new MembershipController(_mockMembershipDataService.Object,_mockLogger.Object,_mockMapper.Object);
        }

        [Test]
        public async Task GetActiveMembership_WhenPageIsLessThanOne_ReturnsBadRequest()
        {
            // Arrange
            int invalidPage = 0;

            // Act
            var result = await _controller.GetActiveMembership(invalidPage);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Page value should be greater than 0"));
        }

        [Test]
        public async Task GetActiveMembership_WhenServiceReturnsNull_ReturnsInternalServerError()
        {
            // Arrange
            int page = 1;
            _mockMembershipDataService.Setup(s => s.GetActiveMembershipData(page)).ReturnsAsync((MembershipResponseDto)null);

            // Act
            var result = await _controller.GetActiveMembership(page);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(objectResult.Value, Is.EqualTo("An unexpected error occurred"));
        }

        [Test]
        public async Task GetActiveMembership_WhenNoMembershipsFound_ReturnsNotFound()
        {
            // Arrange
            int page = 1;
            var emptyServiceResult = new MembershipResponseDto(new List<MembershipDto>(), 0);
            _mockMembershipDataService.Setup(s => s.GetActiveMembershipData(page)).ReturnsAsync(emptyServiceResult);

            // Act
            var result = await _controller.GetActiveMembership(page);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("No subscriptions found."));
        }

        [Test]
        public async Task GetActiveMembership_WhenMembershipsListIsNull_ReturnsNotFound()
        {
            // Arrange
            int page = 1;
            var nullListServiceResult = new MembershipResponseDto(null, 0);
            _mockMembershipDataService.Setup(s => s.GetActiveMembershipData(page)).ReturnsAsync(nullListServiceResult);

            // Act
            var result = await _controller.GetActiveMembership(page);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetActiveMembership_WhenMembershipsFound_ReturnsOkWithPaginatedResponse()
        {
            // Arrange
            int page = 1;
            int totalCount = 2;
            var membershipDtoList = new List<MembershipDto>
            {
                new MembershipDto { Id = "123", CustomerEmail = "test1@example.com" },
                new MembershipDto { Id = "123", CustomerEmail = "test2@example.com" }
            };
            var serviceResult = new MembershipResponseDto(membershipDtoList,totalCount);

            var mappedResponses = new List<MembershipResponse>
            {
                new MembershipResponse { Id = membershipDtoList[0].Id, CustomerEmail = membershipDtoList[0].CustomerEmail },
                new MembershipResponse { Id = membershipDtoList[1].Id, CustomerEmail = membershipDtoList[1].CustomerEmail }
            };

            _mockMembershipDataService.Setup(s => s.GetActiveMembershipData(page)).ReturnsAsync(serviceResult);
            _mockMapper.Setup(m => m.Map<IEnumerable<MembershipResponse>>(membershipDtoList)).Returns(mappedResponses);

            // Act
            var result = await _controller.GetActiveMembership(page);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<PaginatedMembershipResponse>());
            var paginatedResponse = okResult.Value as PaginatedMembershipResponse;
            Assert.Multiple(() =>
            {
                Assert.That(paginatedResponse.TotalCount, Is.EqualTo(totalCount));
                Assert.That(paginatedResponse.MembershipList.Count(), Is.EqualTo(2));
                Assert.That(paginatedResponse.MembershipList, Is.SameAs(mappedResponses));
            });
        }
    }
}
