using AutoMapper;
using MembershipService.Api.Controllers;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Common.Mappings;
using MembershipService.Application.DTOs;
using MembershipService.Application.Mapping;
using MembershipService.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MembershipService.Api.Tests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private Mock<ISubscriptionService> _serviceMock;
        private Mock<ILogger<SubscriptionController>> _loggerMock;
        private IMapper _mapper;
        private SubscriptionController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<ISubscriptionService>();
            _loggerMock = new Mock<ILogger<SubscriptionController>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SubscriptionProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _controller = new SubscriptionController(
                _serviceMock.Object,
                _loggerMock.Object,
                _mapper
            );
        }

        [Test]
        public async Task GetPlans_ReturnsOk_WithMappedData()
        {
            var dtoList = new List<SubscriptionDto>
            {
                new SubscriptionDto
                {
                    PlanType = "MONTHLY",
                    Frequency = "1 month",
                    Skus = new List<SkuDto>
                    {
                        new SkuDto
                        {
                            SkuId = "111",
                            Price = 199,
                            Status = "ACTIVE",
                            StockAvailable = true
                        }
                    }
                }
            };

            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtoList);

            var result = await _controller.GetPlans();
            var ok = (OkObjectResult)result;

            var subscriptions = ok.Value.GetType()
                .GetProperty("subscriptions")
                .GetValue(ok.Value) as List<SubscriptionResponseModel>;

            var error = ok.Value.GetType()
                .GetProperty("error")
                .GetValue(ok.Value);

            Assert.That(error, Is.Null);
            Assert.That(subscriptions.Count, Is.EqualTo(1));
            Assert.That(subscriptions[0].PlanType, Is.EqualTo("MONTHLY"));
            Assert.That(subscriptions[0].Skus[0].SkuId, Is.EqualTo("111"));
        }

        [Test]
        public async Task GetPlans_ReturnsMappedData_ForMultiplePlansAndSkus()
        {
            var dtoList = new List<SubscriptionDto>
            {
                new SubscriptionDto
                {
                    PlanType = "MONTHLY",
                    Frequency = "1 month",
                    Skus = new List<SkuDto>
                    {
                        new SkuDto { SkuId = "111", Price = 199, Status = "ACTIVE", StockAvailable = true },
                        new SkuDto { SkuId = "112", Price = 299, Status = "ACTIVE", StockAvailable = false }
                    }
                },
                new SubscriptionDto
                {
                    PlanType = "YEARLY",
                    Frequency = "12 months",
                    Skus = new List<SkuDto>
                    {
                        new SkuDto { SkuId = "999", Price = 999, Status = "ACTIVE", StockAvailable = true }
                    }
                }
            };

            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtoList);

            var result = await _controller.GetPlans();
            var ok = (OkObjectResult)result;

            var subscriptions =
                (List<SubscriptionResponseModel>)ok.Value.GetType()
                .GetProperty("subscriptions")
                .GetValue(ok.Value);

            Assert.That(subscriptions.Count, Is.EqualTo(2));
            Assert.That(subscriptions[0].Skus.Count, Is.EqualTo(2));
            Assert.That(subscriptions[1].PlanType, Is.EqualTo("YEARLY"));
        }

        [Test]
        public async Task GetPlans_WhenServiceReturnsEmpty_ShouldReturnEmptyList()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(new List<SubscriptionDto>());

            var result = await _controller.GetPlans();
            var ok = (OkObjectResult)result;

            var subscriptions =
                (List<SubscriptionResponseModel>)ok.Value.GetType()
                .GetProperty("subscriptions")
                .GetValue(ok.Value);

            Assert.That(subscriptions, Is.Empty);
        }

        [Test]
        public async Task GetPlans_WhenServiceReturnsNull_ShouldReturnEmptyList()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync((List<SubscriptionDto>)null);

            var result = await _controller.GetPlans();
            var ok = (OkObjectResult)result;

            var subscriptions =
                (List<SubscriptionResponseModel>)ok.Value.GetType()
                .GetProperty("subscriptions")
                .GetValue(ok.Value);

            Assert.That(subscriptions, Is.Null.Or.Empty);
        }

        [Test]
        public void GetPlans_WhenServiceTimesOut_ShouldBubbleException()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                .ThrowsAsync(new TaskCanceledException("Timeout occurred"));

            Assert.ThrowsAsync<TaskCanceledException>(async () => await _controller.GetPlans());
        }

        [Test]
        public void GetPlans_WhenServiceThrowsException_ShouldBubbleUp()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                .ThrowsAsync(new Exception("failure"));

            Assert.ThrowsAsync<Exception>(async () => await _controller.GetPlans());
        }

        [Test]
        public async Task GetPlans_ShouldTriggerLogging()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(new List<SubscriptionDto>());

            await _controller.GetPlans();

            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }
    }
}
