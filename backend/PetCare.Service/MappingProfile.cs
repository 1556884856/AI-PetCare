using AutoMapper;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;

namespace PetCare.Service;

/// <summary>
/// AutoMapper 映射配置，定义 Entity → DTO 的转换规则。
/// 枚举值转换为 int 便于前端使用，导航属性的扁平化字段在这里配置。
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User → UserDto：Role 枚举转 int
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => (int)s.Role));

        // Pet → PetDto：字段名一致，直接映射
        CreateMap<Pet, PetDto>();

        // Service → ServiceDto
        CreateMap<Core.Entities.Service, ServiceDto>();

        // Appointment → AppointmentDto：展开关联实体的名称字段
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
            .ForMember(d => d.PetName, o => o.MapFrom(s => s.Pet.Name))
            .ForMember(d => d.PetType, o => o.MapFrom(s => s.Pet.Type))
            .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.Service.Name))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Service.Price))
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.User.Nickname))
            .ForMember(d => d.CustomerPhone, o => o.MapFrom(s => s.User.Phone));

        // Payment → PaymentDto：ServiceName 和 PetName 在业务层单独填充
        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
            .ForMember(d => d.PayMethod, o => o.MapFrom(s => (int)s.PayMethod))
            .ForMember(d => d.ServiceName, o => o.Ignore())
            .ForMember(d => d.PetName, o => o.Ignore());

        // Coupon → CouponDto
        CreateMap<Coupon, CouponDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => (int)s.Type));

        // UserCoupon → UserCouponDto：展开 Coupon 的子属性
        CreateMap<UserCoupon, UserCouponDto>()
            .ForMember(d => d.CouponName, o => o.MapFrom(s => s.Coupon.Name))
            .ForMember(d => d.Type, o => o.MapFrom(s => (int)s.Coupon.Type))
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Coupon.Value))
            .ForMember(d => d.MinOrderAmount, o => o.MapFrom(s => s.Coupon.MinOrderAmount))
            .ForMember(d => d.ValidFrom, o => o.MapFrom(s => s.Coupon.ValidFrom))
            .ForMember(d => d.ValidTo, o => o.MapFrom(s => s.Coupon.ValidTo));

        // Notification → NotificationDto
        CreateMap<Notification, NotificationDto>();
    }
}
