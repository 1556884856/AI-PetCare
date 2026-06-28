namespace PetCare.Core.Dtos;

// ===================================================================
// 认证与用户 相关的 DTO（数据传输对象）
// ===================================================================

/// <summary>登录请求</summary>
public record LoginRequest(string Phone, string Code);

/// <summary>发送验证码请求</summary>
public record SendCodeRequest(string Phone);

/// <summary>登录响应，包含 JWT Token 和用户信息</summary>
public record LoginResponse(string Token, UserDto User);

/// <summary>用户基础信息 DTO，Role 以 int 形式输出便于前端判断</summary>
public record UserDto(int Id, string Phone, string Nickname, string AvatarUrl, int Role);

/// <summary>更新个人信息请求</summary>
public record UpdateProfileRequest(string? Nickname);

// ===================================================================
// 宠物 相关 DTO
// ===================================================================

/// <summary>宠物信息 DTO</summary>
public record PetDto(int Id, string Name, string Type, string Breed, int Age, decimal Weight, string Notes, DateTime CreatedAt);

/// <summary>创建宠物请求</summary>
public record CreatePetRequest(string Name, string Type, string Breed, int Age, decimal Weight, string Notes);

/// <summary>更新宠物请求</summary>
public record UpdatePetRequest(string Name, string Type, string Breed, int Age, decimal Weight, string Notes);

// ===================================================================
// 服务项目 相关 DTO
// ===================================================================

/// <summary>服务项目 DTO</summary>
public record ServiceDto(int Id, string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, string ImageUrl, int SortOrder, bool IsActive);

/// <summary>创建服务请求</summary>
public record CreateServiceRequest(string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, int SortOrder);

/// <summary>更新服务请求</summary>
public record UpdateServiceRequest(string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, int SortOrder, bool IsActive);

// ===================================================================
// 预约 相关 DTO
// ===================================================================

/// <summary>创建预约请求</summary>
public record CreateAppointmentRequest(int ServiceId, int PetId, DateTime AppointmentDate, string TimeSlot, string Notes);

/// <summary>预约详情 DTO，包含关联的宠物、服务、客户信息</summary>
public record AppointmentDto(int Id, int UserId, int PetId, int ServiceId, DateTime AppointmentDate, string TimeSlot, int Status, string Notes, DateTime CreatedAt, string PetName, string PetType, string ServiceName, decimal Price, string? CustomerName, string? CustomerPhone);

// ===================================================================
// 仪表盘 相关 DTO
// ===================================================================

/// <summary>管理仪表盘 DTO，包含统计数据和今日预约列表</summary>
public record DashboardDto(DashboardStatsDto Stats, List<AppointmentDto> TodayAppointments);

/// <summary>仪表盘统计数据</summary>
public record DashboardStatsDto(int TodayAppointments, int PendingAppointments, decimal TodayRevenue, int MonthNewCustomers);

/// <summary>客户信息 DTO（管理后台列表用）</summary>
public record CustomerDto(int Id, string Phone, string Nickname, int Role, int PetCount, int AppointmentCount, DateTime CreatedAt);

// ===================================================================
// 通知 相关 DTO
// ===================================================================

/// <summary>通知 DTO</summary>
public record NotificationDto(int Id, string Title, string Content, int Type, int? RelatedId, bool IsRead, DateTime CreatedAt);

/// <summary>批量发送通知请求</summary>
public record SendNotificationRequest(int[]? UserIds, string Title, string Content, int Type);

// ===================================================================
// 优惠券 相关 DTO
// ===================================================================

/// <summary>优惠券模板 DTO</summary>
public record CouponDto(int Id, string Name, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, int TotalQuantity, int UsedCount, bool IsActive, DateTime CreatedAt);

/// <summary>创建优惠券请求</summary>
public record CreateCouponRequest(string Name, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, int TotalQuantity);

/// <summary>更新优惠券请求</summary>
public record UpdateCouponRequest(string Name, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, int TotalQuantity, bool IsActive);

/// <summary>批量分发优惠券给指定用户</summary>
public record DistributeCouponRequest(int[] UserIds);

/// <summary>用户持有的优惠券 DTO（含优惠券模板信息）</summary>
public record UserCouponDto(int Id, int CouponId, string CouponName, int Type, decimal Value, decimal MinOrderAmount, DateTime ValidFrom, DateTime ValidTo, bool IsUsed, DateTime? UsedAt, DateTime CreatedAt);

// ===================================================================
// 支付 相关 DTO
// ===================================================================

/// <summary>创建支付请求</summary>
public record CreatePaymentRequest(int AppointmentId, int? CouponId, int PayMethod);

/// <summary>支付记录 DTO</summary>
public record PaymentDto(int Id, int AppointmentId, int UserId, decimal Amount, decimal DiscountAmount, decimal FinalAmount, int? CouponId, int Status, int PayMethod, DateTime? PaidAt, DateTime CreatedAt, string ServiceName, string PetName);
