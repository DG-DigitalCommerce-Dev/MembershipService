using Xunit;
using Moq;
using Moq.Protected; // Required for mocking HttpMessageHandler
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; // Required for ReadFromJsonAsync
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Interfaces;
using MembershipService.Domain.Models; 

public class VtexMembershipClientTests
{
    private readonly Mock<ILogger<VtexMembershipClient>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly VtexMembershipClient _client;

    public VtexMembershipClientTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _mockLogger = new Mock<ILogger<VtexMembershipClient>>();
        _client = new VtexMembershipClient(_httpClient, _mockLogger.Object);
    }


    // Helper method to create a successful HttpResponseMessage
    private HttpResponseMessage CreateSuccessResponse(List<MembershipInfo> data)
    {
        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json"
            )
        };
    }


    // Verifies a successful call on the first attempt.
    [Fact]
    public async Task GetActiveMembershipInfo_ReturnsSuccess_OnFirstAttempt()
    {
        // ARRANGE
        var membershipList = new List<MembershipInfo> { new MembershipInfo() };
        var successResponse = CreateSuccessResponse(membershipList);

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(successResponse);

        // ACT
        var result = await _client.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal(membershipList.Count, result.MembershipInfos.Count);

        // Verify the handler was called exactly once
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        // Verify the "Calling" log was made
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Calling VTEX endpoint")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }


    [Fact]
    public async Task GetActiveMembershipInfo_RetriesAndSucceeds_OnHttpError()
    {
        // ARRANGE
        var membershipList = new List<MembershipInfo> { new MembershipInfo() };
        var successResponse = CreateSuccessResponse(membershipList);

        // Setup a sequence of responses from the handler
        _mockHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // First call: throws HttpRequestException (simulating a 503 or network error)
            .ThrowsAsync(new HttpRequestException("Simulated network error"))
            // Second call: returns a successful response
            .ReturnsAsync(successResponse);

        // ACT
        var result = await _client.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal(membershipList.Count, result.MembershipInfos.Count);

        // Verify the handler was called exactly twice (1 fail, 1 success)
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(2),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        // Verify no error was logged (since it succeeded)
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }


    [Fact]
    public async Task GetActiveMembershipInfo_ReturnsServiceUnavailable_AfterMaxAttempts()
    {
        // ARRANGE
        // Setup the handler to always throw an exception
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Simulated network error"));

        // ACT
        var result = await _client.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("SERVICE_UNAVAILABLE", result.Error);
        Assert.Null(result.MembershipInfos);

        // Verify the handler was called 3 times (the maxAttempts)
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(3),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        // Verify the error was logged on the final attempt
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Unable get response from VTEX Endpoint")),
                It.IsAny<HttpRequestException>(), // Check that it's an HttpRequestException
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetActiveMembershipInfo_ReturnsInternalError_OnUnexpectedException()
    {
        // ARRANGE
        // Create a response that is 200 OK, but has malformed JSON.
        // This will cause ReadFromJsonAsync() to throw a JsonException.
        var malformedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                "{not-valid-json]", // Malformed content
                Encoding.UTF8,
                "application/json"
            )
        };

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(malformedResponse);

        // ACT
        var result = await _client.GetActiveMembershipInfo("token", "key", "active");

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("INTERNAL_ERROR", result.Error);
        Assert.Null(result.MembershipInfos);

        // Verify the handler was called only once (it doesn't retry on this error)
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        // Verify the "Unexpected Error" was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Unexpected Error")),
                It.IsAny<System.Text.Json.JsonException>(), // Check it was a JsonException
                It.IsAny<Func<It.IsAnyType, Exception, string>>() ),
            Times.Once);
    }
}