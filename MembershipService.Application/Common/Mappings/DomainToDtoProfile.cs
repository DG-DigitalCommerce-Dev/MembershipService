using AutoMapper;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Models;
namespace MembershipService.Application.Mapping
{
    public class DomainToDtoProfile : Profile
    {
        public DomainToDtoProfile()
        {
            CreateMap<SubscriptionPlan, SubscriptionDto>();
            CreateMap<Sku, SkuDto>();
        }
    }
}
