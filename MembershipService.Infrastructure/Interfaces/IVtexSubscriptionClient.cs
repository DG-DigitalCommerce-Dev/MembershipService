using MembershipService.Domain.Models;

namespace MembershipService.Infrastructure.Interfaces
{
    public interface IVtexSubscriptionClient
    {
        Task<SubscriptionResponse?> GetSubscriptionPlansAsync();
    }
}
