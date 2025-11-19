using MembershipService.Infrastructure.Interfaces;
using MembershipService.Infrastructure.Integrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace MembershipService.Infrastructure.Extensions
{
    namespace MembershipService.Infrastructure.Extensions
    {
        public static class InfrastructureServiceExtensions
        {
            public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddHttpClient<IVtexMembershipRepository, VtexMembershipRepository>().AddPolicyHandler(GetRetryPolicy());

                services.AddHttpClient<IVtexSubscriptionClient, VtexSubscriptionClient>()
                   .AddPolicyHandler(GetRetryPolicy());
                return services;
            }
            private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            {
                return HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            }
        }
    }

}

