using AutoMapper;
using MembershipService.Api.Models;
using MembershipService.Application.DTOs;
namespace MembershipService.Api.Mappings
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<SubscriptionDto, SubscriptionResponse>();
            CreateMap<SkuDto, SkuResponse>();
        }
    }
}
