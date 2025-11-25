using MembershipService.Application.DTOs;
using MembershipService.Application.Common.Models;

namespace MembershipService.Application.Common.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync();
        Task<CancelResponseModel> CancelSubscriptionAsync(CancelRequestDto request);
        Task ProcessTrialExpiryAsync();
    }
}
