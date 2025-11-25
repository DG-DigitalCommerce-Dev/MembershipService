using MembershipService.Domain.Models;

namespace MembershipService.Infrastructure.Interfaces
{
    public interface IVtexSubscriptionClient
    {
        Task<Subscription?> GetSubscriptionPlansAsync();
        Task<Subscription?> GetSubscriptionAsync(string subscriptionId);

        Task<bool> CancelSubscriptionAsync(string subscriptionId);
        Task<bool> HasUserUsedTrialAsync(string customerEmail);

    }
}
