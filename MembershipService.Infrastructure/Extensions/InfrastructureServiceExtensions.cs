using MembershipService.Infrastructure.Interfaces;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Integrations.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.Logging;
using System; // Required for TimeSpan, Math
using System.Net.Http; // Required for HttpResponseMessage

namespace MembershipService.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IVtexMembershipRepository, VtexMembershipRepository>()
                // 1. Change this to use the overload that provides the ServiceProvider
                .AddPolicyHandler((serviceProvider, request) =>
                    GetRetryPolicy(serviceProvider)
                );

            return services;
        }

        // 2. Update the method to accept IServiceProvider
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider)
        {
            // 3. Get a logger
            var logger = serviceProvider.GetRequiredService<ILogger<VtexMembershipRepository>>();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    2, // 2 retries
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2s, then 4s
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        // 4. THIS IS THE KEY: Log on every retry
                        logger.LogWarning(
                            "Transient error detected: {StatusCode}. Delaying for {Delay}ms, then making retry {RetryAttempt}.",
                            outcome.Result?.StatusCode,
                            timespan.TotalMilliseconds,
                            retryAttempt);
                    }
                );
        }
    }
}