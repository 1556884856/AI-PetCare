namespace PetCare.Core.Dtos;

// ===== Auth & User =====
public record LoginRequest(string Phone, string Code);
public record SendCodeRequest(string Phone);
public record LoginResponse(string Token, UserDto User);

public record UserDto(int Id, string Phone, string Nickname, string AvatarUrl, int Role);
public record UpdateProfileRequest(string? Nickname);

// ===== Pet =====
public record PetDto(int Id, string Name, string Type, string Breed, int Age, decimal Weight, string Notes, DateTime CreatedAt);
public record CreatePetRequest(string Name, string Type, string Breed, int Age, decimal Weight, string Notes);
public record UpdatePetRequest(string Name, string Type, string Breed, int Age, decimal Weight, string Notes);

// ===== Service =====
public record ServiceDto(int Id, string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, string ImageUrl, int SortOrder, bool IsActive);
public record CreateServiceRequest(string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, int SortOrder);
public record UpdateServiceRequest(string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, int SortOrder, bool IsActive);

// ===== Appointment =====
public record CreateAppointmentRequest(int ServiceId, int PetId, DateTime AppointmentDate, string TimeSlot, string Notes);
public record AppointmentDto(int Id, int UserId, int PetId, int ServiceId, DateTime AppointmentDate, string TimeSlot, int Status, string Notes, DateTime CreatedAt, string PetName, string PetType, string ServiceName, decimal Price, string? CustomerName, string? CustomerPhone);

// ===== Dashboard =====
public record DashboardDto(DashboardStatsDto Stats, List<AppointmentDto> TodayAppointments);
public record DashboardStatsDto(int TodayAppointments, int PendingAppointments, decimal TodayRevenue, int MonthNewCustomers);

public record CustomerDto(int Id, string Phone, string Nickname, int Role, int PetCount, int AppointmentCount, DateTime CreatedAt);

// ===== Notification =====
public record NotificationDto(int Id, string Title, string Content, int Type, int? RelatedId, bool IsRead, DateTime CreatedAt);
public record SendNotificationRequest(int[]? UserIds, string Title, string Content, int Type);

// ===== Coupon =====
public record CouponDto(int Id, string Name, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, int TotalQuantity, int UsedCount, bool IsActive, DateTime CreatedAt);
public record CreateCouponRequest(string Name, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, int TotalQuantity);
public record UpdateCouponRequest(string Name, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, int TotalQuantity, bool IsActive);
public record DistributeCouponRequest(int[] UserIds);
public record UserCouponDto(int Id, int CouponId, string CouponName, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, bool IsUsed, DateTime? UsedAt, DateTime CreatedAt);

// ===== Payment =====
public record CreatePaymentRequest(int AppointmentId, int? CouponId, int PayMethod);
public record PaymentDto(int Id, int AppointmentId, int UserId, decimal Amount, decimal DiscountAmount, decimal FinalAmount, int? CouponId, int Status, int PayMethod, DateTime? PaidAt, DateTime CreatedAt, string ServiceName, string PetName);
