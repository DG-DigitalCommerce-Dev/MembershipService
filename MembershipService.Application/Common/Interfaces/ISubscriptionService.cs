using MembershipService.Application.DTOs;

namespace MembershipService.Application.Common.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDto>> GetAllAsync();
    }
}
