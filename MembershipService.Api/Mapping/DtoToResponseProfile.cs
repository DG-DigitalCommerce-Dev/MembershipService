using AutoMapper;
using MembershipService.Application.DTOs;
using MembershipService.Api.Models;

namespace MembershipService.Api.Mapping
{
    public class DtoToResponseProfile :Profile
    {
        public DtoToResponseProfile()
        {
            CreateMap<MembershipDto, MembershipResponse>()
                .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.PlanDto))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddressDto))
                .ForMember(dest => dest.PurchaseSettings, opt => opt.MapFrom(src => src.PurchaseSettingsDto))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ItemDtos));

            CreateMap<PlanDto, Plan>()
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.FrequencyDto))
                .ForMember(dest => dest.Validity, opt => opt.MapFrom(src => src.ValidityDto));

            CreateMap<PurchaseSettingsDto, PurchaseSettings>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethodDto));

            CreateMap<ItemDto, Item>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.AttachmentDtos));

            CreateMap<FrequencyDto, Frequency>();
            CreateMap<ValidityDto, Validity>();
            CreateMap<ShippingAddressDto, ShippingAddress>();
            CreateMap<PaymentMethodDto, PaymentMethod>();
            CreateMap<AttachmentDto, Attachment>();
            CreateMap<AttachmentContentDto, AttachmentContent>();
            CreateMap<CustomDataDto, CustomData>();
        }
    }
}
