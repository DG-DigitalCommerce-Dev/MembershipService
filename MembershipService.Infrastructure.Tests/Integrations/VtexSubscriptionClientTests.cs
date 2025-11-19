using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Tests.TestUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Net;

[TestFixture]
public class VtexSubscriptionClientTests
{
    private Mock<ILogger<VtexSubscriptionClient>> _logger;
    private Mock<IOptions<VtexApiSettings>> _options;
    private VtexApiSettings _settings;

    private const string CatalogUrl = "https://catalog.test/";
    private const string PricingUrl = "https://pricing.test/";
    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<VtexSubscriptionClient>>();
        _settings = new VtexApiSettings
        {
            BaseUrl = CatalogUrl,
            PricingBaseUrl = PricingUrl,
            AppKey = "key",
            AppToken = "token",
            SubscriptionRefs =
            {
                new SubscriptionRefSetting
                {
                    RefId = "dg-plus-sub-monthly",
                    PlanType = "MONTHLY",
                    Frequency = "1 month"
                }
            }
        };
        _options = new Mock<IOptions<VtexApiSettings>>();
        _options.Setup(x => x.Value).Returns(_settings);
    }
    private VtexSubscriptionClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        var mockHandler = new MockHttpMessageHandler(handler);
        var catalogClient = new HttpClient(mockHandler) { BaseAddress = new Uri(CatalogUrl) };

        var client = new VtexSubscriptionClient(catalogClient, _options.Object, _logger.Object);

        typeof(VtexSubscriptionClient)
            .GetField("_pricingClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(client, new HttpClient(mockHandler) { BaseAddress = new Uri(PricingUrl) });

        return client;
    }
    [Test]
    public async Task ReturnsCorrectPrice()
    {
        var product = @"{ ""Id"": ""1"", ""items"": [{ ""Id"": ""100"" }], ""ShowWithoutStock"": true }";
        var price = @"{ ""basePrice"": 200 }";
        var client = CreateClient(req =>
        {
            var url = req.RequestUri!.AbsoluteUri;
            if (url.Contains("productgetbyrefid"))
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(product) };
            if (url.Contains("prices/100"))
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(price) };
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        var result = await client.GetSubscriptionPlansAsync();
        var sku = result!.Subscriptions.First().Skus.First();

        Assert.That(sku.Price, Is.EqualTo(200));
    }
    [Test]
    public async Task MarksInactiveWhenPriceMissing()
    {
        var product = @"{ ""Id"": ""1"", ""items"": [{ ""Id"": ""101"" }] }";

        var client = CreateClient(req =>
        {
            if (req.RequestUri!.AbsoluteUri.Contains("productgetbyrefid"))
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(product) };

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        var result = await client.GetSubscriptionPlansAsync();
        var sku = result!.Subscriptions.First().Skus.First();

        Assert.That(sku.Status, Is.EqualTo("INACTIVE"));
    }
    [Test]
    public async Task ReturnsEmptyWhenProductMissing()
    {
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var result = await client.GetSubscriptionPlansAsync();
        Assert.That(result!.Subscriptions, Is.Empty);
    }
}
