// --- Required Usings ---
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// --- Your Project's Namespaces (assuming these) ---
// Make sure these 'using' statements match your project's structure
using MembershipService.Application.Services;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Interfaces; // For IVtexMembershipClient

/// <summary>
/// Contains all unit tests for the MembershipInfoService.
/// </summary>
public class MembershipInfoServiceTests
{
    private readonly Mock<ILogger<MembershipInfoService>> _mockLogger;
    private readonly Mock<IVtexMembershipClient> _mockClient;
    private readonly MembershipInfoService _service;

    public MembershipInfoServiceTests()
    {
        _mockLogger = new Mock<ILogger<MembershipInfoService>>();
        _mockClient = new Mock<IVtexMembershipClient>();
        _service = new MembershipInfoService(_mockLogger.Object, _mockClient.Object);
    }

    [Fact]
    public async Task GetActiveMembershipInfo_ReturnsSuccessfulResult_WhenClientProvidesValidData()
    {
        // ARRANGE

        var membershipList = new List<MembershipInfo> { new MembershipInfo() };
        var clientResponse = new MembershipResponse
        {
            Error = null,
            MembershipInfos = membershipList
        };

        _mockClient.Setup(c => c.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(clientResponse);

        // ACT
        var result = await _service.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal(membershipList, result.MembershipInfos);

        // Verify informational logs.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Attempting to get Membership information")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Membership Information Received")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Verify no warning was logged.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }


    [Theory] // WE CAN USE THEORY HERE
    [InlineData("SERVICE_UNAVAILABLE")]
    [InlineData("INTERNAL_ERROR")]
    public async Task GetActiveMembershipInfo_ReturnsClientError_WhenClientReturnsError(string error)
    {
        // ARRANGE
        var clientResponse = new MembershipResponse
        {
            Error = error,
            MembershipInfos = null
        };

        _mockClient.Setup(c => c.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(clientResponse);

        // ACT
        var result = await _service.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(error, result.Error);
        Assert.Null(result.MembershipInfos);

        // Verify the "Attempting" log was called.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Attempting to get Membership information")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Verify the "Received" log was *NOT* called, as the method returned early.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Membership Information Received")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetActiveMembershipInfo_ReturnsNotFound_WhenClientReturnsEmptyList()
    {
        // ARRANGE
        var clientResponse = new MembershipResponse
        {
            Error = null,
            MembershipInfos = new List<MembershipInfo>() 
        };

        _mockClient.Setup(c => c.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(clientResponse);

        // ACT
        var result = await _service.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("NOT_FOUND", result.Error);
        Assert.NotNull(result.MembershipInfos); 
        Assert.Empty(result.MembershipInfos); // Check if the list is empty

        // Verify the Warning log was called.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("List of Membership Information from VTEX endpoint is an empty list or null")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetActiveMembershipInfo_ReturnsNotFound_WhenClientReturnsNullMemberships()
    {
        // ARRANGE
        var clientResponse = new MembershipResponse
        {
            Error = null,
            MembershipInfos = null // Null list
        };

        // 2. Setup the mock client.
        _mockClient.Setup(c => c.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(clientResponse);

        // ACT
        var result = await _service.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("NOT_FOUND", result.Error);
        Assert.Null(result.MembershipInfos);

        // Verify the Warning log was called.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("List of Membership Information from VTEX endpoint is an empty list or null")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 5: The "Unhappy Path" (Exception)
    /// Verifies that if the client *throws an exception*, the service
    /// does *not* catch it and lets it propagate up.
    /// </summary>
    [Fact]
    public async Task GetActiveMembershipInfo_ThrowsException_WhenClientThrowsException()
    {
        // ARRANGE
        // 1. Define the exception.
        var clientException = new Exception("Client connection failed!");

        // 2. Setup the mock client to *throw* the exception.
        _mockClient.Setup(c => c.GetActiveMembershipInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(clientException);

        // ACT & ASSERT
        // 1. Verify that the *exact* exception is thrown by the service.
        //    The service's code does not have a try-catch, so the exception
        //    should bubble up.
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _service.GetActiveMembershipInfo("token", "key", "active")
        );

        // 2. Check that it is the same exception.
        Assert.Equal(clientException.Message, ex.Message);

        // 3. Verify only the "Attempting" log was called.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Attempting to get Membership information")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // 4. Verify no other logs were called.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Membership Information Received")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }
}