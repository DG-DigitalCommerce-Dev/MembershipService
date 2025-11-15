//using MembershipService.Domain.Models;
//using MembershipService.Infrastructure.Integrations;
//using MembershipService.Infrastructure.Tests.TestUtils;
//using MembershipService.Infrastructure.Interfaces;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace MembershipService.Infrastructure.Tests.Integrations
//{
//    [TestFixture]
//    public class VtexSubscriptionClientTests
//    {
//        private Mock<ILogger<VtexSubscriptionClient>> _loggerMock = null!;
//        private Mock<IOptions<VtexApiSettings>> _optionsMock = null!;
//        private VtexApiSettings _settings = null!;

//        private const string BaseUrl = "https://fake-catalog.com/";
//        private const string PricingBaseUrl = "https://api.vtex.com/globallogicpartnerus/pricing/";

//        [SetUp]
//        public void Setup()
//        {
//            _loggerMock = new Mock<ILogger<VtexSubscriptionClient>>();

//            _settings = new VtexApiSettings
//            {
//                BaseUrl = BaseUrl,
//                AppKey = "test-key",
//                AppToken = "test-token"
//            };

//            _optionsMock = new Mock<IOptions<VtexApiSettings>>();
//            _optionsMock.Setup(o => o.Value).Returns(_settings);
//        }

//        // ------------------------------------------------------------
//        // SUCCESS – PRODUCT OK + PRICE OK
//        // ------------------------------------------------------------
//        [Test]
//        public async Task GetSubscriptionPlansAsync_ReturnsMappedPlans()
//        {
//            var productJson = @"{
//                ""Id"": ""12345"",
//                ""items"": [ { ""Id"": ""111"" } ],
//                ""ShowWithoutStock"": true
//            }";

//            var priceJson = @"{ ""basePrice"": 200 }";

//            // Mock Catalog API
//            var handler = new MockHttpMessageHandler(req =>
//            {
//                if (req.RequestUri!.AbsoluteUri.Contains("productgetbyrefid"))
//                {
//                    return new HttpResponseMessage(HttpStatusCode.OK)
//                    {
//                        Content = new StringContent(productJson)
//                    };
//                }

//                // For pricing: because real client uses _pricingClient (new HttpClient),
//                // we mock it by pointing BaseAddress to fake domain.
//                if (req.RequestUri!.AbsoluteUri.Contains("pricing"))
//                {
//                    return new HttpResponseMessage(HttpStatusCode.OK)
//                    {
//                        Content = new StringContent(priceJson)
//                    };
//                }

//                return new HttpResponseMessage(HttpStatusCode.NotFound);
//            });

//            var catalogClient = new HttpClient(handler)
//            {
//                BaseAddress = new Uri(BaseUrl)
//            };

//            // Instantiate the real client
//            var client = new VtexSubscriptionClient(catalogClient, _optionsMock.Object, _loggerMock.Object);

//            // Override internal pricing client BaseAddress (critical hack)
//            typeof(VtexSubscriptionClient)
//                .GetField("_pricingClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
//                .SetValue(client, new HttpClient(handler)
//                {
//                    BaseAddress = new Uri(PricingBaseUrl)
//                });

//            // ACT
//            var result = await client.GetSubscriptionPlansAsync();

//            // ASSERT
//            Assert.IsNotNull(result);
//            Assert.AreEqual(2, result!.Subscriptions.Count);   // monthly + yearly

//            var monthly = result.Subscriptions.First(p => p.PlanType == "MONTHLY");
//            Assert.AreEqual("1 month", monthly.Frequency);

//            Assert.AreEqual(1, monthly.Skus.Count);
//            Assert.AreEqual("111", monthly.Skus[0].SkuId);
//            Assert.AreEqual(200, monthly.Skus[0].Price);
//            Assert.AreEqual("ACTIVE", monthly.Skus[0].Status);
//            Assert.IsTrue(monthly.Skus[0].StockAvailable);
//        }

//        // ------------------------------------------------------------
//        // PRICE FAILS → INACTIVE SKU
//        // ------------------------------------------------------------
//        [Test]
//        public async Task GetSubscriptionPlansAsync_PricingFails_SkuInactive()
//        {
//            var productJson = @"{
//                ""Id"": ""12345"",
//                ""items"": [ { ""Id"": ""111"" } ],
//                ""ShowWithoutStock"": true
//            }";

//            var handler = new MockHttpMessageHandler(req =>
//            {
//                if (req.RequestUri!.AbsoluteUri.Contains("productgetbyrefid"))
//                {
//                    return new HttpResponseMessage(HttpStatusCode.OK)
//                    {
//                        Content = new StringContent(productJson)
//                    };
//                }

//                if (req.RequestUri!.AbsoluteUri.Contains("pricing"))
//                {
//                    return new HttpResponseMessage(HttpStatusCode.NotFound);
//                }

//                return new HttpResponseMessage(HttpStatusCode.NotFound);
//            });

//            var catalogClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

//            var client = new VtexSubscriptionClient(catalogClient, _optionsMock.Object, _loggerMock.Object);

//            // Override pricing client with mock
//            typeof(VtexSubscriptionClient)
//                .GetField("_pricingClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
//                .SetValue(client, new HttpClient(handler)
//                {
//                    BaseAddress = new Uri(PricingBaseUrl)
//                });

//            var result = await client.GetSubscriptionPlansAsync();

//            var sku = result!.Subscriptions[0].Skus[0];
//            Assert.IsNull(sku.Price);
//            Assert.AreEqual("INACTIVE", sku.Status);
//        }

//        // ------------------------------------------------------------
//        // PRODUCT FAILS → EMPTY LIST
//        // ------------------------------------------------------------
//        [Test]
//        public async Task GetSubscriptionPlansAsync_CatalogFails_ReturnsEmpty()
//        {
//            var handler = new MockHttpMessageHandler(req =>
//            {
//                return new HttpResponseMessage(HttpStatusCode.NotFound);
//            });

//            var catalogClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

//            var client = new VtexSubscriptionClient(catalogClient, _optionsMock.Object, _loggerMock.Object);

//            var result = await client.GetSubscriptionPlansAsync();

//            Assert.AreEqual(0, result!.Subscriptions.Count);
//        }
//    }
//}


using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Tests.TestUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MembershipService.Infrastructure.Tests.Integrations
{
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
                AppToken = "token"
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
        public async Task Success_SingleSku()
        {
            var product = @"{ ""Id"": ""1"", ""items"": [{ ""Id"": ""100"" }], ""ShowWithoutStock"": true }";
            var price = @"{ ""basePrice"": 200 }";

            var client = CreateClient(req =>
            {
                if (req.RequestUri!.AbsoluteUri.Contains("productgetbyrefid"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(product) };

                if (req.RequestUri!.AbsoluteUri.Contains("prices/100"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(price) };

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var result = await client.GetSubscriptionPlansAsync();
            var plan = result!.Subscriptions.First(p => p.PlanType == "MONTHLY");

            Assert.That(plan.Skus.Count, Is.EqualTo(1));
            Assert.That(plan.Skus[0].Price, Is.EqualTo(200));
        }

        [Test]
        public async Task Success_MultipleSkus()
        {
            var product = @"{ ""Id"": ""1"", ""items"": [{ ""Id"": ""100"" }, { ""Id"": ""200"" }] }";
            var price1 = @"{ ""basePrice"": 150 }";
            var price2 = @"{ ""basePrice"": 0 }";

            var client = CreateClient(req =>
            {
                var url = req.RequestUri!.AbsoluteUri;
                if (url.Contains("productgetbyrefid"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(product) };
                if (url.Contains("prices/100"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(price1) };
                if (url.Contains("prices/200"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(price2) };
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var result = await client.GetSubscriptionPlansAsync();
            var plan = result!.Subscriptions.First(p => p.PlanType == "MONTHLY");

            Assert.That(plan.Skus.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task PricingNotFound_MakesSkuInactive()
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
        public async Task CatalogNotFound_ReturnsEmpty()
        {
            var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NotFound));
            var result = await client.GetSubscriptionPlansAsync();
            Assert.That(result!.Subscriptions, Is.Empty);
        }

        [Test]
        public void CatalogTimeout_Throws()
        {
            var client = CreateClient(req => throw new TaskCanceledException());
            Assert.ThrowsAsync<TaskCanceledException>(async () => await client.GetSubscriptionPlansAsync());
        }

        [Test]
        public void PricingTimeout_Throws()
        {
            var product = @"{ ""Id"": ""1"", ""items"": [{ ""Id"": ""100"" }] }";

            var client = CreateClient(req =>
            {
                if (req.RequestUri!.AbsoluteUri.Contains("productgetbyrefid"))
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(product) };
                throw new TaskCanceledException();
            });

            Assert.ThrowsAsync<TaskCanceledException>(async () => await client.GetSubscriptionPlansAsync());
        }
    }
}
