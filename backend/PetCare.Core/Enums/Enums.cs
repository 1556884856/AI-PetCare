namespace PetCare.Core.Enums;

public enum UserRole { Customer = 0, Staff = 1, Admin = 2 }
public enum AppointmentStatus { Pending = 0, Confirmed = 1, Completed = 2, Cancelled = 3 }
public enum PaymentStatus { Pending = 0, Paid = 1, Refunded = 2 }
public enum PaymentMethod { WeChat = 0, Alipay = 1, InStore = 2 }
public enum CouponType { FixedAmount = 0, Percentage = 1 }
public enum NotificationType { System = 0, Appointment = 1, Payment = 2, Coupon = 3, Promotion = 4 }
