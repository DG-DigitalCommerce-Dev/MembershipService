using AutoMapper;
using MembershipService.Api.Controllers;
using MembershipService.Api.Mappings;
using MembershipService.Api.Models;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<SubscriptionProfile>());
            _mapper = mapperConfig.CreateMapper();

            _controller = new SubscriptionController(_serviceMock.Object, _loggerMock.Object, _mapper);
        }
        [Test]
        public async Task GetPlans_ShouldReturnOk_WithMappedData()
        {
            var dtoList = new List<SubscriptionDto>
            {
                new SubscriptionDto
                {
                    PlanType = "MONTHLY",
                    Frequency = "1 month",
                    Skus = new List<SkuDto>
                    {
                        new SkuDto { SkuId = "111", Price = 199, Status = "ACTIVE", StockAvailable = true }
                    }
                }
            };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtoList);
            var result = await _controller.GetPlans();
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var subs = (List<SubscriptionResponse>)ok.Value.GetType().GetProperty("subscriptions").GetValue(ok.Value);
            Assert.That(subs.Count, Is.EqualTo(1));
            Assert.That(subs[0].PlanType, Is.EqualTo("MONTHLY"));
            Assert.That(subs[0].Skus[0].SkuId, Is.EqualTo("111"));
        }
        [Test]
        public async Task GetPlans_ShouldReturnNotFound_WhenEmpty()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SubscriptionDto>());
            var result = await _controller.GetPlans();
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }
        [Test]
        public async Task GetPlans_ShouldReturnNotFound_WhenNull()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync((IEnumerable<SubscriptionDto>)null);
            var result = await _controller.GetPlans();
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }
        [Test]
        public void GetPlans_ShouldThrowException_WhenServiceFails()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("error"));
            Assert.ThrowsAsync<Exception>(() => _controller.GetPlans());
        }
    }
}
