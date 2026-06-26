using AutoMapper;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;

namespace PetCare.Service;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => (int)s.Role));

        CreateMap<Pet, PetDto>();

        CreateMap<Core.Entities.Service, ServiceDto>();

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
            .ForMember(d => d.PetName, o => o.MapFrom(s => s.Pet.Name))
            .ForMember(d => d.PetType, o => o.MapFrom(s => s.Pet.Type))
            .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.Service.Name))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Service.Price))
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.User.Nickname))
            .ForMember(d => d.CustomerPhone, o => o.MapFrom(s => s.User.Phone));

        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
            .ForMember(d => d.PayMethod, o => o.MapFrom(s => (int)s.PayMethod))
            .ForMember(d => d.ServiceName, o => o.Ignore())
            .ForMember(d => d.PetName, o => o.Ignore());

        CreateMap<Coupon, CouponDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => (int)s.Type));

        CreateMap<UserCoupon, UserCouponDto>()
            .ForMember(d => d.CouponName, o => o.MapFrom(s => s.Coupon.Name))
            .ForMember(d => d.Type, o => o.MapFrom(s => (int)s.Coupon.Type))
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Coupon.Value))
            .ForMember(d => d.MinOrderAmount, o => o.MapFrom(s => s.Coupon.MinOrderAmount))
            .ForMember(d => d.ValidFrom, o => o.MapFrom(s => s.Coupon.ValidFrom))
            .ForMember(d => d.ValidTo, o => o.MapFrom(s => s.Coupon.ValidTo));

        CreateMap<Notification, NotificationDto>();
    }
}
