using AutoMapper;
using MembershipService.Application.DTOs;
using MembershipService.Application.Services;
using MembershipService.Domain.Models;
using MembershipService.Application.Mapping;
using MembershipService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

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
            cfg.AddProfile<DomainToDtoProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        _service = new SubscriptionService(_vtexClientMock.Object, _loggerMock.Object, _mapper);
    }
    [Test]
    public async Task ReturnsMappedData()
    {
        var domainResponse = new Subscription
        {
            Subscriptions = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    PlanType = "MONTHLY",
                    Frequency = "1 month",
                    Skus = new List<Sku>
                    {
                        new Sku { SkuId = "111", Price = 200, Status = "ACTIVE", IsStockAvailable = true }
                    }
                }
            }
        };

        _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync()).ReturnsAsync(domainResponse);

        var result = await _service.GetAllSubscriptionsAsync();
        var list = new List<SubscriptionDto>(result);
        Assert.That(list.Count, Is.EqualTo(1));
        Assert.That(list[0].PlanType, Is.EqualTo("MONTHLY"));
        Assert.That(list[0].Skus[0].SkuId, Is.EqualTo("111"));
    }
    [Test]
    public async Task ReturnsEmptyList()
    {
        _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync())
            .ReturnsAsync(new Subscription { Subscriptions = new List<SubscriptionPlan>() });
        var result = await _service.GetAllSubscriptionsAsync();
        Assert.That(result, Is.Empty);
    }
    [Test]
    public async Task ReturnsEmptyWhenNull()
    {
        _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync()).ReturnsAsync((Subscription)null);
        var result = await _service.GetAllSubscriptionsAsync();
        Assert.That(result, Is.Empty);
    }
    [Test]
    public void ThrowsWhenClientFails()
    {
        _vtexClientMock.Setup(x => x.GetSubscriptionPlansAsync()).ThrowsAsync(new Exception("error"));
        Assert.ThrowsAsync<Exception>(() => _service.GetAllSubscriptionsAsync());
    }
}