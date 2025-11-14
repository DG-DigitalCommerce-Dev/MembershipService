using MembershipService.Domain.Models;

namespace MembershipService.Infrastructure.Integrations.Interfaces
{
    public interface IVtexClient
    {
        Task<SubscriptionResponse?> GetSubscriptionPlansAsync();
    }
}
