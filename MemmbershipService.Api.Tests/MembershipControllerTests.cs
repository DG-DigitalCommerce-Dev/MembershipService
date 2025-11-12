using MembershipService.Api.Controllers;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Integrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MembershipService.Api.Tests
{
    public class MembershipControllerTests
    {
        private readonly Mock<ILogger<VtexMembershipClient>> _mockLogger;

        [Fact]
        public async Task GetActiveMembership_ReturnsOkResult_WhenResultIsValid()
        {
            // Arrange
            var mockService = new Mock<IMembershipInfoService>();
            var mockLogger = new Mock<ILogger<MembershipController>>();

            var expectedMembershipInfos = new List<MembershipInfo> { new MembershipInfo() }; 
            var serviceResult = new MembershipResponse()
            {
                Error = "",
                MembershipInfos = expectedMembershipInfos
            };

            mockService.Setup(s => s.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(serviceResult);

            var controller = new MembershipController(mockService.Object,mockLogger.Object);

            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var path = new PathString("/api/v1/membership/skus");

            request.Setup(r => r.Path).Returns(path);
            httpContext.Setup(h => h.Request).Returns(request.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            
            // Act
            var result = await controller.GetActiveMembership("token", "key", "active");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedMembershipInfos, okResult.Value);
        }

        [Theory]
        [InlineData("NOT_FOUND", StatusCodes.Status404NotFound)]
        [InlineData("INVALID_REQUEST", StatusCodes.Status400BadRequest)]
        [InlineData("SERVICE_UNAVAILABLE", StatusCodes.Status503ServiceUnavailable)]
        [InlineData("SOME_UNKNOWN_ERROR", StatusCodes.Status500InternalServerError)] 
        public async Task GetActiveMembership_ReturnsCorrectStatusCode_BasedOnError(string error, int expectedStatusCode)
        {
            // ARRANGE
            var mockService = new Mock<IMembershipInfoService>();
            var mockLogger = new Mock<ILogger<MembershipController>>();

            var serviceResult = new MembershipResponse()
            {
                Error = error,
                MembershipInfos = null
            };

            mockService
                .Setup(s => s.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(serviceResult);

            var controller = new MembershipController(mockService.Object, mockLogger.Object);
            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var path = new PathString("/api/v1/membership/skus"); // A mock path

            request.Setup(r => r.Path).Returns(path);
            httpContext.Setup(h => h.Request).Returns(request.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            // ACT
            var result = await controller.GetActiveMembership("token", "key", "active");

            // ASSERT
            
            // Checks if the result is of same type or derived type 
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);

            Assert.Equal(expectedStatusCode, objectResult.StatusCode);

            Assert.Equal(serviceResult, objectResult.Value);
        }

        [Fact]
        public async Task GetActiveMembership_Returns500InternalServerError_WhenServiceThrowsException()
        {
            // ARRANGE
            var mockService = new Mock<IMembershipInfoService>();
            var mockLogger = new Mock<ILogger<MembershipController>>();

            var exceptionMessage = "Some Exception!";
            var serviceException = new Exception(exceptionMessage);

            mockService
                .Setup(s => s.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(serviceException);

            var controller = new MembershipController(mockService.Object, mockLogger.Object);
            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var path = new PathString("/api/v1/membership/skus"); 

            request.Setup(r => r.Path).Returns(path);
            httpContext.Setup(h => h.Request).Returns(request.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            // ACT
            var result = await controller.GetActiveMembership("token", "key", "active");

            // ASSERT
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            Assert.Equal(exceptionMessage, objectResult.Value);

            // Verify the error was logged
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Issue in retrieving membership information: Message=Some Exception!")),
                    It.IsAny<Exception>(), 
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

    }
}
