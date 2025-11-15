using AutoMapper;
using MembershipService.Application.Common.Mappings;
using MembershipService.Application.DTOs;
using MembershipService.Application.Mapping;
using MembershipService.Application.Services;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MembershipService.Application.Tests.Services
{
    [TestFixture]
    public class SubscriptionServiceTests
    {
        private Mock<IVtexSubscriptionClient> _vtexClientMock;
        private Mock<ILogger<SubscriptionService>> _loggerMock;
        private IMapper _mapper;
        private SubscriptionService _service;

        [SetUp]
        public void Setup()
        {
            _vtexClientMock = new Mock<IVtexSubscriptionClient>();
            _loggerMock = new Mock<ILogger<SubscriptionService>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SubscriptionProfile>();
                cfg.AddProfile<DomainToDtoProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
            _service = new SubscriptionService(
                _vtexClientMock.Object,
                _loggerMock.Object,
                _mapper
            );
        }

        [Test]
        public async Task GetAllAsync_ReturnsMappedDtos_WhenDataExists()
        {
            var domainResponse = new SubscriptionResponse
            {
                Subscriptions = new List<SubscriptionPlan>
                {
                    new SubscriptionPlan
                    {
                        PlanType = "MONTHLY",
                        Frequency = "1 month",
                        Skus = new List<Sku>
                        {
                            new Sku { SkuId = "111", Price = 200, Status = "ACTIVE", StockAvailable = true }
                        }
                    }
                }
            };

            _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync())
                .ReturnsAsync(domainResponse);

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);

            var enumerator = result.GetEnumerator();
            enumerator.MoveNext();
            var first = enumerator.Current;

            Assert.That(first.PlanType, Is.EqualTo("MONTHLY"));
            Assert.That(first.Skus[0].SkuId, Is.EqualTo("111"));
        }

        [Test]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoSubscriptions()
        {
            var domainResponse = new SubscriptionResponse
            {
                Subscriptions = new List<SubscriptionPlan>()
            };

            _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync())
                .ReturnsAsync(domainResponse);

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_ReturnsEmpty_WhenResponseIsNull()
        {
            _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync())
                .ReturnsAsync((SubscriptionResponse)null);

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAllAsync_WhenClientThrows_ExceptionBubblesUp()
        {
            _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync())
                .ThrowsAsync(new Exception("VTEX error"));

            Assert.ThrowsAsync<Exception>(async () => await _service.GetAllAsync());
        }

        [Test]
        public void GetAllAsync_WhenClientTimesOut_ShouldBubbleUp()
        {
            _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync())
                .ThrowsAsync(new TaskCanceledException("timeout"));

            Assert.ThrowsAsync<TaskCanceledException>(async () => await _service.GetAllAsync());
        }
    }
}
