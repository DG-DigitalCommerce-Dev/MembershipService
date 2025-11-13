using MembershipService.Domain.Models;
using System.Threading.Tasks;

namespace MembershipService.Application.Common.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponse> GetSubscriptionPlansAsync();
    }
}
