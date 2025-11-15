using AutoMapper;
using MembershipService.Application.DTOs;
using MembershipService.Application.Models;
namespace MembershipService.Application.Common.Mappings;
public class SubscriptionProfile : Profile
{
    public SubscriptionProfile()
    {
        CreateMap<SubscriptionDto, SubscriptionResponseModel>();
        CreateMap<SkuDto, SkuResponseModel>();
    }
}
