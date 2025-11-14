using MembershipService.Application.DTOs;

namespace MembershipService.Application.Responses
{
    public class SubscriptionResponse
    {
        public List<SubscriptionDto> Subscriptions { get; set; } = new();
        public string? Error { get; set; }
    }
}
