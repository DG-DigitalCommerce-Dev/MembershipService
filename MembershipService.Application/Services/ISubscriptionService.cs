using System.Collections.Generic;
using System.Threading.Tasks;

namespace MembershipService.Application.Services
{
    public interface ISubscriptionService
    {
        // This is the public method the Controller calls.
        Task<List<object>> GetSubscriptionsWithPricingAsync(List<string> refIds);
    }
}