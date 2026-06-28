namespace PetCare.Core.Enums;

/// <summary>用户角色枚举：Customer=普通用户, Staff=店员, Admin=管理员</summary>
public enum UserRole { Customer = 0, Staff = 1, Admin = 2 }

/// <summary>预约状态枚举：Pending=待确认, Confirmed=已确认, Completed=已完成, Cancelled=已取消</summary>
public enum AppointmentStatus { Pending = 0, Confirmed = 1, Completed = 2, Cancelled = 3 }

/// <summary>支付状态枚举：Pending=待支付, Paid=已支付, Refunded=已退款</summary>
public enum PaymentStatus { Pending = 0, Paid = 1, Refunded = 2 }

/// <summary>支付方式枚举：WeChat=微信支付, Alipay=支付宝, InStore=到店支付</summary>
public enum PaymentMethod { WeChat = 0, Alipay = 1, InStore = 2 }

/// <summary>优惠券类型枚举：FixedAmount=满减券, Percentage=折扣券</summary>
public enum CouponType { FixedAmount = 0, Percentage = 1 }

/// <summary>通知类型枚举：System=系统通知, Appointment=预约通知, Payment=支付通知, Coupon=优惠券通知, Promotion=促销通知</summary>
public enum NotificationType { System = 0, Appointment = 1, Payment = 2, Coupon = 3, Promotion = 4 }
